using Microsoft.AspNetCore.Mvc;

namespace Docentify.Application.Activities.Queries;

public class GetActivityAttemptHistoryQuery
{
    [FromRoute]
    public int ActivityId { get; set; }
}