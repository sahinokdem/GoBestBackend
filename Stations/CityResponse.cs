namespace GoBest.Stations
{
    public sealed record CityResponse
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string CountryCode { get; init; }
    }
}