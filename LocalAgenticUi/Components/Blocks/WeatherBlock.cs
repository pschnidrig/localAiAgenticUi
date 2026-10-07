using Microsoft.AspNetCore.Components.AI;
using LocalAgenticUi.Tools;

namespace LocalAgenticUi.Components.Blocks;

// The source generator creates a handler that turns every "get_weather" call
// into this block and fills the properties from the arguments and the result.
[ToolBlock(WeatherTool.Name)]
public partial class WeatherBlock : FunctionInvocationContentBlock
{
    [ToolParameter(Name = "city")]
    public string? City { get; set; }

    [ToolResult(Name = "temperatureC")]
    public int? TemperatureC { get; set; }

    [ToolResult(Name = "condition")]
    public string? Condition { get; set; }
}
