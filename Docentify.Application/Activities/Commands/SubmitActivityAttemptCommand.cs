using Docentify.Application.Activities.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Docentify.Application.Activities.Commands;

public class SubmitActivityAttemptCommand
{
    [FromRoute]
    public int ActivityId { get; set; }
    
    [FromBody]
    public List<QuestionAnswerValueObject> Answers { get; set; }
}