namespace GoBest.Stations;

public sealed record StationResponse
{
    public long Id { get; set; }

    public long? CityId { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? StationType { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}