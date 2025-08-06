
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
                Mode = ToCompanyMode(apiDto),
                CountryCode = apiDto.Company.Country_Code,
                IataCode = apiDto.Company.Iata_Code
            };
        }

        public static CompanyMode ToCompanyMode(ServiceAPIDto apiDto)
        {
            return apiDto.Mode.ToLower() switch
            {
                "bus" => CompanyMode.Bus,
                "train" => CompanyMode.Train,
                "flight" or "air" => CompanyMode.Air,
                _ => throw new ArgumentException($"Invalid mode: {apiDto.Mode}")
            };
        }

        public static CompanyResponse ToResponse(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }
            return new CompanyResponse
            {
                Id = company.Id,
                Name = company.Name,
                Mode = company.Mode.ToString(),
                CountryCode = company.CountryCode,
                IataCode = company.IataCode
            };
        }

    }
}