using GoBest.Models;
using GoBest.Itineraries.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoBest.Itineraries;

public sealed class ItineraryAggregate
{
    /* ---------- ctor helpers ---------- */
    public static ItineraryAggregate FromSingleLeg(TravelLeg leg) =>
        new([leg]);

    public static ItineraryAggregate FromPath(IReadOnlyList<TravelLeg> legs) =>
        new(legs);

    private ItineraryAggregate(IReadOnlyList<TravelLeg> legs)
    {
        Legs          = legs;
        TotalLegs     = (short)legs.Count;
        TotalPrice    = legs.Sum(l => l.Price);
        TotalDuration = legs.Last().Service.ArrivalTime
                      - legs.First().Service.DepartureTime;
    }

    /* ---------- public surface ---------- */
    public IReadOnlyList<TravelLeg> Legs   { get; }
    public decimal   TotalPrice    { get; }
    public TimeSpan  TotalDuration { get; }
    public short     TotalLegs     { get; }

    /* ---------- EF materialisation ---------- */
    public (Itinerary itin, IEnumerable<ItineraryLeg> legs) ToEntities(
        long originCityId, long destCityId, DateTime searchUtc, int pax)
    {
        var itin = new Itinerary
        {
            OriginCityId  = originCityId,
            DestCityId    = destCityId,
            SearchTime    = searchUtc,
            TotalPrice    = TotalPrice * pax,
            TotalDuration = TotalDuration,
            TotalLegs     = TotalLegs
        };

        var legEntities = Legs.Select((leg, idx) => new ItineraryLeg
        {
            Itinerary  = itin,
            LegOrder   = (short)(idx + 1),
            ServiceId  = leg.Service.Id,
            SeatTypeId = leg.SeatType.Id,
            Price      = leg.Price
        });

        return (itin, legEntities);
    }

    /* ---------- DTO mapping ---------- */
    public SearchResponse ToDto()
    {
        var dtoLegs = Legs.Select((leg, idx) => new LegDto
        {
            Order = (short)(idx + 1),
            ServiceId = leg.Service.Id,
            ServiceCode = leg.Service.ServiceCode,
            CompanyName = leg.Service.Company!.Name,
            OriginCity = leg.Service.OriginStation!.City!.Name,
            DestCity = leg.Service.DestStation!.City!.Name,
            Departure = leg.Service.DepartureTime,
            Arrival = leg.Service.ArrivalTime,
            SeatTypeName = leg.SeatType.Name,
            Price = leg.Price,
            CompanyMode = leg.Service.Company!.Mode.ToString(),
            OriginStation = leg.Service.OriginStation!.Name,
            DestStation = leg.Service.DestStation!.Name
        }).ToArray();

        return new SearchResponse
        {
            ItineraryId   = 0,
            TotalLegs     = TotalLegs,
            TotalDuration = TotalDuration,
            Legs          = dtoLegs,
            Summary       = $"{TotalLegs} legs · {Format(TotalDuration)} · €{TotalPrice:N0}"
        };
    }

    private static string Format(TimeSpan ts)
        => $"{(int)ts.TotalHours} h {ts.Minutes:D2} m";
}
