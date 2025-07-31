using System.Text.Json.Serialization;

namespace GoBest.Routes.DTO
{
    public class ServiceAPIDto
    {
        public int Service_Id { get; set; }
        public string Service_Code { get; set; }
        public string Mode { get; set; }
        public CompanyAPIDto Company { get; set; }
        public StationAPIDto Origin { get; set; }
        public StationAPIDto Destination { get; set; }
        public decimal Base_Price { get; set; }
        public string Currency { get; set; }
        public List<SeatTypeAPIDto> Seat_Types { get; set; }
    }

    public class CompanyAPIDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Iata_Code { get; set; }
        public string Country_Code { get; set; }
    }

    public class StationAPIDto
    {
        public int Station_Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public CityAPIDto City { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [JsonPropertyName("departure_time")]
        public DateTime? DepartureTime { get; set; }

        [JsonPropertyName("arrival_time")]
        public DateTime? ArrivalTime { get; set; }
    }

    public class CityAPIDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country_Code { get; set; }
    }

    public class SeatTypeAPIDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Available { get; set; }
    }

}
