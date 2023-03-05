namespace CodingChallenge;

public class AuthHeaderHandler : HttpClientHandler
{
    private readonly string _appKey;

    public AuthHeaderHandler(string appKey)
    {
        _appKey = appKey;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("app_key", _appKey);

        return base.SendAsync(request, cancellationToken);
    }
}