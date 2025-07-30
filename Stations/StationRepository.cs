using GoBest.Data;
using GoBest.Models;

namespace GoBest.Stations
{

    public class StationRepository
    {
        private readonly MyDbContext _db;

        public StationRepository(MyDbContext db)
        {
            _db = db;
        }

        public async Task<Station?> GetStationByIdAsync(int stationId)
        {
            return await _db.Stations.FindAsync(stationId);
        }

        public async Task SaveStationAsync(Station station)
        {
            if (station == null) throw new ArgumentNullException(nameof(station));
            _db.Stations.Add(station);
            await _db.SaveChangesAsync();
        }
    
    }
}