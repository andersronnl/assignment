using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using InsuranceServiceApp.Services;
using VehicleServiceApp.Models;

namespace InsuranceService.Tests;

public class InsuranceServiceTests
{
    [Fact]
    public async Task GetPersonInsurancesAsync_PersonWithCarInsurance_IncludesVehicleDetails()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection((IDictionary<string, string?>)new Dictionary<string, string>
            {
                ["PersonInsurances:12345:0:Type"] = "Car",
                ["PersonInsurances:12345:0:MonthlyCost"] = "30",
                ["PersonInsurances:12345:0:CarRegistrationNumber"] = "ABC123"
            })
            .Build();

        var mockVehicleApi = new Mock<IVehicleApiClient>();
        mockVehicleApi.Setup(X => X.GetVehicleAsync("ABC123"))
            .ReturnsAsync(new Vehicle { RegistrationNumber = "ABC123" });

        var mockLogger = new Mock<ILogger<InsuranceServiceApp.Services.InsuranceService>>();

        var service = new InsuranceServiceApp.Services.InsuranceService(
            config,
            mockVehicleApi.Object,
            mockLogger.Object);

        // Act
        var result = await service.GetPersonInsurancesAsync("12345");

        // Assert
        Assert.NotNull(result);
        var carInsurance = result.Insurances.First(I => I.Type == "Car");
        Assert.NotNull(carInsurance.VehicleDetails);
        Assert.Equal("ABC123", carInsurance.VehicleDetails?.RegistrationNumber);
    }

    [Fact]
    public async Task GetPersonInsurancesAsync_PersonNotFound_ReturnsNull()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection((IDictionary<string, string?>)new Dictionary<string, string>())
            .Build();

        var mockVehicleApi = new Mock<IVehicleApiClient>();
        var mockLogger = new Mock<ILogger<InsuranceServiceApp.Services.InsuranceService>>();

        var service = new InsuranceServiceApp.Services.InsuranceService(
            config,
            mockVehicleApi.Object,
            mockLogger.Object);

        // Act
        var result = await service.GetPersonInsurancesAsync("INVALID");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPersonInsurancesAsync_VehicleServiceUnavailable_ReturnsInsurancesWithoutVehicleDetails()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection((IDictionary<string, string?>)new Dictionary<string, string>
            {
                ["PersonInsurances:12345:0:Type"] = "Car",
                ["PersonInsurances:12345:0:MonthlyCost"] = "30",
                ["PersonInsurances:12345:0:CarRegistrationNumber"] = "ABC123"
            })
            .Build();

        var mockVehicleApi = new Mock<IVehicleApiClient>();
        mockVehicleApi.Setup(X => X.GetVehicleAsync("ABC123"))
            .ThrowsAsync(new HttpRequestException());

        var mockLogger = new Mock<ILogger<InsuranceServiceApp.Services.InsuranceService>>();

        var service = new InsuranceServiceApp.Services.InsuranceService(
            config,
            mockVehicleApi.Object,
            mockLogger.Object);

        // Act
        var result = await service.GetPersonInsurancesAsync("12345");

        // Assert
        Assert.NotNull(result);
        var carInsurance = result.Insurances.First(I => I.Type == "Car");
        Assert.Null(carInsurance.VehicleDetails);
    }
}