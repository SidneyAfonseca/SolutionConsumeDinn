using DIn.Domain;
using DIn.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsumeDinnApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly AppDbContext _context;

    public ConfigController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments()
    {
        var departments = await _context.Departments.ToListAsync();
        return Ok(departments);
    }

    [HttpGet("accountplans")]
    public async Task<IActionResult> GetAccountPlans()
    {
        var plans = await _context.AccountPlans.ToListAsync();
        return Ok(plans);
    }
}
