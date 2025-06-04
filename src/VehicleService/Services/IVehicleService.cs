namespace VehicleServiceApp.Services;

using VehicleServiceApp.Models;

public interface IVehicleService
{
    Task<Vehicle?> GetVehicleAsync(string registrationNumber);
}