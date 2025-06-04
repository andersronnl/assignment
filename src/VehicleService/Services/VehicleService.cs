namespace VehicleServiceApp.Services;

using Models;
using Microsoft.Extensions.Configuration;

public class VehicleService(IConfiguration Configuration) : IVehicleService
{
    private readonly Dictionary<string, Vehicle> _vehicles = Configuration.GetSection("Vehicles")
            .Get<Dictionary<string, Vehicle>>() ?? new Dictionary<string, Vehicle>();
    
    public Task<Vehicle?> GetVehicleAsync(string RegistrationNumber)
    {
        if (string.IsNullOrWhiteSpace(RegistrationNumber))
            return Task.FromResult<Vehicle?>(null);
            
        _vehicles.TryGetValue(RegistrationNumber.ToUpper(), out var vehicle);
        return Task.FromResult(vehicle);
    }
}