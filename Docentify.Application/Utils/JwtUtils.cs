using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace Docentify.Application.Utils;

public static class JwtUtils
{
    public static Dictionary<string, string> GetJwtDataFromRequest(HttpRequest request)
    {
        var jwt = request.Headers.Authorization.ToString().Replace("Bearer ", "", StringComparison.InvariantCulture);
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(jwt);
        
        var data = jwtSecurityToken.Claims
            .DistinctBy(claim => claim.Type)
            .ToDictionary(claim => claim.Type, claim => claim.Value);

        return data;
    }
}