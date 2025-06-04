namespace InsuranceServiceApp.Services;

using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VehicleServiceApp.Models;

public class VehicleApiClient : IVehicleApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VehicleApiClient> _logger;
    
    public VehicleApiClient(HttpClient httpClient, ILogger<VehicleApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<Vehicle?> GetVehicleAsync(string registrationNumber)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/vehicles/{registrationNumber}");
            
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
            _logger.LogError(ex, "Failed to retrieve vehicle {RegistrationNumber}", registrationNumber);
            throw;
        }
    }
}