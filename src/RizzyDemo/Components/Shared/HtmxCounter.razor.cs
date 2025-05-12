using Microsoft.AspNetCore.Components;

namespace RizzyDemo.Components.Shared;

public partial class HtmxCounter : ComponentBase
{
	[Parameter, EditorRequired] 
	public HtmxCounterState State { get; set; } = new();
    
	public class HtmxCounterState
	{
		public int Value { get; set; } = 0;
	}	
}