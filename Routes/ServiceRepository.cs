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

        public async Task SaveAsync(Service service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            _db.Services.Add(service);
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
                            s.DepartureTime <  endUtc);

            if (rq.Mode != TravelMode.All)
            {
                var mode = (CompanyMode)rq.Mode;
                query = query.Where(s =>
                    s.ServiceSeatInventories.Any(inv => inv.SeatType!.Mode == mode));
            }

            return await query.AsNoTrackingWithIdentityResolution().ToListAsync(ct);
        }



    }
}