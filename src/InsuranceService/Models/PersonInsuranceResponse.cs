namespace InsuranceServiceApp.Models;

using System.Text.Json.Serialization;

public class PersonInsuranceResponse
{
    public string PersonId { get; set; } = string.Empty;
    public List<Insurance> Insurances { get; set; } = new();
    [JsonPropertyName("totalMonthlyCost")]
    public decimal TotalMonthlyCost => Insurances.Sum(I => I.MonthlyCost);
}
