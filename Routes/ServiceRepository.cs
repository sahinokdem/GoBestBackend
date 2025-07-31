using GoBest.Companies;
using GoBest.Data;
using GoBest.Models;
using GoBest.Routes.DTO;
using GoBest.Stations;

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
    }
}