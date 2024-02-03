using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Rizzy.Antiforgery;

namespace Rizzy.Configuration;

public class RizzyConfig
{
    private Type? _defaultLayout = null;
    private Type? _rootLayout = null;

	public AntiforgeryStrategy AntiforgeryStrategy { get; set; } = AntiforgeryStrategy.GenerateTokensPerPage;

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

	public Type? RootLayout
	{
		get => _rootLayout;
		set
		{
			if (value != null && !typeof(LayoutComponentBase).IsAssignableFrom(value))
				throw new Exception($"{nameof(value)} is not a Razor layout component");

			_rootLayout = value;
		}
	}

}
