using LanguageExt;

namespace CodingChallenge.Domain;

public interface IRoadsService
{
    Task<Either<RoadCorridor, Error>> GetRoad(string roadName);

    Task<Either<bool, Error>> ValidateRoadName(string roadName);
}