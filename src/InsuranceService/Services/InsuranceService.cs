namespace InsuranceServiceApp.Services;

using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using InsuranceServiceApp.Models;
using VehicleServiceApp.Models;

public class InsuranceService : IInsuranceService
{
    private readonly Dictionary<string, List<Insurance>> _personInsurances;
    private readonly IVehicleApiClient _vehicleApiClient;
    private readonly ILogger<InsuranceService> _logger;
    
    public InsuranceService(
        IConfiguration configuration,
        IVehicleApiClient vehicleApiClient,
        ILogger<InsuranceService> logger)
    {
        _vehicleApiClient = vehicleApiClient;
        _logger = logger;
        
        // Load insurance data from configuration
        _personInsurances = configuration.GetSection("PersonInsurances")
            .Get<Dictionary<string, List<Insurance>>>() ?? new Dictionary<string, List<Insurance>>();
    }
    
    public async Task<PersonInsuranceResponse?> GetPersonInsurancesAsync(string personId)
    {
        if (!_personInsurances.TryGetValue(personId, out var insurances))
        {
            return null;
        }
        
        var response = new PersonInsuranceResponse
        {
            PersonId = personId,
            Insurances = new List<Insurance>(insurances)
        };
        
        // For car insurances, fetch vehicle details
        foreach (var insurance in response.Insurances.Where(i => i.Type == "Car"))
        {
            if (!string.IsNullOrEmpty(insurance.CarRegistrationNumber))
            {
                try
                {
                    var vehicle = await _vehicleApiClient.GetVehicleAsync(insurance.CarRegistrationNumber);
                    insurance.VehicleDetails = vehicle;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "Could not retrieve vehicle details for {Registration}",
                        insurance.CarRegistrationNumber);
                    // Continue without vehicle details - graceful degradation
                }
            }
        }
        
        return response;
    }
}