using Microsoft.Extensions.Configuration;
using VehicleServiceApp.Models;
using VehicleServiceApp.Services;

namespace VehicleService.Tests;

public class VehicleServiceTests
{
    [Fact]
    public async Task GetVehicleAsync_ValidRegistration_ReturnsVehicle()
    {
        // Arrange
        var vehicles = new Dictionary<string, Vehicle>
        {
            ["ABC123"] = new Vehicle { RegistrationNumber = "ABC123" }
        };
        
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Vehicles:ABC123:RegistrationNumber"] = "ABC123"
            })
            .Build();
        
        var service = new VehicleServiceApp.Services.VehicleService(config);

        // Act
        var result = await service.GetVehicleAsync("ABC123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ABC123", result?.RegistrationNumber);
    }

    [Fact]
    public async Task GetVehicleAsync_InvalidRegistration_ReturnsNull()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>())
            .Build();
        
        var service = new VehicleServiceApp.Services.VehicleService(config);

        // Act
        var result = await service.GetVehicleAsync("INVALID");

        // Assert
        Assert.Null(result);
    }
}