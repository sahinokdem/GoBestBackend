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
            if (company == null) throw new ArgumentNullException(nameof(company));

            var existingCompany = await _db.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == company.Id);

            if (existingCompany == null)
            {
                _db.Companies.Add(company);
            }
            else
            {
                _db.Companies.Update(company);
            }

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
         
    }
}