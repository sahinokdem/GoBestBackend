using GoBest.Models;
using GoBest.Routes.DTO;

namespace GoBest.Stations
{
    
    public class StationMapper
    {
        public static Station ToStation(ServiceAPIDto apiDto)
        {
            if (apiDto == null)
            {
                throw new ArgumentNullException(nameof(apiDto));
            }

            return null;
        }
    }
}