using GoBest.Data;
using GoBest.Models;
using GoBest.Users;
using Microsoft.EntityFrameworkCore;

namespace GoBest.Companies;

public class CompanyMaintainerRepository
{
    private readonly MyDbContext _db;

    public CompanyMaintainerRepository(MyDbContext db)
    {
        _db = db;
    }

    public async Task<bool> IsCompanyMaintainerOfCompanyAsync(long companyId, long userId)
    {
        return await _db.CompanyMaintainers
            .AnyAsync(cm => cm.CompanyId == companyId && cm.UserId == userId);
    }

    public async Task<bool> IsCompanyMaintainerAsync(long userId)
    {
        return await _db.CompanyMaintainers
            .AnyAsync(cm => cm.UserId == userId);
    }
    public async Task<long> GetCompanyIdByMaintainerAsync(long userId)
    {
        Console.WriteLine($"Looking up company for userId: {userId}");

        var maintainer = await _db.CompanyMaintainers
            .FirstOrDefaultAsync(cm => cm.UserId == userId);

        if (maintainer == null)
        {
            Console.WriteLine($"No company maintainer found for userId = {userId}");
            throw new UnauthorizedAccessException("User is not a company maintainer.");
        }

        Console.WriteLine($"CompanyId for user {userId} is {maintainer.CompanyId}");
        return (long)maintainer.CompanyId;
    }

    
    public async Task<CompanyMaintainer> AddCompanyMaintainerAsync(long companyId, long userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null)
            throw new Exception($"User with ID {userId} not found");

        user.Role = UserRole.CompanyRep;
        _db.Users.Update(user); // ✅ bu satır eklenmeli

        var maintainer = new CompanyMaintainer
        {
            CompanyId = companyId,
            UserId = userId
        };

        _db.CompanyMaintainers.Add(maintainer);
        await _db.SaveChangesAsync();

        return maintainer;
    }


}
