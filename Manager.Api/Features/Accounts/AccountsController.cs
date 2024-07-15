using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Manager.Api.Features.Accounts;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class AccountsController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<AccountsController> _logger;
    private readonly EntryMeter _entryMeter;
    private readonly IMemoryCache _cache;

    public AccountsController(IAccountRepository accountRepository,
        ILogger<AccountsController> logger,
        EntryMeter entryMeter,
        IMemoryCache cache)
    {
        _accountRepository = accountRepository;
        _logger = logger;
        _entryMeter = entryMeter;
        _cache = cache;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAccountRequest request)
    {
        _logger.LogInformation($"Started {MethodBase.GetCurrentMethod()}");
        //try
        //{
        if (string.IsNullOrEmpty(request.Description))
            return BadRequest("Description is required");

        var account = new Account() { Description = request.Description };
        var response = await _accountRepository.Insert(account);

        if (response.IsFailed)
            return BadRequest("Fail to insert account");

        _entryMeter.ReadsCounter.Add(1);
        _logger.LogInformation($"Ended {MethodBase.GetCurrentMethod()}");
        _cache.Set("cachedData", account, TimeSpan.FromMinutes(10));
        return Created("/accounts", response.Value.Id);
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError("Exception: {exception}", ex.Message);
        //    return Problem(detail: "Tente novamente mais tarde", statusCode: 500);
        //}
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        if (!_cache.TryGetValue("cachedData", out Account? account))
        {
            var accountGet = await _accountRepository.GetById(id);
            if (accountGet.IsSuccess)
            {
                account = accountGet.Value;
                _cache.Set("cachedData", account, TimeSpan.FromMinutes(10));
            }
            else
                return NotFound();
        }
        return Ok(account);
    }
}