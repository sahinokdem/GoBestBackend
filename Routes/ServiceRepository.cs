using GoBest.Companies;
using GoBest.Data;
using GoBest.Models;
using GoBest.Routes.DTO;

namespace GoBest.Routes
{

    public class ServiceRepository
    {
        private readonly MyDbContext _db;
        private readonly StationRepository _stationRepository;
        private readonly CompanyRepository _companyRepository;

        public ServiceRepository(MyDbContext db, StationRepository stationRepository, CompanyRepository companyRepository)
        {
            _db = db;
            _stationRepository = stationRepository;
            _companyRepository = companyRepository;
        }

        public async Task SaveFromApi(ServiceAPIDto serviceDto)
        {
            // Convert DTO to Service entity

            var Company = await _companyRepository.GetCompanyByIdAsync(serviceDto.Company.Id);


            var service = new Service
            {
                ServiceCode = serviceDto.Service_Code,
                DepartureTime = serviceDto.Origin.Time,
                ArrivalTime = serviceDto.Destination.Time,
                BasePrice = serviceDto.Base_Price,
                Sold = false,
                SalesCount = 0,
                //CompanyId = serviceDto.Company.Id,
                //OriginStationId = serviceDto.Origin.Station_Id,
                //DestStationId = serviceDto.Destination.Station_Id,
            };

            // Add to context and save
            _db.Services.Add(service);
            await _db.SaveChangesAsync();
        }
    }
}