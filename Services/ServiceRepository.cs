using GoBest.Data;
using GoBest.Models;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Services
{
    public class ServiceRepository
    {
        private readonly MyDbContext _db;

        public ServiceRepository(MyDbContext db)
        {
            _db = db;
        }
        
        public async Task<Service> GetServiceByIdAsync(long id)
        {
            return await _db.Services
                .Include(s => s.ServiceSeatInventories)
                .Include(s => s.Company)
                .Include(s => s.OriginStation)
                .Include(s => s.DestStation)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        
        public async Task<Service> GetServiceByCodeAsync(string serviceCode)
        {
            return await _db.Services
                .Include(s => s.ServiceSeatInventories)
                .Include(s => s.Company)
                .Include(s => s.OriginStation)
                .Include(s => s.DestStation)
                .FirstOrDefaultAsync(s => s.ServiceCode == serviceCode);
        }
        
        public async Task<List<Service>> FindDirectServicesAsync(long originStationId, long destStationId, DateTime departureDate)
        {
            var startOfDay = departureDate.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
            
            return await _db.Services
                .Where(s => s.OriginStationId == originStationId && 
                           s.DestStationId == destStationId && 
                           s.DepartureTime >= startOfDay && 
                           s.DepartureTime <= endOfDay)
                .Include(s => s.ServiceSeatInventories)
                .Include(s => s.Company)
                .Include(s => s.OriginStation)
                .Include(s => s.DestStation)
                .ToListAsync();
        }
        
        public async Task<List<Service>> GetAllServicesForRouteCalculationAsync()
        {
            return await _db.Services
                .Include(s => s.OriginStation)
                .Include(s => s.DestStation)
                .ToListAsync();
        }
        
        public async Task SaveServiceAsync(Service service)
        {
            _db.Services.Add(service);
            await _db.SaveChangesAsync();
        }
        
        public async Task UpdateServiceAsync(Service service)
        {
            _db.Services.Update(service);
            await _db.SaveChangesAsync();
        }
    }
}
