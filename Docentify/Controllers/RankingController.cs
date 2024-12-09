using Docentify.Application.Ranking.Handlers;
using Docentify.Application.Ranking.Queries;
using Microsoft.AspNetCore.Mvc;

namespace DocentifyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RankingController(
    RankingQueryHandler queryHandler,
    RankingCommandHandler commandHandler,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRanking([FromQuery] GetRankingQuery query, CancellationToken cancellationToken)
    {
        return Ok(await queryHandler.GetRankingAsync(query, Request, cancellationToken));
    }
}