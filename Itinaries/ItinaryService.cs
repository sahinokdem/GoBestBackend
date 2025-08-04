using GoBest.Itineraries;
using GoBest.Itineraries.DTO;
using GoBest.Models;
using GoBest.Routes;

namespace GoBest.Itinaries;


public sealed class ItineraryService
{

    private sealed record Path(
        long City,
        DateTime? LastArrival,
        List<TravelLeg> Legs)
    {
        public Path Extend(TravelLeg next) =>
            new((long)next.Service.DestStation!.CityId, next.Service.ArrivalTime,
                new List<TravelLeg>(Legs) { next });
    }


    private readonly ItineraryRepository _itRepo;
    private readonly ServiceRepository   _svcRepo;
    private const int MAX_TRANSFERS = 2;          // at most 3 legs total
    private static readonly TimeSpan MIN_BUFFER = TimeSpan.FromMinutes(20);
    private readonly ILogger<ItineraryService> _logger;

    public ItineraryService(ItineraryRepository itRepo, ServiceRepository svcRepo, ILogger<ItineraryService> logger)
    {
        _itRepo = itRepo; _svcRepo = svcRepo; _logger = logger;
    }

    public async Task<IReadOnlyList<SearchResponse>> SearchAsync(SearchRequest rq, CancellationToken ct)
    {
        // 1️⃣ cached results – today’s DB first
        var cached = await _itRepo.GetItinerariesAsync(rq, ct);
        if (cached.Any()) return cached;

        // 2️⃣ gather candidate services for the requested day & mode
        var candidates = await _svcRepo.GetDailyServicesAsync(rq, ct);

        // 3️⃣ build best itineraries in-memory
        var itineraries = BuildItineraries(rq, candidates).ToArray();

        // 4️⃣ persist and map to DTOs
        var persisted = await _itRepo.SaveAsync(itineraries, rq, ct);
        return persisted;
    }

    /* ---------- path-finding ---------- */
 /* ---------- path-finding ---------- */
    private IEnumerable<ItineraryAggregate> BuildItineraries(
        SearchRequest rq, IReadOnlyList<Service> services)
    {
        /* Şehir → servis listesi grafiği */
        var graph = services
            .GroupBy(s => s.OriginStation!.CityId)
            .ToDictionary(g => g.Key, g => g.ToList());

        /* BFS kuyruğu */
        var q = new Queue<Path>();
        q.Enqueue(new Path(rq.OriginCityId, null, new()));

        const int MAX_DAYS_BETWEEN = 1;                    // ★ en fazla 1 gün
        TimeSpan MAX_HORIZON = TimeSpan.FromDays(MAX_DAYS_BETWEEN);

        while (q.Count > 0)
        {
            var current = q.Dequeue();
            if (current.Legs.Count > MAX_TRANSFERS) continue;

            foreach (var svc in graph.GetValueOrDefault(current.City)
                                    ?? Enumerable.Empty<Service>())
            {
                /* 1) İlk bacak travelDate günü olmalı  */
                bool firstLeg = current.Legs.Count == 0;
                if (firstLeg && !SameDate(svc.DepartureTime, rq.TravelDate))
                    continue;

                /* 2) Aktarmalarda minimum bekleme 20 dk */
                if (current.LastArrival.HasValue &&
                    svc.DepartureTime < current.LastArrival + MIN_BUFFER)
                    continue;

                /* 3) Tüm yolculuk en fazla 1 takvim günü sürsün */
                if (svc.DepartureTime - current.Legs
                        .FirstOrDefault()?.Service.DepartureTime > MAX_HORIZON)
                    continue;

                /* 4) Koltuk kapasitesi */
                foreach (var inv in svc.ServiceSeatInventories
                                    .Where(x => x.Available >= rq.Passengers))
                {
                    var nextPath = current.Extend(new TravelLeg(svc, inv));

                    if (svc.DestStation!.CityId == rq.DestCityId)
                        yield return ItineraryAggregate.FromPath(nextPath.Legs);
                    else
                        q.Enqueue(nextPath);
                }
            }
        }
    }

    
    private static bool SameDate(DateTime dt, DateOnly target) =>
        DateOnly.FromDateTime(dt) == target;
}
