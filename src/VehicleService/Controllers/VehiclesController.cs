namespace VehicleServiceApp.Controllers;

using Microsoft.AspNetCore.Mvc;
using VehicleServiceApp.Services;
using VehicleServiceApp.Models;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    
    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }
    
    [HttpGet("{registrationNumber}")]
    public async Task<ActionResult<ApiResponse<Vehicle>>> GetVehicle(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            return BadRequest(new ApiResponse<Vehicle> 
            { 
                Success = false, 
                ErrorMessage = "Registration number is required" 
            });
        }
        
        var vehicle = await _vehicleService.GetVehicleAsync(registrationNumber);
        
        if (vehicle == null)
        {
            return NotFound(new ApiResponse<Vehicle> 
            { 
                Success = false, 
                ErrorMessage = $"Vehicle with registration {registrationNumber} not found" 
            });
        }
        
        return Ok(new ApiResponse<Vehicle> 
        { 
            Success = true, 
            Data = vehicle 
        });
    }
}