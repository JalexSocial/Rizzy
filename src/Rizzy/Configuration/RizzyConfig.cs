using Microsoft.AspNetCore.Components;
using Rizzy.Htmx;

namespace Rizzy.Configuration;

public class RizzyConfig
{
    private Type? _defaultLayout = null;
    private Type? _rootComponent = typeof(HtmxApp<EmptyLayout>);

    public AntiforgeryStrategy AntiforgeryStrategy { get; set; } = AntiforgeryStrategy.GenerateTokensPerPage;

    /// <summary>
    /// The URL of the Rizzy client script that <c>&lt;RzScripts /&gt;</c> loads after htmx.
    /// Default <c>_content/Rizzy/js/rizzy.js</c> (the copy in the Rizzy package).
    /// </summary>
    public string ClientScriptUrl { get; set; } = "_content/Rizzy/js/rizzy.js";

    /// <summary>
    /// Layout that is applied to all pages without a layout attribute 
    /// </summary>
    public Type? DefaultLayout
    {
        get => _defaultLayout;
        set
        {
            if (value != null && !typeof(LayoutComponentBase).IsAssignableFrom(value))
                throw new ArgumentException($"{value.FullName} is not a Razor layout component (it must derive from LayoutComponentBase).", nameof(DefaultLayout));

            _defaultLayout = value;
        }
    }

    /// <summary>
    /// Core application layout that contains full html page layout
    /// </summary>
    public Type? RootComponent
    {
        get => _rootComponent;
        set
        {
            if (value != null)
            {
                // Check if value is of type HtmxApp<T> where T is a subclass of LayoutComponentBase
                if (!value.IsGenericType || value.GetGenericTypeDefinition() != typeof(HtmxApp<>) ||
                    !typeof(LayoutComponentBase).IsAssignableFrom(value.GetGenericArguments()[0]))
                {
                    throw new ArgumentException($"{value.FullName} is not a valid root component. Use HtmxApp<TLayout>, where TLayout derives from LayoutComponentBase.", nameof(RootComponent));
                }
            }

            _rootComponent = value;
        }
    }
}
