using System.Net;

namespace Rizzy.Tests.Pipeline;

/// <summary>Antiforgery validation for controller actions with AddControllersWithViews() (RIZZY-020, RM-C1).</summary>
public class AntiforgeryTests(RizzyTestHost host) : IClassFixture<RizzyTestHost>
{
    private Task<HttpResponseMessage> PostWithoutToken(string url)
        => host.Client.PostAsync(url, new FormUrlEncodedContent([new("Name", "x")]));

    [Fact]
    public async Task ValidateAntiForgeryToken_rejects_posts_without_a_token()
        => (await PostWithoutToken("/ctrl/antiforgery-validated")).StatusCode.Should().Be(HttpStatusCode.BadRequest);
}
