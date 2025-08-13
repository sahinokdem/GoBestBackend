using GoBest.Companies;
using GoBest.Models;

namespace GoBest.Itinaries;

/// <summary> LINQ tarafında kullanılacak projeksiyon uzantısı. </summary>
public static class ItineraryMappingExtensions
{
    public static IQueryable<SearchResponse> ToSearchDto(
        this IQueryable<Itinerary> query)
    {
        return query.Select(i => new SearchResponse
        {
            ItineraryId = i.Id,
            TotalLegs = i.TotalLegs ?? 0,
            TotalDuration = i.TotalDuration ?? TimeSpan.Zero,
            Summary = $"{i.TotalLegs} legs · " +
                            $"{(int)(i.TotalDuration ?? TimeSpan.Zero).TotalHours} h " +
                            $"{(i.TotalDuration ?? TimeSpan.Zero).Minutes:D2} m · " +
                            $"€{i.TotalPrice ?? 0:N0}",
            Legs = i.ItineraryLegs
                   .OrderBy(l => l.LegOrder)
                   .Select(l => new LegDto
                   {
                       Order = l.LegOrder,
                       ServiceId = l.ServiceId!.Value,
                       ServiceCode = l.Service!.ServiceCode,
                       CompanyName = l.Service.Company!.Name,
                       OriginCity = l.Service.OriginStation!.City!.Name,
                       DestCity = l.Service.DestStation!.City!.Name,
                       Departure = l.Service.DepartureTime,
                       Arrival = l.Service.ArrivalTime,
                       Price = l.Price ?? l.Service.BasePrice,
                       SeatTypeName = l.SeatType.Name,
                       CompanyMode = l.Service.Company.Mode.ToString(), // ★ yeni
                       OriginStation = l.Service.OriginStation.Name,
                       DestStation = l.Service.DestStation.Name
                   })
                   .ToArray()
        });
    }
    
    public static CompanyMode? ToCompanyMode(this TravelMode mode) => mode switch
    {
        TravelMode.Bus    => CompanyMode.Bus,
        TravelMode.Train  => CompanyMode.Train,
        TravelMode.Flight => CompanyMode.Air,
        _                 => null               // All
    };

}
