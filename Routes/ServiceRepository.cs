using GoBest.Companies;
using GoBest.Data;
using GoBest.Models;
using GoBest.Routes.DTO;
using GoBest.Stations;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Routes
{

    public class ServiceRepository
    {
        private readonly MyDbContext _db;
        //private readonly StationRepository _stationRepository;
        //private readonly CompanyRepository _companyRepository;

        public ServiceRepository(MyDbContext db)
        {
            _db = db;
            //_stationRepository = stationRepository;
            //_companyRepository = companyRepository;
        }

        public async Task<Service?> GetServiceByIdAsync(long serviceId)
        {
            return await _db.Services
                .Include(s => s.OriginStation).ThenInclude(st => st.City)
                .Include(s => s.DestStation).ThenInclude(st => st.City)
                .Include(s => s.Company)
                .FirstOrDefaultAsync(s => s.Id == serviceId);
        }

        public async Task SaveAsync(Service service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            var existingService = await _db.Services
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == service.Id);

            if (existingService == null)
            {
                _db.Services.Add(service);
            }
            else
            {
                // 1️⃣ Bu servisi içeren itinerary'leri bul
                var relatedItineraries = await _db.ItineraryLegs
                    .Where(il => il.ServiceId == service.Id && il.ItineraryId != null)
                    .Select(il => il.ItineraryId!.Value)
                    .Distinct()
                    .ToListAsync();

                // 2️⃣ İlgili leg ve itinerary'leri çek
                var legsToRemove = await _db.ItineraryLegs
                    .Where(il => relatedItineraries.Contains(il.ItineraryId!.Value))
                    .ToListAsync();

                var itinerariesToRemove = await _db.Itineraries
                    .Where(i => relatedItineraries.Contains(i.Id))
                    .ToListAsync();

                // 3️⃣ Silme işlemleri
                _db.ItineraryLegs.RemoveRange(legsToRemove);
                _db.Itineraries.RemoveRange(itinerariesToRemove);

                // 4️⃣ Güncelleme
                _db.Services.Update(service);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Service>> GetDailyServicesAsync(
            SearchRequest rq, CancellationToken ct)
        {
            var startUtc = DateTime.SpecifyKind(
                            rq.TravelDate.ToDateTime(TimeOnly.MinValue),
                            DateTimeKind.Utc);
            var endUtc = startUtc.AddDays(1);

            var query = _db.Services
                .Include(s => s.Company)                               // ★ EKLEDİK
                .Include(s => s.OriginStation).ThenInclude(st => st.City)
                .Include(s => s.DestStation).ThenInclude(st => st.City)
                .Include(s => s.ServiceSeatInventories)
                    .ThenInclude(inv => inv.SeatType)                  // (mode filtresi için)
                .Where(s => s.DepartureTime >= startUtc &&
                            s.DepartureTime < endUtc);

            if (rq.Mode != TravelMode.All)
            {
                var mode = (CompanyMode)rq.Mode;
                query = query.Where(s =>
                    s.ServiceSeatInventories.Any(inv => inv.SeatType!.Mode == mode));
            }

            return await query.AsNoTrackingWithIdentityResolution().ToListAsync(ct);
        }

        internal async Task<IReadOnlyList<Service>> GetAllServicesAsync()
        {
            return await _db.Services
                .Include(s => s.OriginStation).ThenInclude(st => st.City)
                .Include(s => s.DestStation).ThenInclude(st => st.City)
                .Include(s => s.Company)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Service>> GetServicesByCompanyIdAsync(long companyId)
        {
            return await _db.Services
                .Include(s => s.OriginStation).ThenInclude(st => st.City)
                .Include(s => s.DestStation).ThenInclude(st => st.City)
                .Include(s => s.Company)
                .Where(s => s.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync();
        }
        
        public async Task<Service?> GetServiceByCompanyIdAsync(long companyId, long serviceId)
        {
            return await _db.Services
                .Include(s => s.OriginStation!).ThenInclude(st => st.City!)
                .Include(s => s.DestStation!).ThenInclude(st => st.City!)
                .Include(s => s.Company!)
                .FirstOrDefaultAsync(s => s.CompanyId == companyId && s.Id == serviceId);
        }
    }
}