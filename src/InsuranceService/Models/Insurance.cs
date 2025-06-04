namespace InsuranceServiceApp.Models;

using VehicleServiceApp.Models;

public class Insurance
{
    public string Type { get; set; } = string.Empty; // "Pet", "Health", "Car"
    public decimal MonthlyCost { get; set; }
    public string? CarRegistrationNumber { get; set; } // Only for car insurance
    public Vehicle? VehicleDetails { get; set; } // Populated when car insurance
}