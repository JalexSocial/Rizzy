namespace Rizzy.Tests.Pipeline;

/// <summary>
/// Blazor SSR form mapping (<c>[SupplyParameterFromForm]</c>) for Rizzy views (RIZZY-020, RM-C1).
/// </summary>
public class FormPostTests(RizzyTestHost host) : IClassFixture<RizzyTestHost>
{
    private async Task<string> Post(string url, params (string, string)[] fields)
    {
        var content = new FormUrlEncodedContent(fields.Select(f => new KeyValuePair<string, string>(f.Item1, f.Item2)));
        var response = await host.Client.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    [Fact]
    public async Task Controller_view_binds_named_form_values()
        => (await Post("/ctrl/post", ("_handler", "greet"), ("Name", "Ada"))).Should().Contain("posted:Ada");

    [Fact]
    public async Task Minimal_api_view_binds_named_form_values()
        => (await Post("/minimal-post", ("_handler", "greet"), ("Name", "Ada"))).Should().Contain("posted:Ada");

    [Fact]
    public async Task Other_form_names_are_not_bound()
        => (await Post("/ctrl/post", ("_handler", "other"), ("Name", "Ada"))).Should().Contain("posted:&lt;null&gt;");

    [Fact]
    public void Framework_internals_used_for_form_mapping_are_present()
    {
        // Upgrade canary: fails loudly if a new ASP.NET Core version renames the internal
        // HttpContextFormDataProvider/SetFormData or FormCollectionReadOnlyDictionary.
        FluentActions.Invoking(BlazorFormData.EnsureSupported).Should().NotThrow();
    }

    [Fact]
    public async Task Multipart_form_is_read_asynchronously()
    {
        // TestServer disallows synchronous IO by default, so a sync Request.Form read would throw.
        using var content = new MultipartFormDataContent
        {
            { new StringContent("greet"), "_handler" },
            { new StringContent(new string('x', 200_000)), "Name" },
        };
        var response = await host.Client.PostAsync("/ctrl/post", content);
        (await response.Content.ReadAsStringAsync()).Should().Contain("posted:xxxx");
    }
}
