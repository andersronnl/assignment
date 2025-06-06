namespace InsuranceServiceApp.Services;

using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VehicleServiceApp.Models;

public class VehicleApiClient(HttpClient HttpClient, ILogger<VehicleApiClient> Logger) : IVehicleApiClient
{
    
    public async Task<Vehicle?> GetVehicleAsync(string registrationNumber)
    {
        try
        {
            var response = await HttpClient.GetAsync($"api/vehicles/{registrationNumber}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<Vehicle>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return apiResponse?.Data;
        }
        catch (HttpRequestException ex)
        {
            Logger.LogError(ex, "Failed to retrieve vehicle {RegistrationNumber}", registrationNumber);
            throw new InvalidOperationException($"Failed to retrieve vehicle with registration number {registrationNumber}", ex);
        }
    }
}
