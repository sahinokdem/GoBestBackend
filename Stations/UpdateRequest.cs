using GoBest.Stations;

public sealed record UpdateStationRequest
{
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
}

public sealed record UpdateCityRequest
{
    public string Name { get; init; } = null!;
    public string CountryCode { get; init; } = null!;
}

