// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

namespace Rizzy;

/// <summary>
/// Provides content to <see cref="RzHeadOutlet"/> components.
/// </summary>
public sealed class RzHeadContent : ComponentBase
{
    /// <summary>
    /// Gets or sets the content to be rendered in <see cref="RzHeadOutlet"/> instances.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<SectionContent>(0);
        builder.AddComponentParameter(1, nameof(SectionContent.SectionId), RzHeadOutlet.HeadSectionId);
        builder.AddComponentParameter(2, nameof(SectionContent.ChildContent), ChildContent);
        builder.CloseComponent();
    }
}