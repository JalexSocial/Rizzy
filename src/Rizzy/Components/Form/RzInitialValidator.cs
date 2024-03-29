﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Rizzy.Components;

public class RzInitialValidator : ComponentBase
{
    [CascadingParameter]
    public EditContext? EditContext { get; set; }

    protected override void OnParametersSet()
    {
        EditContext?.Validate();
    }
}
