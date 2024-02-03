using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Rizzy.Antiforgery;
using Rizzy.Components.Layout;

namespace Rizzy.Configuration;

public class RizzyConfig
{
    private Type? _defaultLayout = null;
    private Type? _rootComponent = typeof(EmptyRootComponent);

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

	public Type? RootComponent
	{
		get => _rootComponent;
		set
		{
			if (value != null && !typeof(IComponent).IsAssignableFrom(value))
				throw new Exception($"{nameof(value)} is not a Razor component");

			_rootComponent = value;
		}
	}

}
