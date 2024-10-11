using Docentify.Application.Users.Commands;
using Docentify.Application.Users.Handlers;
using Docentify.Application.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentifyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    UserQueryHandler queryHandler,
    UserCommandHandler commandHandler,
    IConfiguration configuration) : ControllerBase
{
    [Authorize(Roles = "Users,Institutions")]
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUserById([FromRoute] int userId, CancellationToken cancellationToken)
    {
        var query = new GetUserQuery { UserId = userId };
        
        var result = await queryHandler.GetUserByIdAsync(query, Request, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "Users")]
    [HttpPatch]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await commandHandler.UpdateUserAsync(command, Request, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "Users")]
    [HttpGet("Preferences")]
    public async Task<IActionResult> GetUserPreferences(CancellationToken cancellationToken)
    {
        var result = await queryHandler.GetUserPreferencesAsync(Request, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "Users")]
    [HttpPatch("Preferences")]
    public async Task<IActionResult> UpdateUserPreferences([FromBody] UpdateUserPreferencesCommand command, CancellationToken cancellationToken)
    {
        var result = await commandHandler.UpdateUserPreferencesAsync(command, Request, cancellationToken);
        return Ok(result);
    }
}