namespace GoBest.Stations
{
    using GoBest.Models;
    using GoBest.Routes.DTO;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CityMapper
    {
        public static City ToCity(CityAPIDto cityDto)
        {
            if (cityDto == null)
            {
                throw new ArgumentNullException(nameof(cityDto));
            }
            return new City
            {
                Name = cityDto.Name,
                CountryCode = cityDto.Country_Code,
            };
        }
    }
}