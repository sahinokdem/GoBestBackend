public sealed record LegDto
{
    public short    Order          { get; init; }       // 1, 2, …
    public long     ServiceId      { get; init; }
    public string   ServiceCode    { get; init; } = string.Empty;
    public string   CompanyName    { get; init; } = string.Empty;
    public string   CompanyMode    { get; init; } = string.Empty; // ★ yeni
    public string   OriginCity     { get; init; } = string.Empty;
    public string   DestCity       { get; init; } = string.Empty;
    public string   OriginStation  { get; init; } = string.Empty;
    public string   DestStation    { get; init; } = string.Empty;
    public DateTime Departure      { get; init; }
    public DateTime Arrival        { get; init; }
    public string   SeatTypeName   { get; init; } = string.Empty;   // ★ yeni
    public decimal  Price          { get; init; }
}