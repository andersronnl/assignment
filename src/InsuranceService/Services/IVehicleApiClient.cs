namespace InsuranceServiceApp.Services;

using VehicleServiceApp.Models;

public interface IVehicleApiClient
{
    Task<Vehicle?> GetVehicleAsync(string registrationNumber);
}