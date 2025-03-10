﻿using Microsoft.AspNetCore.Components;
using Rizzy.Components;
using Rizzy.Htmx;

namespace Rizzy.Configuration;

public class RizzyConfig
{
    private Type? _defaultLayout = null;
    private Type? _rootComponent = typeof(EmptyRootComponent);

    public AntiforgeryStrategy AntiforgeryStrategy { get; set; } = AntiforgeryStrategy.GenerateTokensPerPage;

    /// <summary>
    /// Layout that is applied to all pages without a layout attribute 
    /// </summary>
    public Type? DefaultLayout
    {
        get => _defaultLayout;
        set
        {
            if (value != null && !typeof(LayoutComponentBase).IsAssignableFrom(value))
                throw new Exception($"{nameof(value)} is not a Razor layout component");

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
                    throw new Exception($"{nameof(value)} must be of type HtmxApp<T> where T is a subclass of LayoutComponentBase");
                }
            }

            _rootComponent = value;
        }
    }

    /// <summary>
    /// Gets or sets the Nonce HMAC Key as a base-64 encoded string. This key is used to sign generated nonce values
    /// such that they may be revalidated when submitted again via htmx
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public string NonceHMACKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets the decoded Nonce HMAC Key as a byte array.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public byte[] NonceHMACKeyBytes => Convert.FromBase64String(NonceHMACKey);
}
