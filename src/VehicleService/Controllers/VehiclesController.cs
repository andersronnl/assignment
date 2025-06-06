namespace VehicleServiceApp.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services;
using Models;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController(IVehicleService VehicleService) : ControllerBase
{
    [HttpGet("{registrationNumber}")]
    public async Task<ActionResult<ApiResponse<Vehicle>>> GetVehicle(string RegistrationNumber)
    {
        if (string.IsNullOrWhiteSpace(RegistrationNumber))
        {
            return BadRequest(new ApiResponse<Vehicle>
            {
                Success = false,
                ErrorMessage = "Registration number is required"
            });
        }
        
        var vehicle = await VehicleService.GetVehicleAsync(RegistrationNumber);
        
        if (vehicle == null)
        {
            return NotFound(new ApiResponse<Vehicle>
            {
                Success = false,
                ErrorMessage = $"Vehicle with registration {RegistrationNumber} not found"
            });
        }
        
        return Ok(new ApiResponse<Vehicle>
        {
            Success = true,
            Data = vehicle
        });
    }
}
