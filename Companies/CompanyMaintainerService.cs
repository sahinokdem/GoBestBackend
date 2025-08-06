
using System.Threading.Tasks;
using GoBest.Models;
using GoBest.Users;

namespace GoBest.Companies;

public class CompanyMaintainerService
{
    private readonly CompanyMaintainerRepository _companyMaintainerRepository;
    private readonly UserService _userService;

    public CompanyMaintainerService(CompanyMaintainerRepository companyMaintainerRepository, UserService userService)
    {
        _userService = userService;
        _companyMaintainerRepository = companyMaintainerRepository;
    }

    public async Task<bool> IsCompanyMaintainerOfCompanyAsync(long companyId, long userId)
    {
        return await _companyMaintainerRepository
        .IsCompanyMaintainerOfCompanyAsync(companyId, userId);
    }

    public async Task<bool> AddCompanyMaintainerAsync(long companyId, string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            throw new ArgumentException($"User with email {email} does not exist.");
        }
        var userId = user.Id;
        if (await IsCompanyMaintainerOfCompanyAsync(companyId, userId))
        {
            throw new InvalidOperationException("User is already a maintainer of this company.");
        }

        var companyMaintainer = await _companyMaintainerRepository
        .AddCompanyMaintainerAsync(companyId, userId);

        return companyMaintainer != null;
    }

    internal async Task<bool> IsCompanyMaintainerAsync(long id)
    {
        return await _companyMaintainerRepository.IsCompanyMaintainerAsync(id);
    }

    public async Task<long> GetCompanyIdByMaintainerAsync(long userId)
    {
        return await _companyMaintainerRepository.GetCompanyIdByMaintainerAsync(userId);
    }
}