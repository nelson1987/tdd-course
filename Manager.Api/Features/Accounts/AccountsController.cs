using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public AccountsController(IAccountRepository accountRepository,
        ILogger<AccountsController> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
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

        _logger.LogInformation($"Ended {MethodBase.GetCurrentMethod()}");

        return Created("/accounts", response.Value.Id);
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError("Exception: {exception}", ex.Message);
        //    return Problem(detail: "Tente novamente mais tarde", statusCode: 500);
        //}
    }
}