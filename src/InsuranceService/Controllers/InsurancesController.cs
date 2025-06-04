
using Microsoft.AspNetCore.Mvc;
using InsuranceServiceApp.Services;
using InsuranceServiceApp.Models;

namespace InsuranceServiceApp.Controllers;
[ApiController]
[Route("api/[controller]")]
public class InsurancesController(IInsuranceService InsuranceService) : ControllerBase
{
    [HttpGet("{personId}")]
    public async Task<ActionResult<PersonInsuranceResponse>> GetPersonInsurances(string PersonId)
    {
        var response = await InsuranceService.GetPersonInsurancesAsync(PersonId);
        
        if (response == null)
        {
            return NotFound(new {
                Success = false, 
                ErrorMessage = $"No insurances found for person ID {PersonId}"
            });
        }
        
        return Ok(response);
    }
}