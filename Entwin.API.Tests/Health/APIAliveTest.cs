using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;

[Trait("Category","Health")]
public class ApiHealthTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiHealthTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/health");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
    }
}
