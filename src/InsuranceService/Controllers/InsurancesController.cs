namespace InsuranceServiceApp.Controllers;

using Microsoft.AspNetCore.Mvc;
using InsuranceServiceApp.Services;
using InsuranceServiceApp.Models;

[ApiController]
[Route("api/[controller]")]
public class InsurancesController : ControllerBase
{
    private readonly IInsuranceService _insuranceService;
    
    public InsurancesController(IInsuranceService insuranceService)
    {
        _insuranceService = insuranceService;
    }
    
    [HttpGet("{personId}")]
    public async Task<ActionResult<PersonInsuranceResponse>> GetPersonInsurances(string personId)
    {
        var response = await _insuranceService.GetPersonInsurancesAsync(personId);
        
        if (response == null)
        {
            return NotFound(new { 
                Success = false, 
                ErrorMessage = $"No insurances found for person ID {personId}" 
            });
        }
        
        return Ok(response);
    }
}