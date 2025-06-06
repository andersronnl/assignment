namespace VehicleServiceApp.Models;

using System.Text.Json.Serialization;

public class Vehicle
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    [JsonPropertyName("year")]
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
}
