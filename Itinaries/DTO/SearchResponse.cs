public sealed record SearchResponse
{
    public long   ItineraryId      { get; init; }
    public string Summary          { get; init; } = string.Empty; // “2 legs · 11 h 20 m · €132”
    public short  TotalLegs        { get; init; }
    public TimeSpan TotalDuration  { get; init; }
    public decimal TotalPrice      { get; init; }
    public required IReadOnlyList<LegDto> Legs { get; init; }
}