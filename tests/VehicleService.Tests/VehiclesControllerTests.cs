using Microsoft.AspNetCore.Mvc;
using Moq;
using VehicleServiceApp.Controllers;
using VehicleServiceApp.Services;
using VehicleServiceApp.Models;

namespace VehicleService.Tests;

public class VehiclesControllerTests
{
    [Fact]
    public async Task GetVehicle_EmptyRegistration_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<IVehicleService>();
        var controller = new VehiclesController(mockService.Object);

        // Act
        var result = await controller.GetVehicle("");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Vehicle>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Registration number is required", response.ErrorMessage);
    }

    [Fact]
    public async Task GetVehicle_ValidRegistration_ReturnsVehicle()
    {
        // Arrange
        var mockService = new Mock<IVehicleService>();
        mockService.Setup(x => x.GetVehicleAsync("ABC123"))
            .ReturnsAsync(new Vehicle { RegistrationNumber = "ABC123" });
        
        var controller = new VehiclesController(mockService.Object);

        // Act
        var result = await controller.GetVehicle("ABC123");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Vehicle>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("ABC123", response.Data?.RegistrationNumber);
    }

    [Fact]
    public async Task GetVehicle_NotFound_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<IVehicleService>();
        mockService.Setup(x => x.GetVehicleAsync("INVALID"))
            .ReturnsAsync((Vehicle?)null);
        
        var controller = new VehiclesController(mockService.Object);

        // Act
        var result = await controller.GetVehicle("INVALID");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Vehicle>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Contains("not found", response.ErrorMessage);
    }
}