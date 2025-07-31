
using GoBest.Models;
using GoBest.Routes.DTO;

namespace GoBest.Companies
{
    public class CompanyMapper
    {
        public static Company ToCompany(ServiceAPIDto apiDto)
        {
            return new Company
            {
                Name = apiDto.Company.Name,
                Mode = apiDto.Mode.ToLower() switch
                {
                    "bus" => CompanyMode.Bus,
                    "train" => CompanyMode.Train,
                    "flight" or "air" => CompanyMode.Air,
                    _ => throw new ArgumentException($"Invalid mode: {apiDto.Mode}")
                },
                CountryCode = apiDto.Company.Country_Code,
                IataCode = apiDto.Company.Iata_Code
            };
        }

    }
}