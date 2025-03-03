using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rizzy.Components;

namespace Rizzy.Utility;

public static class RazorRenderer
{
    /// <summary>
    /// Extracts all MarkupContent from the given RenderFragment as a concatenated markdown string.
    /// It removes the first and last lines if they are blank.
    /// </summary>
    /// <param name="fragment">The RenderFragment to process (nullable).</param>
    /// <returns>A string containing the concatenated markup content.</returns>
    public static string AsMarkupString(this RenderFragment? fragment)
    {
        if (fragment == null)
        {
            return string.Empty;
        }

        var builder = new RenderTreeBuilder();
        fragment.Invoke(builder); 
        var frames = builder.GetFrames();
        var markupContentList = new List<string>();

        for (int i = 0; i < frames.Count; i++)
        {
            var frame = frames.Array[i];
            if (frame.FrameType == RenderTreeFrameType.Markup && !frame.MarkupContent != null)
            {
                markupContentList.Add(frame.MarkupContent);
            }
        }

        // Remove the first line if it is blank.
        if (markupContentList.Count > 0 && string.IsNullOrWhiteSpace(markupContentList[0]))
        {
            markupContentList.RemoveAt(0);
        }
        // Remove the last line if it is blank.
        if (markupContentList.Count > 0 && string.IsNullOrWhiteSpace(markupContentList[^1]))
        {
            markupContentList.RemoveAt(markupContentList.Count - 1);
        }

        return string.Join("\n", markupContentList);
    }

    /// <summary>
    /// Render a fragment to html using the Razor renderer
    /// </summary>
    /// <param name="fragment"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static async Task<string> RenderHtmlAsync(this RenderFragment? fragment, IServiceProvider serviceProvider)
    {
        if (fragment is null)
            return string.Empty;

        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var dictionary = new Dictionary<string, object?>
            {
                { "Fragment", fragment }
            };

            var parameters = ParameterView.FromDictionary(dictionary);
            var output = await htmlRenderer.RenderComponentAsync<FragmentComponent>(parameters);

            return output.ToHtmlString();
        });

        return html;
    }
}