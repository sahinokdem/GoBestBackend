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
        // 1) Önce cache/DB
        var cached = await _itRepo.GetItinerariesAsync(rq, ct);
        if (cached.Any()) return cached;

        // 2) Günün servislerini topla (mode filtresi burada var)
        var candidates = await _svcRepo.GetDailyServicesAsync(rq, ct);

        // 3) Tüm seat type kombinasyonlarını üret (pax filtresi YOK)
        var aggregates = BuildItineraries(rq, candidates).ToArray();

        // 4) Hepsini DB'ye kaydet (pax bakmadan)
        await _itRepo.SaveAsync(aggregates, rq, ct);   // dönüşü artık kullanmıyoruz

        // 5) Kullanıcıya dönerken DB’den pax + seat type bazlı filtre ile oku
        //    (GetItinerariesAsync içinde zaten: inv.SeatTypeId == l.SeatTypeId && inv.Available >= rq.Passengers)
        var fresh = await _itRepo.GetItinerariesAsync(rq, ct);
        return fresh;
    }



    /* ---------- path-finding ---------- */
    private IEnumerable<ItineraryAggregate> BuildItineraries(
        SearchRequest rq, IReadOnlyList<Service> services)
    {
        var graph = services
            .GroupBy(s => s.OriginStation!.CityId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var q = new Queue<Path>();
        q.Enqueue(new Path(rq.OriginCityId, null, new()));

        const int MAX_DAYS_BETWEEN = 1;
        TimeSpan MAX_HORIZON = TimeSpan.FromDays(MAX_DAYS_BETWEEN);

        while (q.Count > 0)
        {
            var current = q.Dequeue();
            if (current.Legs.Count > MAX_TRANSFERS) continue;

            foreach (var svc in graph.GetValueOrDefault(current.City)
                                    ?? Enumerable.Empty<Service>())
            {
                bool firstLeg = current.Legs.Count == 0;
                if (firstLeg && !SameDate(svc.DepartureTime, rq.TravelDate))
                    continue;

                if (current.LastArrival.HasValue &&
                    svc.DepartureTime < current.LastArrival + MIN_BUFFER)
                    continue;

                if (svc.DepartureTime - current.Legs.FirstOrDefault()?.Service.DepartureTime > MAX_HORIZON)
                    continue;

                // ⬇️ Tüm seat type’ları dene (istersen Available > 0 ile daralt)
                foreach (var inv in svc.ServiceSeatInventories
                                    //.Where(x => x.Available > 0) // istersen aç
                                    )
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
