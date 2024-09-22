using Docentify.Application.Users.Commands;
using Docentify.Application.Users.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace DocentifyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    UserQueryHandler queryHandler,
    UserCommandHandler commandHandler,
    IConfiguration configuration) : ControllerBase
{
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var result = await commandHandler.RegisterUserAsync(command, cancellationToken);
        return Ok();
    }
    
}