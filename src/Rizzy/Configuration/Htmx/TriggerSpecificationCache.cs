using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rizzy.Configuration.Htmx;

public class TriggerSpecificationCache : Dictionary<string, HtmxTriggerSpecification[]> { }

public class HtmxTriggerSpecification
{
    [JsonPropertyName("trigger")]
    public string Trigger { get; set; } = string.Empty;
    [JsonPropertyName("sseEvent")]
    public string? SseEvent { get; set; }
    [JsonPropertyName("eventFilter")]
    public string? EventFilter { get; set; }
    [JsonPropertyName("changed")]
    public bool? Changed { get; set; }
    [JsonPropertyName("once")]
    public bool? Once { get; set; }
    [JsonPropertyName("consume")]
    public bool? Consume { get; set; }
    [JsonPropertyName("from")]
    public string? From { get; set; }
    [JsonPropertyName("target")]
    public string? Target { get; set; }
    [JsonPropertyName("throttle")]
    public int? Throttle { get; set; }
    [JsonPropertyName("queue")]
    public string? Queue { get; set; }
    [JsonPropertyName("root")]
    public string? Root { get; set; }
    [JsonPropertyName("threshold")]
    public string? Threshold { get; set; }
    [JsonPropertyName("delay")]
    public int? Delay { get; set; }
    [JsonPropertyName("pollInterval")]
    public int? PollInterval { get; set; }
}
