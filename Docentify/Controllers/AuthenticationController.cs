using Docentify.Application.Authentication.Commands;
using Docentify.Application.Authentication.Handlers;
using Docentify.Application.Authentication.Validators;
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
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command, RegisterUserCommandValidator validator, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(validation.Errors);
        
        var result = await commandHandler.RegisterUserAsync(command, cancellationToken);
        return Created(string.Empty, result);
    }
    
    [HttpPost("Login/User")]
    public async Task<IActionResult> LoginUser([FromBody] LoginCommand command, [FromServices] LoginCommandValidator validator, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(validation.Errors);
        
        var result = await commandHandler.LoginUserAsync(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("Register/Institution")]
    public async Task<IActionResult> RegisterInstitution([FromBody] RegisterInstitutionCommand command, [FromServices] RegisterInstitutionCommandValidator validator, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(validation.Errors);
        
        var result = await commandHandler.RegisterInstitutionAsync(command, cancellationToken);
        return Created(string.Empty, result);
    }
    
    [HttpPost("Login/Institution")]
    public async Task<IActionResult> LoginInstitution([FromBody] LoginCommand command, LoginCommandValidator validator, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(validation.Errors);
        
        var result = await commandHandler.LoginInstitutionAsync(command, cancellationToken);
        return Ok(result);
    }
}