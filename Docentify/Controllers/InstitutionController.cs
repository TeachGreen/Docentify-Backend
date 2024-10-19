using Docentify.Application.Institutions.Commands;
using Docentify.Application.Institutions.Handlers;
using Docentify.Application.Institutions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentifyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InstitutionController(
    InstitutionQueryHandler queryHandler,
    InstitutionCommandHandler commandHandler,
    IConfiguration configuration) : ControllerBase
{
    [Authorize(Roles = "Users,Institutions")]
    [HttpGet("{institutionId:int}")]
    public async Task<IActionResult> GetInstitutionById([FromRoute] int institutionId, CancellationToken cancellationToken)
    {
        var query = new GetInstitutionByIdQuery { InstitutionId = institutionId };
        
        var result = await queryHandler.GetInstitutionByIdAsync(query, Request, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "Users,Institutions")]
    [HttpGet("{institutionId:int}/Users")]
    public async Task<IActionResult> GetInstitutionUsers([FromRoute] int institutionId, [FromQuery] GetInstitutionUsersQuery query, CancellationToken cancellationToken)
    {
        query.SetInstitutionId(institutionId);
        
        var result = await queryHandler.GetInstitutionUsersAsync(query, Request, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "Institutions")]
    [HttpPatch]
    public async Task<IActionResult> UpdateInstitution([FromBody] UpdateInstitutionCommand command, CancellationToken cancellationToken)
    {
        var result = await commandHandler.UpdateInstitutionAsync(command, Request, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "Institutions")]
    [HttpPost("AssociateById/{userId}")]
    public async Task<IActionResult> AssociateUserById(int userId, CancellationToken cancellationToken)
    {
        var command = new AssociateUserByIdCommand
        {
            UserId = userId
        };
        
        await commandHandler.AssociateUserByIdAsync(command, Request, cancellationToken);
        return Ok();
    }
    
    [Authorize(Roles = "Institutions")]
    [HttpPost("AssociateByDocument/{document}")]
    public async Task<IActionResult> AssociateUserByDocument(string document, CancellationToken cancellationToken)
    {
        var command = new AssociateUserByDocumentCommand
        {
            Document = document
        };
        
        await commandHandler.AssociateUserByDocumentAsync(command, Request, cancellationToken);
        return Ok();
    }
}