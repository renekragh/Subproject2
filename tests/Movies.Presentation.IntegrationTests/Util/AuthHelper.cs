using System.Security.Claims;

namespace Movies.Presentation.IntegrationTests.Util;

public class AuthHelper
{
    public static Dictionary<string, object> GetBearerForUser(string userId)
    {
        return new Dictionary<string, object>{{ClaimTypes.NameIdentifier, userId}};
    }
}
