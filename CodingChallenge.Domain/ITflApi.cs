using Refit;

namespace CodingChallenge.Domain;

public interface ITflApi
{
    [Get("/road/{roadName}")]
    Task<TflApiResponse> GetRoad(string roadName);

    [Get("/road")]
    Task<TflApiResponse> GetRoads();
}