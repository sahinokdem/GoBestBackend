using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoBest.Companies;

public class CompanyController : ControllerBase
{
    private readonly CompanyService _companyService;

    public CompanyController(CompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet("companies")]
    public async Task<IActionResult> GetAllCompanies()
    {
        var companies = await _companyService.GetAllCompaniesAsync();
        return Ok(companies);
    }


    [Authorize(Policy = "AdminOnly")]
    [HttpPut("companies/{id:long}")]
    public async Task<IActionResult> UpdateCompany(long id, [FromBody] UpdateCompanyRequest dto)
        => await _companyService.UpdateCompanyAsync(id, dto) ? NoContent() : NotFound();
}
