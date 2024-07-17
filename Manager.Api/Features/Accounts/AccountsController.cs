using FluentValidation;
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
    private readonly IValidator<CreateAccountRequest> _validator;
    private readonly ILogger<AccountsController> _logger;
    private readonly EntryMeter _entryMeter;
    private readonly IMemoryCache _cache;

    public AccountsController(IAccountRepository accountRepository,
        ILogger<AccountsController> logger,
        EntryMeter entryMeter,
        IMemoryCache cache,
        IValidator<CreateAccountRequest> validator)
    {
        _accountRepository = accountRepository;
        _logger = logger;
        _entryMeter = entryMeter;
        _cache = cache;
        _validator = validator;
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAccountRequest command)
    {
        _logger.LogInformation($"Started {MethodBase.GetCurrentMethod()}");
        var validation = _validator.Validate(command);
        if (validation.IsInvalid())
            return UnprocessableEntity(validation.ToModelState());
        //try
        //{
        if (string.IsNullOrEmpty(command.Description))
            return BadRequest("Description is required");

        var account = new Account() { Description = command.Description };
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
}