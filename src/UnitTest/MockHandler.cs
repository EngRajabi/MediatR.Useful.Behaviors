using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest;

public sealed class MockHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseGenerator;

    public MockHandler
        (Func<HttpRequestMessage, HttpResponseMessage> responseGenerator)
    {
        _responseGenerator = responseGenerator;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = _responseGenerator(request);
        response.RequestMessage = request;
        return Task.FromResult(response);
    }
}