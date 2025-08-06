namespace GoBest.Routes.DTO;

public sealed record ServiceResponse
{
    public long Id { get; init; }
    public string ServiceCode { get; init; }
    public string OriginCity { get; init; }
    public string DestCity { get; init; }
    public string CompanyName { get; init; }
}
