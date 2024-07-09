using Microsoft.AspNetCore.Mvc;

namespace Manager.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] Request request)
    {
        if (string.IsNullOrEmpty(request.Description))
            return StatusCode(400, "Description is required");

        return StatusCode(201);
    }
}

public record Request(string Description);