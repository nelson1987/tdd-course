using Microsoft.AspNetCore.Mvc;

namespace Manager.Api.Controllers;

[ApiController]
[Route("[controller]")]
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

            await _accountRepository.Insert(new Account());

            return StatusCode(201);
        }
        catch (Exception)
        {
            return StatusCode(400, "Tente novamente mais tarde");
        }
    }
}

public record CreateAccountRequest(string? Description);

public class Account
{
    public int Id { get; set; }
    public string? Description { get; set; }
}

public interface IAccountRepository
{
    Task<Account> Insert(Account account);
}