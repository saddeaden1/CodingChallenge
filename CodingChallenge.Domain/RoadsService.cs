using LanguageExt;
using Refit;

namespace CodingChallenge.Domain;

public class RoadsService : IRoadsService
{
    private readonly ITflApi _api;

    public RoadsService(ITflApi api)
    {
        _api = api;
    }

    public async Task<Either<RoadCorridor, Error>> GetRoad(string roadName)
    {
        try
        {
            var result = await _api.GetRoad(roadName);

            return  new RoadCorridor
            {
                StatusSeverityDescription = ((RoadCorridorResponses)result).Roads.SingleOrDefault()!.StatusSeverityDescription,
                DisplayName = ((RoadCorridorResponses)result).Roads.SingleOrDefault()!.DisplayName,
                StatusSeverity = ((RoadCorridorResponses)result).Roads.SingleOrDefault()!.StatusSeverity
            };

        }
        catch (ApiException ex)
        {
            var result = await ex.GetContentAsAsync<ErrorResponse>();
            return new Error
            {
                ExceptionType = result.ExceptionType,
                Message = result.Message,
                HttpStatusCode = result.HttpStatusCode
            };
        }
        catch (HttpRequestException ex)
        {
            return new Error
            {
                Message = ex.Message
            };
        }
        catch (NotImplementedException)
        {
            return new Error
            {
                Message = "Unknown type returned from TFL Api"
            };
        }
    }

    public async Task<Either<bool,Error>> ValidateRoadName(string roadName)
    {
        try
        {
            var result = await _api.GetRoads();
            return ((RoadCorridorResponses)result).Roads.Select(x => x.Id.ToLower()).Contains(roadName.ToLower());
        }
        catch (ApiException ex)
        {
            var result = await ex.GetContentAsAsync<ErrorResponse>();

            return new Error
            {
                ExceptionType = result.ExceptionType,
                Message = result.Message,
                HttpStatusCode = result.HttpStatusCode
            };
        }catch (HttpRequestException ex)
        {
            return new Error
            {
                Message = ex.Message
            };
        }
        catch (NotImplementedException)
        {
            return new Error
            {
                Message = "Unknown type returned from TFL Api"
            };
        }
    }
}