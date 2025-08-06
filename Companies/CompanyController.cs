using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoBest.Companies;

public class CompanyController : ControllerBase
{
    private readonly CompanyService _companyService;
    private readonly CompanyMaintainerService _companyMaintainerService;

    public CompanyController(CompanyService companyService, CompanyMaintainerService companyMaintainerService)
    {
        _companyMaintainerService = companyMaintainerService;
        _companyService = companyService;
    }

    [HttpGet("companies")]
    public async Task<IActionResult> GetAllCompanies()
    {
        var companies = await _companyService.GetAllCompaniesAsync();
        return Ok(companies);
    }


    [Authorize(Policy = "AdminAndCompanyRepOnly")]
    [HttpPut("companies/{id:long}")]
    public async Task<IActionResult> UpdateCompany(long id, [FromBody] UpdateCompanyRequest dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null) return Unauthorized();
        long userId = long.Parse(userIdClaim.Value);

        var updated = await _companyService.UpdateCompanyAsync(userId, id, dto);
        return updated ? NoContent() : NotFound();
    }


    [Authorize(Policy = "AdminOnly")]
    [HttpPost("companyMaintainers/{companyId:long}/users/{email}")]
    public async Task<IActionResult> AddCompanyMaintainer(long companyId, string email)
    {
        var added = await _companyMaintainerService.AddCompanyMaintainerAsync(companyId, email);
        return added ? NoContent() : NotFound();
    }
}