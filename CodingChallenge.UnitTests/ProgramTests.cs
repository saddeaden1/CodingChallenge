using Autofac;
using AutoFixture;
using CodingChallenge.Domain;
using CodingChallenge.TestHelpers;
using FluentAssertions;
using Moq;

namespace CodingChallenge.UnitTests;

public class ProgramTests
{
    private Mock<IRoadsService> _roadServiceMock;
    private IFixture _fixture;
    private StringWriter _consoleOutput;
    private TestEnvironmentExiter _environmentExiter;

    [SetUp]
    public void Setup()
    {
        var builder = new ContainerBuilder();
        _fixture = new Fixture();

        _roadServiceMock = new Mock<IRoadsService>();

        builder.Register(_ => _roadServiceMock.Object).As<IRoadsService>();

        _environmentExiter = new TestEnvironmentExiter();
        builder.Register(_=> _environmentExiter).As<IEnvironmentExiter>();

        _ = new Program(builder.Build());

        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);
    }

    [Test]
    public async Task Main_GoodRoadResponse_ReturnsRoadCorridorResponse()
    {
        var roadName = _fixture.Create<string>();

        _roadServiceMock.Setup(x => x.ValidateRoadName(It.IsAny<string>())).ReturnsAsync(true);

        var roadCorridor = _fixture.Build<RoadCorridor>().With(x => x.DisplayName, roadName).Create();
        _roadServiceMock.Setup(x => x.GetRoad(It.IsAny<string>())).ReturnsAsync(roadCorridor);

        try
        {
            await Program.Main(new[] { roadName });
        }
        catch (UnitTestException)
        {
        }

        _environmentExiter.ExitCode.Should().Be(0);
        _consoleOutput.ToString().Should().Contain($"The status of the road {roadCorridor.DisplayName} is as follows");
        _consoleOutput.ToString().Should().Contain($"Road Status is {roadCorridor.StatusSeverity}");
        _consoleOutput.ToString().Should().Contain($"Road Status Description is {roadCorridor.StatusSeverityDescription}");
    }

    [Test]
    public async Task Main_NotValidRoad_ReturnsNotFoundError()
    {
        var roadName = _fixture.Create<string>();

        _roadServiceMock.Setup(x => x.ValidateRoadName(It.IsAny<string>())).ReturnsAsync(false);

        try
        {
            await Program.Main(new[] { roadName });
        }
        catch (UnitTestException)
        {
        }

        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain($"{roadName} is not a valid road");
        _consoleOutput.ToString().Should().NotContain("Http Status code:");
        _consoleOutput.ToString().Should().NotContain("ExceptionType:");
    }

    [Test]
    public async Task Main_ApiErrorInGetRoad_ReturnsError()
    {
        var roadName = _fixture.Create<string>();

        _roadServiceMock.Setup(x => x.ValidateRoadName(It.IsAny<string>())).ReturnsAsync(true);

        var error = new Error
        {
            ExceptionType = "System.Exception",
            HttpStatusCode = 500,
            Message = "An error occurred while processing your request."
        };

        _roadServiceMock.Setup(x => x.GetRoad(It.IsAny<string>())).ReturnsAsync(error);

        try
        {
            await Program.Main(new[] { roadName });
        }
        catch (UnitTestException)
        {
        }

        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain($"An error has occurred with the message: {error.Message}");
        _consoleOutput.ToString().Should().Contain($"Http Status code: {error.HttpStatusCode}");
        _consoleOutput.ToString().Should().Contain($"ExceptionType: {error.ExceptionType}");
    }

    [Test]
    public async Task Main_ApiErrorInValidateRoadName_ReturnsError()
    {
        var roadName = _fixture.Create<string>();

        var error = new Error
        {
            ExceptionType = "System.Exception",
            HttpStatusCode = 500,
            Message = "An error occurred while processing your request."
        };
        _roadServiceMock.Setup(x => x.ValidateRoadName(It.IsAny<string>())).ReturnsAsync(error);

        try
        {
            await Program.Main(new[] { roadName });
        }
        catch (UnitTestException)
        {
        }

        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain($"An error has occurred with the message: {error.Message}");
        _consoleOutput.ToString().Should().Contain($"Http Status code: {error.HttpStatusCode}");
        _consoleOutput.ToString().Should().Contain($"ExceptionType: { error.ExceptionType}");
    }

    [Test]
    public async Task Main_RoadNameEmpty_ReturnsNotRoadInputtedMessage()
    {

        try
        {
            await Program.Main(new[] { "" });
        }
        catch (UnitTestException)
        {
        }

        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain("No road inputted please enter a road");
    }

    [Test]
    public async Task Main_RoadNameArrayEmpty_ReturnsNotRoadInputtedMessage()
    {

        try
        {
            await Program.Main(new string[] { });
        }
        catch (UnitTestException)
        {
        }

        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain("No road inputted please enter a road");
    }

    [Test]
    public async Task Main_NullArgs_ReturnsNotRoadInputtedMessage()
    {
        try
        {
            await Program.Main(null);
        }
        catch (UnitTestException)
        {
        }

        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain("No road inputted please enter a road");
    }

    [Test]
    public async Task Main_MultipleRoadNames_ReturnsMultipleRoadsMessage()
    {

        try
        {
            await Program.Main(new[] { _fixture.Create<string>(), _fixture.Create<string>() });
        }
        catch (UnitTestException)
        {
        }

        _environmentExiter.ExitCode.Should().Be(1);
        _consoleOutput.ToString().Should().Contain("Please enter roads one at a time");
    }
}