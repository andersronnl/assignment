namespace InsuranceServiceApp.Models;

public class PersonInsuranceResponse
{
    public string PersonId { get; set; } = string.Empty;
    public List<Insurance> Insurances { get; set; } = new();
    public decimal TotalMonthlyCost => Insurances.Sum(i => i.MonthlyCost);
}