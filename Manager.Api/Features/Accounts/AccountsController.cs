using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Api.Features.Accounts;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class AccountsController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;

    public AccountsController(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAccountRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Description))
                return BadRequest("Description is required");

            var account = new Account() { Description = request.Description };
            var response = await _accountRepository.Insert(account);

            if (response.IsFailed)
                return BadRequest("Fail to insert account");

            return Created("/accounts", response.Value.Id);
        }
        catch (Exception)
        {
            return Problem(detail: "Tente novamente mais tarde", statusCode: 500);
        }
    }
}