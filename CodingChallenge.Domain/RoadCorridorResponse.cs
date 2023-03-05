
using Newtonsoft.Json;

namespace CodingChallenge.Domain;

public class RoadCorridorResponses : TflApiResponse
{
    [JsonProperty("items")]
    public List<RoadCorridorResponse> Roads { get; set; }
}

public class RoadCorridorResponse 
{
    [JsonProperty("$type")]
    public string Type { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    [JsonProperty("statusSeverity")]
    public string StatusSeverity { get; set; }

    [JsonProperty("statusSeverityDescription")]
    public string StatusSeverityDescription { get; set; }
}