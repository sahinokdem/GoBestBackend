namespace GoBest.Stations
{
    using GoBest.Models;
    using GoBest.Routes.DTO;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class StationService
    {
        private readonly StationRepository _stationRepository;

        public StationService(StationRepository stationRepository)
        {
            _stationRepository = stationRepository;
        }

    }
    
}