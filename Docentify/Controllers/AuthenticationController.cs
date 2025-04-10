using Docentify.Application.Authentication.Commands;
using Docentify.Application.Authentication.Handlers;
using Docentify.Application.Authentication.Validators;
using Docentify.Domain.Exceptions;
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
            throw new BadRequestException(validation.Errors[0].ErrorMessage);
        
        var result = await commandHandler.RegisterUserAsync(command, cancellationToken);
        return Created(string.Empty, result);
    }
    
    [HttpPost("Login/User")]
    public async Task<IActionResult> LoginUser([FromBody] LoginCommand command, [FromServices] LoginCommandValidator validator, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            throw new BadRequestException(validation.Errors[0].ErrorMessage);
        
        var result = await commandHandler.LoginUserAsync(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("Register/Institution")]
    public async Task<IActionResult> RegisterInstitution([FromBody] RegisterInstitutionCommand command, [FromServices] RegisterInstitutionCommandValidator validator, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            throw new BadRequestException(validation.Errors[0].ErrorMessage);
        
        var result = await commandHandler.RegisterInstitutionAsync(command, cancellationToken);
        return Created(string.Empty, result);
    }
    
    [HttpPost("Login/Institution")]
    public async Task<IActionResult> LoginInstitution([FromBody] LoginCommand command, LoginCommandValidator validator, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            throw new BadRequestException(validation.Errors[0].ErrorMessage);
        
        var result = await commandHandler.LoginInstitutionAsync(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("PasswordChangeRequest/User")]
    public async Task<IActionResult> PasswordChangeRequestUser([FromBody] PasswordChangeRequestUserCommand command, CancellationToken cancellationToken)
    {
        var result = await commandHandler.PasswordChangeRequestUserAsync(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("ConfirmIdentity/User")]
    public async Task<IActionResult> IdentityConfirmationUser([FromBody] IdentityConfirmationUserCommand command, CancellationToken cancellationToken)
    {
        var result = await commandHandler.IdentityConfirmationUserAsync(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("PasswordCreation/User")]
    public async Task<IActionResult> NewPasswordCreationUser([FromBody] NewPasswordCreationUserCommand command, CancellationToken cancellationToken)
    {
        var result = await commandHandler.NewPasswordCreationUserAsync(command, cancellationToken);
        return Ok(result);
    }
}