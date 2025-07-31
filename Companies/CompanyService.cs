using GoBest.Models;
using GoBest.Routes.DTO;

namespace GoBest.Companies
{
    public class CompanyService
    {
        private readonly CompanyRepository _companyRepository;


        public CompanyService(CompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
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
    }
}