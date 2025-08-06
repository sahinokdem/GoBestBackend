public sealed record UpdateCompanyRequest
{
    public string Name { get; init; } = null!;
    public string CountryCode { get; init; } = null!;
    public string? IataCode { get; init; }
}

public sealed record CompanyResponse
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Mode { get; init; }
    public string CountryCode { get; init; } = null!;
    public string? IataCode { get; init; }
}

