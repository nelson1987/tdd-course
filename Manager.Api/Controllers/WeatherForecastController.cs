using Microsoft.AspNetCore.Mvc;

namespace Manager.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(ILogger<AccountsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}