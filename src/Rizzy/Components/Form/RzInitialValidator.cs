using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Rizzy.Components.Form;


public class RzInitialValidator : ComponentBase
{
	[CascadingParameter]
	public EditContext? EditContext { get; set; }

	protected override void OnParametersSet()
	{
		EditContext?.Validate();
	}
}
