using Manager.Api.Features;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Api.Controllers;

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

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAccountRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Description))
                return StatusCode(400, "Description is required");

            var response = await _accountRepository.Insert(new Account() { Description = request.Description });
            if (response.IsFailed)
                return StatusCode(400, "Fail to insert account");

            return StatusCode(201);
        }
        catch (Exception)
        {
            return StatusCode(500, "Tente novamente mais tarde");
        }
    }
}