using Microsoft.AspNetCore.Mvc;

namespace Docentify.Application.Activities.Queries;

public class GetActivityAttemptHistoryByStepIdQuery
{
    [FromRoute]
    public int StepId { get; set; }
}