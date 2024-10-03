using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentifyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    // UserQueryHandler queryHandler,
    // UserCommandHandler commandHandler,
    IConfiguration configuration) : ControllerBase
{
    [Authorize(Roles = "Institution")]
    [HttpGet]
    public async Task<IActionResult> RegisterUser(CancellationToken cancellationToken)
    {
        // var result = await commandHandler.RegisterUserAsync(command, cancellationToken);
        return Ok("teste");
    }
}