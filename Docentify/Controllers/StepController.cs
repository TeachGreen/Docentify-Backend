using Docentify.Application.Steps.Commands;
using Docentify.Application.Steps.Handlers;
using Docentify.Application.Steps.Queries;
using Docentify.Application.Steps.ViewModels;
using Docentify.Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentifyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StepController(
    StepQueryHandler queryHandler,
    StepCommandHandler commandHandler,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet("{stepId:int}")]
    public async Task<IActionResult> GetStepById([FromRoute] int stepId, CancellationToken cancellationToken)
    {
        var query = new GetStepByIdQuery { StepId = stepId };
        
        var jwtData = JwtUtils.GetJwtDataFromRequest(Request);
        StepViewModel result;
        if (jwtData["aud"] == "Users")
        {
            result = await queryHandler.GetStepByIdUserAsync(query, Request, cancellationToken);
        } else
        {
            result = await queryHandler.GetStepByIdInstitutionAsync(query, Request, cancellationToken);
        }
        
        return Ok(result);
    }
    
    [HttpPost("Course/{courseId:int}")]
    public async Task<IActionResult> InsertStep(int courseId, InsertStepCommand command, CancellationToken cancellationToken)
    {
        command.CourseId = courseId;
        
        return Created(string.Empty, await commandHandler.InsertStepAsync(command, Request, cancellationToken));
    }
    
    [HttpPatch("{stepId:int}")]
    public async Task<IActionResult> UpdateStep(int stepId, [FromBody] UpdateStepCommand command, CancellationToken cancellationToken)
    {
        command.StepId = stepId;
        
        var result = await commandHandler.UpdateStepAsync(command, Request, cancellationToken);
        
        return Ok(result);
    }
    
    [HttpDelete("{stepId:int}")]
    public async Task<IActionResult> DeleteStep(int stepId, CancellationToken cancellationToken)
    {
        var command = new DeleteStepCommand { StepId = stepId };
        
        await commandHandler.DeleteStepAsync(command, Request, cancellationToken);
        return NoContent();
    }
    
    [HttpPost("Complete/{stepId:int}")]
    public async Task<IActionResult> CompleteStep(int stepId, CancellationToken cancellationToken)
    {
        var command = new CompleteStepCommand { StepId = stepId };
        
        await commandHandler.CompleteStepAsync(command, Request, cancellationToken);
        return NoContent();
    }
}