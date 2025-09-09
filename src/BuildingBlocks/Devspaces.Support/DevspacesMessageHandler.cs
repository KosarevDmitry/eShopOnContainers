namespace Devspaces.Support;
/// <summary>
/// для чего 
/// </summary>
public class DevspacesMessageHandler : DelegatingHandler
{
    private const string DevspacesHeaderName = "azds-route-as";
    private readonly IHttpContextAccessor _httpContextAccessor;
    public DevspacesMessageHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    // :: add header `azds-route-as`
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        DebugLogger.Logger.Log("add header `azds-route-as` to httpclient request");  
        var req = _httpContextAccessor.HttpContext.Request;

        if (req.Headers.ContainsKey(DevspacesHeaderName))
        {
            request.Headers.Add(DevspacesHeaderName, req.Headers[DevspacesHeaderName] as IEnumerable<string>);
        }
        return base.SendAsync(request, cancellationToken);
    }
}
