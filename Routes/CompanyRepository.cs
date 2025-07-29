using GoBest.Data;
using GoBest.Models;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Routes
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

        public async Task<Company?> GetCompanyByIdAsync(long companyId)
        {
            return await _db.Companies.FindAsync(companyId);
        }

        public async Task SaveCompanyAsync(Company company)
        {
            _db.Companies.Add(company);
            await _db.SaveChangesAsync();
        }
    }
}