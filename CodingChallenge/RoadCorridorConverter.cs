using CodingChallenge.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodingChallenge;
public class RoadCorridorConverter : JsonConverter<TflApiResponse>
{
    public override TflApiResponse ReadJson(JsonReader reader, Type objectType, TflApiResponse existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {

        if (reader.TokenType == JsonToken.StartArray)
        {
            var jArray = JArray.Load(reader);

            var jObject = new JObject
            {
                ["items"] = jArray
            };

            return jObject.ToObject<RoadCorridorResponses>();
        }
        else
        {
            JObject obj = JObject.Load(reader);

            var type = obj["$type"]!.ToString();

            switch (type)
            {
                case "Tfl.Api.Presentation.Entities.ApiError, Tfl.Api.Presentation.Entities":
                    return obj.ToObject<ErrorResponse>();
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public override void WriteJson(JsonWriter writer, TflApiResponse value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}