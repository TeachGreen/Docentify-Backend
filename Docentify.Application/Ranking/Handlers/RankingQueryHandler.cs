using Docentify.Application.Ranking.Queries;
using Docentify.Application.Ranking.ValueObjects;
using Docentify.Application.Ranking.ViewModels;
using Docentify.Application.Steps.Queries;
using Docentify.Application.Steps.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Exceptions;
using Docentify.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Docentify.Application.Ranking.Handlers;

public class RankingQueryHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<RankingViewModel> GetRankingAsync(GetRankingQuery query, HttpRequest request,
        CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);

        var user = await context.Users.AsNoTracking()
            .Include(u => u.Enrollments)
            .Where(u => u.Email == jwtData["email"])
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("No user with the provided authentication was found");
        }

        var ranking = context.Database.SqlQueryRaw<RankingPositionValueObject>($"""
            SET @rownum = 0;
            SELECT * FROM (SELECT userid, name, score, (@rownum:=@rownum + 1) AS position FROM 
            (SELECT userid, name, score
            FROM users
            INNER JOIN userscores ON users.id = userscores.userId
            ORDER BY score DESC, name DESC) AS a) AS b
            {(query.Search is not null ? $"WHERE LOWER(name) LIKE '%{query.Search.ToLower()}%'" : "")}
            LIMIT 10 OFFSET {(query.Page - 1) * 10}                   
        """).ToList();
        
            var userRanking = context.Database.SqlQueryRaw<int>($"""
                SET @rownum = 0;
                SELECT position FROM (SELECT userid, name, score, (@rownum:=@rownum + 1) AS position FROM 
                (SELECT userid, name, score
                FROM users
                INNER JOIN userscores ON users.id = userscores.userId
                ORDER BY score DESC, name DESC) AS a) AS b
                WHERE userid = {user.Id}
            """).ToList()[0];
        
        var maxPage = (int)double.Ceiling(context.Database.SqlQueryRaw<int>($"""
            SELECT COUNT(*)
            FROM users
            INNER JOIN userscores ON users.id = userscores.userId
            ORDER BY score DESC, name DESC
            {(query.Search is not null ? $"WHERE LOWER(name) LIKE '%{query.Search.ToLower()}%'" : "")}
        """).ToList()[0] / 10d) + 1;
        
        return new RankingViewModel
        {
            Rankings = ranking,
            MaxPage = maxPage,
            UserRanking = userRanking
        };
    }
}