using Docentify.Application.Activities.Commands;
using Docentify.Application.Activities.Handlers;
using Docentify.Application.Activities.Queries;
using Docentify.Application.Activities.ViewModels;
using Docentify.Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentifyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActivityController(
    ActivityQueryHandler queryHandler,
    ActivityCommandHandler commandHandler,
    IConfiguration configuration) : ControllerBase
{
    [Authorize(Roles = "Users,Institutions")]
    [HttpGet("{activityId:int}")]
    public async Task<IActionResult> GetActivityById([FromRoute] int activityId, CancellationToken cancellationToken)
    {
        var query = new GetActivityByIdQuery { ActivityId = activityId };
        
        var jwtData = JwtUtils.GetJwtDataFromRequest(Request);
        ActivityViewModel result;
        if (jwtData["aud"] == "Users")
        {
            result = await queryHandler.GetActivityByIdUserAsync(query, Request, cancellationToken);
        } else
        {
            result = await queryHandler.GetActivityByIdInstitutionAsync(query, Request, cancellationToken);
        }
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Users,Institutions")]
    [HttpGet("{activityId:int}/Attempt")]
    public async Task<IActionResult> GetActivityAttemptHistory(GetActivityAttemptHistoryQuery query, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(Request);
        
        List<AttemptViewModel> result;
        if (jwtData["aud"] == "Users")
        {
            result = await queryHandler.GetActivityAttemptHistoryUserAsync(query, Request, cancellationToken);
        } else
        {
            result = await queryHandler.GetActivityAttemptHistoryInstitutionAsync(query, Request, cancellationToken);
        }
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Users")]
    [HttpPost("{activityId:int}/Attempt")]
    public async Task<IActionResult> SubmitActivityAttempt(SubmitActivityAttemptCommand command, CancellationToken cancellationToken)
    {
        return Ok(await commandHandler.SubmitActivityAttemptAsync(command, Request, cancellationToken));
    }
    
    [Authorize(Roles = "Institutions")]
    [HttpPost("Step/{stepId:int}")]
    public async Task<IActionResult> InsertActivity(int stepId, InsertActivityCommand command, CancellationToken cancellationToken)
    {
        command.StepId = stepId;
        
        return Created(string.Empty, await commandHandler.InsertActivityAsync(command, Request, cancellationToken));
    }
    
    [Authorize(Roles = "Institutions")]
    [HttpPost("{activityId:int}/Question")]
    public async Task<IActionResult> InsertQuestion([FromRoute] int activityId, [FromBody] InsertQuestionCommand command, CancellationToken cancellationToken)
    {
        command.ActivityId = activityId;
        
        return Created(string.Empty, await commandHandler.InsertQuestionAsync(command, Request, cancellationToken));
    }
    
    [Authorize(Roles = "Institutions")]
    [HttpPatch("{activityId:int}")]
    public async Task<IActionResult> UpdateActivity([FromRoute] int activityId, [FromBody] UpdateActivityCommand command, CancellationToken cancellationToken)
    {
        command.ActivityId = activityId;
        
        var result = await commandHandler.UpdateActivityAsync(command, Request, cancellationToken);
        
        return Ok(result);
    }
    
    [Authorize(Roles = "Institutions")]
    [HttpPatch("Question/{questionId:int}")]
    public async Task<IActionResult> UpdateActivity([FromRoute] int questionId, [FromBody] UpdateQuestionCommand command, CancellationToken cancellationToken)
    {
        command.QuestionId = questionId;
        
        var result = await commandHandler.UpdateQuestionAsync(command, Request, cancellationToken);
        
        return Ok(result);
    }

    
    [Authorize(Roles = "Institutions")]
    [HttpDelete("{activityId:int}")]
    public async Task<IActionResult> DeleteActivity([FromRoute] int activityId, CancellationToken cancellationToken)
    {
        var command = new DeleteActivityCommand { ActivityId = activityId };
        
        await commandHandler.DeleteActivityAsync(command, Request, cancellationToken);
        return NoContent();
    }
    
    [Authorize(Roles = "Institutions")]
    [HttpDelete("Question/{questionId:int}")]
    public async Task<IActionResult> DeleteQuestion([FromRoute] int questionId, CancellationToken cancellationToken)
    {
        var command = new DeleteQuestionCommand { QuestionId = questionId };
        
        await commandHandler.DeleteQuestionAsync(command, Request, cancellationToken);
        return NoContent();
    }
}