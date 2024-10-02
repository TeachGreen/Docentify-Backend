using Docentify.Application.Authentication.Commands;
using Docentify.Application.Authentication.Handlers;
using Docentify.Application.Authentication.Validators;
using Docentify.Domain.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DocentifyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(
    AuthenticationQueryHandler queryHandler,
    AuthenticationCommandHandler commandHandler,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost("Register/User")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        await commandHandler.RegisterUserAsync(command, cancellationToken);
        
        return NoContent();
    }
    
    [HttpPost("Login/User")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand command, CancellationToken cancellationToken)
    {
        var result = await commandHandler.LoginUserAsync(command, cancellationToken);

        if (result.Item1 == EStatusCode.Unauthorized)
            return Unauthorized(result.Item2);
        
        return Ok(result.Item2);
    }
}