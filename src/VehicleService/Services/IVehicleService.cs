namespace VehicleServiceApp.Services;

using Models;

public interface IVehicleService
{
    public Task<Vehicle?> GetVehicleAsync(string RegistrationNumber);
}