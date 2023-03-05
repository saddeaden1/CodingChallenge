using Autofac;
using AutoFixture;
using CodingChallenge.Domain;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;

namespace CodingChallenge.IntegrationTests;

public class ProgramIntegrationTests
{
    private StringWriter _consoleOutput;
    private IFixture _fixture;
    private TestEnvironmentExiter _environmentExiter;

    [SetUp]
    public void Setup()
    {
        var builder = new ContainerBuilder();
        _fixture = new Fixture();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json")
            .Build();

        builder.Register(_ => RestService.For<ITflApi>("https://api.tfl.gov.uk",
            new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new RoadCorridorConverter() }
                }),
                HttpMessageHandlerFactory = () => new AuthHeaderHandler(configuration["Key"])
            })).As<ITflApi>();

        _environmentExiter = new TestEnvironmentExiter();
        builder.Register(_ => _environmentExiter).As<IEnvironmentExiter>();

        builder.RegisterType<RoadsService>().As<IRoadsService>();

        _ = new Program(builder.Build());

        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);
    }

    [Test]
    public async Task GetRoad_ExistingRoad_OutputsRoadStatusResponse()
    {
        var road = "A2";

        try
        {
            await Program.Main(new[] { road });
        }
        catch (UnitTestException)
        {
        }

        _consoleOutput.ToString().Should().Contain($"The status of the road {road} is as follows");
        _consoleOutput.ToString().Should().Contain("Road Status is");
        _consoleOutput.ToString().Should().Contain("Road Status Description is");
        _environmentExiter.ExitCode.Should().Be(0);
    }

    [Test]
    public async Task Run_NonExistentRoad_ReturnsRoadNotFoundError()
    {
        var road = _fixture.Create<string>();

        try
        {
            await Program.Main(new[] { road });
        }
        catch (UnitTestException)
        {
        }

        _consoleOutput.ToString().Should().Contain($"{road} is not a valid road");
        _environmentExiter.ExitCode.Should().Be(1);
    }
}