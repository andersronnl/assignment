namespace InsuranceServiceApp.Services;

using InsuranceServiceApp.Models;

public interface IInsuranceService
{
    Task<PersonInsuranceResponse?> GetPersonInsurancesAsync(string personId);
}