using Docentify.Application.Courses.Commands;
using Docentify.Application.Courses.Handlers;
using Docentify.Application.Courses.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentifyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController(
    CourseQueryHandler queryHandler,
    CourseCommandHandler commandHandler,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet("User")]
    public async Task<IActionResult> QueryCourses([FromQuery] QueryCoursesQuery query, CancellationToken cancellationToken)
    {
        var result = await queryHandler.QueryCoursesAsync(query, Request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("Institution/{institutionId:int}")]
    public async Task<IActionResult> GetInstitutionCourses([FromRoute] int institutionId, [FromQuery] GetInstitutionCoursesQuery query, CancellationToken cancellationToken)
    {
        query.InstitutionId = institutionId;
        
        var result = await queryHandler.GetInstitutionCoursesAsync(query, Request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{courseId:int}")]
    public async Task<IActionResult> GetCourseById([FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var query = new GetCourseByIdQuery
        {
            CourseId = courseId
        };
        
        var result = await queryHandler.GetCourseByIdAsync(query, Request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{courseId:int}/with-steps")]
    public async Task<IActionResult> GetCourseByIdWithSteps([FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var query = new GetCourseByIdQuery
        {
            CourseId = courseId
        };
        
        var result = await queryHandler.GetCourseByIdWithStepsAsync(query, Request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("Favorite/{courseId:int}")]
    public async Task<IActionResult> FavoriteCourse([FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var command = new CourseCommand
        {
            CourseId = courseId
        };
        
        await commandHandler.FavoriteCourseAsync(command, Request, cancellationToken);
        return Ok();
    }

    [HttpPost("Enroll/{courseId:int}")]
    public async Task<IActionResult> EnrollCourse([FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var command = new CourseCommand
        {
            CourseId = courseId
        };
        
        await commandHandler.EnrollCourseAsync(command, Request, cancellationToken);
        return Ok();
    }

    [HttpPost("Discontinue/{courseId:int}")]
    public async Task<IActionResult> DiscontinueCourse([FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var command = new CourseCommand
        {
            CourseId = courseId
        };
        
        await commandHandler.DiscontinueCourseAsync(command, Request, cancellationToken);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> InsertCourse([FromBody] InsertCourseCommand command, CancellationToken cancellationToken)
    {
        var result = await commandHandler.InsertCourseAsync(command, Request, cancellationToken);
        return Created(string.Empty, result);
    }

    [HttpPatch("{courseId:int}")]
    public async Task<IActionResult> UpdateCourse([FromRoute] int courseId, [FromBody] UpdateCourseCommand command, CancellationToken cancellationToken)
    {
        command.Id = courseId;
        
        var result = await commandHandler.UpdateCourseAsync(command, Request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{courseId:int}")]
    public async Task<IActionResult> DeleteCourse([FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var command = new CourseCommand
        {
            CourseId = courseId
        };
        
        await commandHandler.DeleteCourseAsync(command, Request, cancellationToken);
        return NoContent();
    }
}