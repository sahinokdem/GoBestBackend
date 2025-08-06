namespace GoBest.Routes.DTO;

public sealed record UpdateServiceRequest
{

    public DateTime DepartureTime { get; set; }

    public DateTime ArrivalTime { get; set; }
}