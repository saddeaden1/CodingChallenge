using CodingChallenge.Domain;
using FluentAssertions;
using Newtonsoft.Json;

namespace CodingChallenge.UnitTests;

public class RoadCorridorConverterTests
{
    [Test]
    public void TestDeserializeRoadCorridor()
    {
        //arrange
        var converter = new RoadCorridorConverter();
        var json = @"[{
                ""$type"": ""Tfl.Api.Presentation.Entities.RoadCorridor, Tfl.Api.Presentation.Entities"",
                ""id"": ""a2"",
                ""displayName"": ""A2"",
                ""statusSeverity"": ""Good"",
                ""statusSeverityDescription"": ""No Exceptional Delays"",
                ""bounds"": ""[[-0.0857,51.44091],[0.17118,51.49438]]"",
                ""envelope"": ""[[-0.0857,51.44091],[-0.0857,51.49438],[0.17118,51.49438],[0.17118,51.44091],[-0.0857,51.44091]]"",
                ""url"": ""/Road/a2""
                 }]";

        //act
        var roadCorridor = JsonConvert.DeserializeObject<TflApiResponse>(json, converter);

        //assert
        roadCorridor.Should().BeEquivalentTo(new RoadCorridorResponses
        {
            Roads = new List<RoadCorridorResponse>()
            {
                new()
                {
                    StatusSeverityDescription = "No Exceptional Delays",
                    DisplayName = "A2",
                    StatusSeverity = "Good",
                    Id = "a2",
                }
            }
        });
    }

    [Test]
    public void TestDeserializeApiError()
    {
        // Arrange
        var converter = new RoadCorridorConverter();
        var json = @"{
                ""$type"": ""Tfl.Api.Presentation.Entities.ApiError, Tfl.Api.Presentation.Entities"",
                ""timestampUtc"": ""2022-03-01T12:00:00Z"",
                ""exceptionType"": ""System.Exception"",
                ""httpStatusCode"": 500,
                ""httpStatus"": ""InternalServerError"",
                ""relativeUri"": ""/Road/a2"",
                ""message"": ""An error occurred while processing your request.""
            }";

        //act
        var apiError = JsonConvert.DeserializeObject<TflApiResponse>(json, converter);
        
        //assert
        apiError.Should().BeEquivalentTo(new ErrorResponse
        {
            ExceptionType = "System.Exception",
            HttpStatusCode = 500,
            Message = "An error occurred while processing your request.",
            HttpStatus = "InternalServerError",
            TimestampUtc = DateTime.Parse("2022-03-01T12:00:00Z"),
            RelativeUri = "/Road/a2"
        });
    }

    [Test]
    public void TestDeserializeApiNotImplemented()
    {
        //arrange
        var converter = new RoadCorridorConverter();
        var json = @"{
                ""$type"": ""NOT IMPLEMENTED"",
                ""timestampUtc"": ""2022-03-01T12:00:00Z"",
                ""exceptionType"": ""System.Exception"",
                ""httpStatusCode"": 500,
                ""httpStatus"": ""InternalServerError"",
                ""relativeUri"": ""/Road/a2"",
                ""message"": ""An error occurred while processing your request.""
            }";

        //act
        var apiError = () => JsonConvert.DeserializeObject<TflApiResponse>(json, converter);

        //assert
        apiError.Should().Throw<NotImplementedException>();
    }
}