using Newtonsoft.Json;

namespace CodingChallenge.Domain;

public class ErrorResponse : TflApiResponse
{
    [JsonProperty("$type")]
    public DateTime Type { get; set; }

    [JsonProperty("timestampUtc")]
    public DateTime TimestampUtc { get; set; }

    [JsonProperty("exceptionType")]
    public string ExceptionType { get; set; }

    [JsonProperty("httpStatusCode")]
    public int HttpStatusCode { get; set; }

    [JsonProperty("httpStatus")]
    public string HttpStatus { get; set; }

    [JsonProperty("relativeUri")]
    public string RelativeUri { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}