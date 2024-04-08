using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

public class MockHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = new HttpResponseMessage();

        var uri = request.RequestUri.ToString();
        if (uri.Contains("register"))
        {
            // Simulate a successful registration with a user ID response
            var jsonContent = "{\"id\": \"622a039371dceb0e7e31f297\"}";
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }
        else
        {
            response.StatusCode = HttpStatusCode.BadRequest;
        }

        return Task.FromResult(response);
    }
}
