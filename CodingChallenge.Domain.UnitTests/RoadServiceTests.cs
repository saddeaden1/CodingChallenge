using System.Net;
using AutoFixture;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Refit;

namespace CodingChallenge.Domain.UnitTests;

public class RoadServiceTests
{
    private RoadsService _sut;
    private Mock<ITflApi> _mockApi;
    private IFixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _mockApi = new Mock<ITflApi>();
        _sut = new RoadsService(_mockApi.Object);
    }

    [Test]
    public async Task GetRoad_RoadResponse_ReturnsRoadCorridor()
    {
        //arrange
        var roadCorridorResponse = _fixture.Create<RoadCorridorResponse>();

        _mockApi.Setup(x => x.GetRoad(It.IsAny<string>())).ReturnsAsync(new RoadCorridorResponses
        { 
            Roads = new List<RoadCorridorResponse>
            {
                roadCorridorResponse
            }
        });

        //act
        var actual = await _sut.GetRoad(_fixture.Create<string>());

        //assert
        _mockApi.Verify(x => x.GetRoad(It.IsAny<string>()), Times.Once);
        actual.Match(_ =>
        {
            Assert.Fail();
        },
            response =>
            {
                response.Should().BeEquivalentTo(new RoadCorridor
                {
                    StatusSeverityDescription = roadCorridorResponse.StatusSeverityDescription,
                    DisplayName = roadCorridorResponse.DisplayName,
                    StatusSeverity = roadCorridorResponse.StatusSeverity
                });
            });
    }

    [Test]
    public async Task GetRoad_ApiException_ReturnsError()
    {
        //arrange
        var errorResponse = _fixture.Create<ErrorResponse>();

        var apiResponseException = await ApiException.Create(new HttpRequestMessage(), HttpMethod.Get, new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent(JsonConvert.SerializeObject(errorResponse))
        }, new RefitSettings());

        _mockApi.Setup(x => x.GetRoad(It.IsAny<string>())).Throws(apiResponseException);

        //act
        var actual = await _sut.GetRoad(_fixture.Create<string>());

        //assert
        _mockApi.Verify(x => x.GetRoad(It.IsAny<string>()), Times.Once);
        actual.Match(error =>
        {
            error.Should().BeEquivalentTo(new Error
            {
                ExceptionType = errorResponse.ExceptionType,
                HttpStatusCode = errorResponse.HttpStatusCode,
                Message = errorResponse.Message
            });
        },
            _ =>
            {
                Assert.Fail();
            });
    }

    [Test]
    public async Task GetRoad_HttpRequestException_ReturnsError()
    {
        //arrange
        var exceptionMessage = _fixture.Create<string>();
        _mockApi.Setup(x => x.GetRoad(It.IsAny<string>())).Throws(new HttpRequestException(exceptionMessage));

        //act
        var actual = await _sut.GetRoad(_fixture.Create<string>());

        //assert
        _mockApi.Verify(x => x.GetRoad(It.IsAny<string>()), Times.Once);
        actual.Match(error =>
            {
                error.Should().BeEquivalentTo(new Error
                {
                    Message = exceptionMessage
                });
            },
            _ =>
            {
                Assert.Fail();
            });
    }

    [Test]
    public async Task GetRoad_NotImplementedResponseType_ReturnsUnknownTypeResponseError()
    {
        //arrange
        _mockApi.Setup(x => x.GetRoad(It.IsAny<string>())).ThrowsAsync(new NotImplementedException());

        //act
        var actual = await _sut.GetRoad(_fixture.Create<string>());

        //assert
        _mockApi.Verify(x => x.GetRoad(It.IsAny<string>()), Times.Once);
        actual.Match(error =>
        {
            error.Should().BeEquivalentTo(new Error
            {
                Message = "Unknown type returned from TFL Api"
            });
        },
            _ =>
            {
                Assert.Fail();
            });
    }

    [Test]
    public async Task ValidateRoadName_ValidRoad_ReturnsTrue()
    {
        //arrange
        var roads = _fixture.CreateMany<RoadCorridorResponse>().ToList();

        _mockApi.Setup(x => x.GetRoads()).ReturnsAsync(new RoadCorridorResponses
        {
            Roads = roads
        });

        //act
        var roadName = roads.First().Id;
        var actual = await _sut.ValidateRoadName(roadName);

        //assert
        _mockApi.Verify(x => x.GetRoads(), Times.Once);
        actual.Match(_ =>
            {
                Assert.Fail();
            },
            response =>
            {
                response.Should().BeTrue();
            });
    }

    [Test]
    public async Task ValidateRoadName_InvalidRoad_ReturnsFalse()
    {
        //arrange
        var roads = _fixture.CreateMany<RoadCorridorResponse>().ToList();
        _mockApi.Setup(x => x.GetRoads()).ReturnsAsync(new RoadCorridorResponses
        {
            Roads = roads
        });

        //act
        var actual = await _sut.ValidateRoadName(_fixture.Create<string>());

        //assert
        _mockApi.Verify(x => x.GetRoads(), Times.Once);
        actual.Match(_ =>
            {
                Assert.Fail();
            },
            response =>
            {
                response.Should().BeFalse();
            });
    }

    [Test]
    public async Task ValidateRoadName_ApiException_ReturnsError()
    {
        //arrange
        var errorResponse = _fixture.Create<ErrorResponse>();
        var apiResponseException = await ApiException.Create(new HttpRequestMessage(), HttpMethod.Get, new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent(JsonConvert.SerializeObject(errorResponse))
        }, new RefitSettings());

        _mockApi.Setup(x => x.GetRoads()).Throws(apiResponseException);

        //act
        var actual = await _sut.ValidateRoadName(_fixture.Create<string>());

        //assert
        _mockApi.Verify(x => x.GetRoads(), Times.Once);
        actual.Match(error =>
            {
                error.Should().BeEquivalentTo(new Error
                {
                    ExceptionType = errorResponse.ExceptionType,
                    HttpStatusCode = errorResponse.HttpStatusCode,
                    Message = errorResponse.Message
                });
            },
            _ =>
            {
                Assert.Fail();
            });
    }

    [Test]
    public async Task ValidateRoadName_HttpRequestException_ReturnsError()
    {
        //arrange
        var exceptionMessage = _fixture.Create<string>();
        _mockApi.Setup(x => x.GetRoads()).Throws(new HttpRequestException(exceptionMessage));

        //act
        var actual = await _sut.ValidateRoadName(_fixture.Create<string>());

        //assert
        _mockApi.Verify(x => x.GetRoads(), Times.Once);
        actual.Match(error =>
            {
                error.Should().BeEquivalentTo(new Error
                {
                    Message = exceptionMessage
                });
            },
            _ =>
            {
                Assert.Fail();
            });
    }
}