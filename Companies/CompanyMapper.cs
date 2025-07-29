
using GoBest.Models;
using GoBest.Routes.DTO;

namespace GoBest.Companies
{
    public class CompanyMapper
    {
        public static Company toCompany(ServiceAPIDto apiDto)
        {
            return new Company
            {
                Name = apiDto.Company.Name,
                Mode = Enum.Parse<CompanyMode>(apiDto.Mode, true),
                CountryCode = apiDto.Company.Country_Code,
                IataCode = apiDto.Company.Iata_Code
            };
        }
    }
}