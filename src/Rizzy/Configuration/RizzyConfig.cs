﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Rizzy.Configuration;

public class RizzyConfig
{
    private Type? _defaultLayout = null;

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
}
