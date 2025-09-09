namespace WebMVC.Infrastructure;
// :: force to add the Bear token to request headers
public class HttpClientAuthorizationDelegatingHandler
    : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authorizationHeader = _httpContextAccessor.HttpContext
            .Request.Headers["Authorization"];

        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
        }

        var token = await GetToken();

        if (token != null)
        {
            DebugLogger.Logger.Log("Add bear to  Authorization  header");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // :: add bear token
        }

        return await base.SendAsync(request, cancellationToken);
    }

    async Task<string> GetToken()
    {
        const string ACCESS_TOKEN = "access_token";
        DebugLogger.Logger.Log(" call IAuthenticationService.GetTokenAsync  `access_token`");
        return await _httpContextAccessor.HttpContext
            .GetTokenAsync(ACCESS_TOKEN);
    }
}
