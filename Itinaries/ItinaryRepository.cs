using GoBest.Data;
using GoBest.Itinaries;
using GoBest.Itineraries;
using GoBest.Models;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Itinaries;

public class ItineraryRepository
{
    private readonly MyDbContext _db;

    public ItineraryRepository(MyDbContext db) => _db = db;

    public async Task<IReadOnlyList<SearchResponse>> GetItinerariesAsync(
        SearchRequest rq, CancellationToken ct)
    {
        var startUtc = DateTime.SpecifyKind(
                        rq.TravelDate.ToDateTime(TimeOnly.MinValue),
                        DateTimeKind.Utc);
        var endUtc   = startUtc.AddDays(1);
        var compMode = rq.Mode.ToCompanyMode();          // önceki düzeltme

        return await _db.Itineraries
            .AsNoTracking()
            .Include(i => i.ItineraryLegs)
                .ThenInclude(l => l.Service)
                    .ThenInclude(s => s.Company)
            .Include(i => i.ItineraryLegs)
                .ThenInclude(l => l.Service)
                    .ThenInclude(s => s.OriginStation).ThenInclude(st => st.City)
            .Include(i => i.ItineraryLegs)
                .ThenInclude(l => l.Service)
                    .ThenInclude(s => s.DestStation).ThenInclude(st => st.City)
            .Include(i => i.ItineraryLegs)                       // 🔸 koltuk adedi için
                .ThenInclude(l => l.Service)
                    .ThenInclude(s => s.ServiceSeatInventories)
            .Where(i =>
                /* şehir ve tarih filtresi */
                i.OriginCityId == rq.OriginCityId &&
                i.DestCityId   == rq.DestCityId   &&
                i.SearchTime  >= startUtc         &&
                i.SearchTime  <  endUtc           &&

                /* mode (Bus/Train/Flight) filtresi  */
                (compMode == null ||
                    i.ItineraryLegs.Any(l => l.Service.Company!.Mode == compMode)) &&

                /* 👇 yeni: her bacakta yeterli koltuk */
                (rq.Passengers <= 0 ||                     // 0 → kapasite kontrolü yok
                    i.ItineraryLegs.All(l =>
                        l.Service.ServiceSeatInventories
                        .Any(inv => inv.Available >= rq.Passengers)))
            )
            .OrderBy(i => i.TotalPrice)
            .Take(20)
            .ToSearchDto()
            .ToListAsync(ct);
    }




    public async Task<IReadOnlyList<SearchResponse>> SaveAsync(
    IEnumerable<ItineraryAggregate> aggregates,
    SearchRequest rq,
    CancellationToken ct)
    {
        /* --- 0) Benzersiz aday listesi ------------------------------------ */
        string Key(ItineraryAggregate ag) =>
            string.Join('|', ag.Legs.Select(l => $"{l.Service.Id}:{l.SeatType.Id}"));

        var aggList = aggregates.DistinctBy(Key).ToArray();
        var keysToInsert = aggList.Select(Key).ToHashSet();

        /* --- 1) Aynı gün + origin/dest'teki mevcut itineraries ------------- */
        var startUtc = DateTime.SpecifyKind(
                        rq.TravelDate.ToDateTime(TimeOnly.MinValue),
                        DateTimeKind.Utc);
        var endUtc = startUtc.AddDays(1);

        var rawLegs = await _db.ItineraryLegs
            .AsNoTracking()
            .Where(il => il.Itinerary!.OriginCityId == rq.OriginCityId &&
                        il.Itinerary.DestCityId    == rq.DestCityId   &&
                        il.Itinerary.SearchTime    >= startUtc        &&
                        il.Itinerary.SearchTime    <  endUtc)
            .Select(il => new {                      // ← EF bunu çevirebilir
                il.ItineraryId,
                il.LegOrder,
                il.ServiceId,
                il.SeatTypeId
            })
            .ToListAsync(ct);

        /* Anahtarı bellekte üret */
        var existingMap = rawLegs
            .GroupBy(x => x.ItineraryId)
            .ToDictionary(
                g => string.Join('|',
                    g.OrderBy(x => x.LegOrder)
                    .Select(x => $"{x.ServiceId}:{x.SeatTypeId}")),
                g => g.Key);

        /* --- 2) Sadece yeni anahtarları ekle ------------------------------- */
        // … rawLegs sorgusu değişmedi …

        var map = new Dictionary<ItineraryAggregate, Itinerary>();

        foreach (var ag in aggList)
        {
            var k = Key(ag);
            if (existingMap.TryGetValue(k, out var existingId))
            {
                // DB'de zaten var
                var dummy = new Itinerary { Id = (long)existingId };
                map[ag] = dummy;
                continue;
            }

            var travelDayUtc = DateTime.SpecifyKind(
                rq.TravelDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);

            var (itin, legs) = ag.ToEntities(
                rq.OriginCityId,
                rq.DestCityId,
                travelDayUtc,
                rq.Passengers);

            _db.Itineraries.Add(itin);
            _db.ItineraryLegs.AddRange(legs);
            map[ag] = itin;                    // nesne referansı
        }

        if (_db.ChangeTracker.HasChanges())
            await _db.SaveChangesAsync(ct);

        return aggList.Select(ag => ag.ToDto() with { ItineraryId = map[ag].Id })
                    .ToList();

    }





}