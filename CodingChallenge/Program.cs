using CodingChallenge.Domain;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using Autofac;
using LanguageExt;

namespace CodingChallenge;

public class Program
{
    private static IContainer _container;
    private static IRoadsService _roadService;
    private static IEnvironmentExiter _exiter;

    public Program(IContainer container)
    {
        _container = container;
    }

    private static void BuildContainer()
    {
        var builder = new ContainerBuilder();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json")
            .Build();

        builder.Register(_ => RestService.For<ITflApi>("http://api.tfl.gov.uk",
            new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new RoadCorridorConverter() }
                }),

                //Adds the key to the request as a header 
                HttpMessageHandlerFactory = () => new AuthHeaderHandler(configuration["Key"])
            })).As<ITflApi>();

        builder.RegisterType<RoadsService>().As<IRoadsService>();
        builder.RegisterType<EnvironmentExiter>().As<IEnvironmentExiter>();

        _container = builder.Build();
    }

    public static async Task Main(string[] args)
    {
        //Conditional creation of the container added to allow
        //the tests to setup the DI container in the constructor and call only this method in the test

        if (_container == null)
        {
            BuildContainer();
        }

        _roadService = _container!.Resolve<IRoadsService>();

        //put the environment exit code in a class to allow me to inject a test version in my tests so
        //the test doesn't exit when testing this method
        _exiter = _container!.Resolve<IEnvironmentExiter>();

        await ValidateArgs(args);

        var road = args!.First();

        var roadNameValidity = await _roadService.ValidateRoadName(road);

        roadNameValidity.Match(error =>
            {

                HandleError(error);

            }, isValid =>
                {
                    if (!isValid)
                    { 
                        Console.WriteLine($"{road} is not a valid road");
                        _exiter.Exit(1);
                    }
                }
            );

        var roadStatusResponse = await _roadService.GetRoad(road);

        roadStatusResponse.Match(errorResponse =>
            {
                HandleError(errorResponse);

            }, roadCorridorResponse =>
            {
                Console.WriteLine($"The status of the road {roadCorridorResponse.DisplayName} is as follows");
                Console.WriteLine($"Road Status is {roadCorridorResponse.StatusSeverity}");
                Console.WriteLine($"Road Status Description is {roadCorridorResponse.StatusSeverityDescription}");
                _exiter.Exit(0);
            }
        );
    }

    private static async Task ValidateArgs(string[] args)
    {
        if (args == null)
        {
            Console.WriteLine("No road inputted please enter a road");
            _exiter.Exit(1);
        }

        var validator = new ArgsValidator();

        var validationResult = await validator.ValidateAsync(args);

        if (!validationResult.IsValid)
        {
            Console.WriteLine(validationResult.ToString());
            _exiter.Exit(1);
        }
    }

    private static void HandleError(Error error)
    {
        Console.WriteLine($"An error has occurred with the message: {error.Message}");

        if (error.ExceptionType != null)
        {
            Console.WriteLine($"ExceptionType: {error.ExceptionType}");
        }

        if (!error.HttpStatusCode.IsDefault())
        {
            Console.WriteLine($"Http Status code: {error.HttpStatusCode}");
        }

        _exiter.Exit(1);
    }
}