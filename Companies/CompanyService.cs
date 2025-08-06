using GoBest.Models;
using GoBest.Routes.DTO;

namespace GoBest.Companies
{
    public class CompanyService
    {
        private readonly CompanyRepository _companyRepository;
        private readonly CompanyMaintainerService _companyMaintainerService;


        public CompanyService(CompanyRepository companyRepository,
         CompanyMaintainerService companyMaintainerService)
        {
            _companyRepository = companyRepository;
            _companyMaintainerService = companyMaintainerService;
        }

        public async Task<long> SaveCompanyFromApi(ServiceAPIDto apiDto)
        {
            if (apiDto == null)
            {
                throw new ArgumentNullException(nameof(apiDto));
            }

            Company company = CompanyMapper.ToCompany(apiDto);
            return await _companyRepository.SaveAndGetCompanyId(company);
        }

        public async Task<List<CompanyResponse>> GetAllCompaniesAsync()
        {
            var companies = await _companyRepository.GetAllCompaniesAsync();
            var companyResponses = new List<CompanyResponse>();
            foreach (var company in companies)
            {
                companyResponses.Add(CompanyMapper.ToResponse(company));
            }
            return companyResponses;
        }

        public async Task<bool> UpdateCompanyAsync(long userId, long companyId, UpdateCompanyRequest dto)
        {
            if (await _companyMaintainerService.IsCompanyMaintainerAsync(userId)
                && (!await _companyMaintainerService.IsCompanyMaintainerOfCompanyAsync(companyId, userId)))
            {
                throw new UnauthorizedAccessException("User is not authorized to update this company.");
            }
            var company = await _companyRepository.GetCompanyByIdAsync(companyId);
            if (company is null) return false;

            company.Name = dto.Name.Trim();
            company.CountryCode = dto.CountryCode.ToUpperInvariant();
            company.IataCode = dto.IataCode?.ToUpperInvariant();

            await _companyRepository.SaveCompanyAsync(company);
            return true;
        }
    }
}