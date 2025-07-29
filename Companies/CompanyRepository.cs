using GoBest.Data;
using GoBest.Models;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Companies
{
    public class CompanyRepository
    {
        private readonly MyDbContext _db;

        public CompanyRepository(MyDbContext db)
        {
            _db = db;
        }

        public async Task<Company?> GetCompanyByIdAsync(long companyId)
        {
            return await _db.Companies.FindAsync(companyId);
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            return await _db.Companies.ToListAsync();
        }

        public async Task<Company?> GetCompanyByNameAsync(string name)
        {
            return await _db.Companies
                .Where(c => c.Name.Contains(name))
                .FirstOrDefaultAsync();
        }

        public async Task SaveCompanyAsync(Company company)
        {
            _db.Companies.Add(company);
            await _db.SaveChangesAsync();
        }

        public async Task<long> SaveAndGetCompanyId(Company company)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));

            var existingCompany = _db.Companies
                .Where(c => c.Name == company.Name && c.CountryCode == company.CountryCode)
                .FirstOrDefault();

            if (existingCompany != null)
            {
                return existingCompany.Id;
            }

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();

            return company.Id;
        }
        
        public async Task<Company?> GetCompanyByIataCodeAsync(string iataCode)
        {
            if (string.IsNullOrEmpty(iataCode)) return null;
            
            return await _db.Companies
                .Where(c => c.IataCode == iataCode)
                .FirstOrDefaultAsync();
        }
        
        public async Task<Company> SaveCompanyFromApiAsync(Routes.DTO.CompanyAPIDto companyDto)
        {
            var company = await _db.Companies
                .Where(c => (c.Name == companyDto.Name && c.CountryCode == companyDto.Country_Code) || 
                           (!string.IsNullOrEmpty(companyDto.Iata_Code) && c.IataCode == companyDto.Iata_Code))
                .FirstOrDefaultAsync();
                
            if (company == null)
            {
                company = new Company
                {
                    Name = companyDto.Name,
                    IataCode = companyDto.Iata_Code,
                    CountryCode = companyDto.Country_Code
                };
                
                _db.Companies.Add(company);
                await _db.SaveChangesAsync();
            }
            
            return company;
        }
    }
}