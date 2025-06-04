namespace VehicleServiceApp.Services;

using VehicleServiceApp.Models;
using Microsoft.Extensions.Configuration;

public class VehicleService : IVehicleService
{
    private readonly Dictionary<string, Vehicle> _vehicles;
    
    public VehicleService(IConfiguration configuration)
    {
        _vehicles = configuration.GetSection("Vehicles")
            .Get<Dictionary<string, Vehicle>>() ?? new Dictionary<string, Vehicle>();
    }
    
    public Task<Vehicle?> GetVehicleAsync(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
            return Task.FromResult<Vehicle?>(null);
            
        _vehicles.TryGetValue(registrationNumber.ToUpper(), out var vehicle);
        return Task.FromResult(vehicle);
    }
}