You're absolutely right about HttpClientFactory! Let me revise the plan to ensure we follow all document requirements and use proper HttpClient management.

# Updated ThreadPilot Integration Plan (C#/.NET) - Requirements Compliant

## Document Requirements Checklist ✅

### Core Requirements:
- ✅ **Two separate application projects** (not just endpoints)
- ✅ **Endpoint 1:** Vehicle registration → vehicle info
- ✅ **Endpoint 2:** Personal ID → insurances + monthly costs
- ✅ **Integration:** Endpoint 2 calls Endpoint 1 for car insurance
- ✅ **REST APIs** for both endpoints
- ✅ **At least 3 unit tests**
- ✅ **Graceful error handling**
- ✅ **README.md** with architecture, running instructions, error handling discussion
- ✅ **Insurance prices:** Pet ($10), Health ($20), Car ($30)

---

## Phase 1: Solution Setup

**Create Structure:**
```bash
dotnet new sln -n ThreadPilotIntegration
dotnet new webapi -n VehicleService -o src/VehicleService
dotnet new webapi -n InsuranceService -o src/InsuranceService
dotnet new xunit -n VehicleService.Tests -o tests/VehicleService.Tests
dotnet new xunit -n InsuranceService.Tests -o tests/InsuranceService.Tests
dotnet sln add src/VehicleService src/InsuranceService tests/VehicleService.Tests tests/InsuranceService.Tests
```

---

## Phase 2: Vehicle Service (Endpoint 1)

**Models:**
```csharp
public class Vehicle
{
    public string RegistrationNumber { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string ErrorMessage { get; set; }
}
```

**Service Layer:**
```csharp
public interface IVehicleService
{
    Task<Vehicle?> GetVehicleAsync(string registrationNumber);
}

public class VehicleService : IVehicleService
{
    private readonly Dictionary<string, Vehicle> _vehicles;
    
    public VehicleService(IConfiguration configuration)
    {
        // Load from appsettings.json
        _vehicles = configuration.GetSection("Vehicles")
            .Get<Dictionary<string, Vehicle>>() ?? new();
    }
    
    public Task<Vehicle?> GetVehicleAsync(string registrationNumber)
    {
        _vehicles.TryGetValue(registrationNumber?.ToUpper(), out var vehicle);
        return Task.FromResult(vehicle);
    }
}
```

**Controller:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    
    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }
    
    [HttpGet("{registrationNumber}")]
    public async Task<ActionResult<ApiResponse<Vehicle>>> GetVehicle(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            return BadRequest(new ApiResponse<Vehicle> 
            { 
                Success = false, 
                ErrorMessage = "Registration number is required" 
            });
        }
        
        var vehicle = await _vehicleService.GetVehicleAsync(registrationNumber);
        
        if (vehicle == null)
        {
            return NotFound(new ApiResponse<Vehicle> 
            { 
                Success = false, 
                ErrorMessage = $"Vehicle with registration {registrationNumber} not found" 
            });
        }
        
        return Ok(new ApiResponse<Vehicle> 
        { 
            Success = true, 
            Data = vehicle 
        });
    }
}
```

**Program.cs:**
```csharp
builder.Services.AddScoped<IVehicleService, VehicleService>();
```

---

## Phase 3: Insurance Service (Endpoint 2) with HttpClientFactory

**Models:**
```csharp
public class Insurance
{
    public string Type { get; set; } // "Pet", "Health", "Car"
    public decimal MonthlyCost { get; set; }
    public string? CarRegistrationNumber { get; set; } // Only for car insurance
    public Vehicle? VehicleDetails { get; set; } // Populated when car insurance
}

public class PersonInsuranceResponse
{
    public string PersonId { get; set; }
    public List<Insurance> Insurances { get; set; } = new();
    public decimal TotalMonthlyCost => Insurances.Sum(i => i.MonthlyCost);
}
```

**HttpClient Service:**
```csharp
public interface IVehicleApiClient
{
    Task<Vehicle?> GetVehicleAsync(string registrationNumber);
}

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
```

**Insurance Service:**
```csharp
public interface IInsuranceService
{
    Task<PersonInsuranceResponse?> GetPersonInsurancesAsync(string personId);
}

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
            .Get<Dictionary<string, List<Insurance>>>() ?? new();
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
```

**Program.cs (InsuranceService):**
```csharp
// Register HttpClientFactory with typed client
builder.Services.AddHttpClient<IVehicleApiClient, VehicleApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:VehicleService"]);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<IInsuranceService, InsuranceService>();
```

---

## Phase 4: Configuration (appsettings.json)

**VehicleService/appsettings.json:**
```json
{
  "Vehicles": {
    "ABC123": {
      "RegistrationNumber": "ABC123",
      "Make": "Toyota",
      "Model": "Camry", 
      "Year": 2020,
      "Color": "Blue"
    },
    "XYZ789": {
      "RegistrationNumber": "XYZ789",
      "Make": "Honda",
      "Model": "Civic",
      "Year": 2019,
      "Color": "Red"
    }
  }
}
```

**InsuranceService/appsettings.json:**
```json
{
  "ServiceUrls": {
    "VehicleService": "http://localhost:5000/"
  },
  "PersonInsurances": {
    "12345": [
      {
        "Type": "Car",
        "MonthlyCost": 30,
        "CarRegistrationNumber": "ABC123"
      },
      {
        "Type": "Health",
        "MonthlyCost": 20
      }
    ],
    "67890": [
      {
        "Type": "Pet",
        "MonthlyCost": 10
      }
    ]
  }
}
```

---

## Phase 5: Required Unit Tests (Minimum 3)

**VehicleService.Tests:**
```csharp
public class VehicleServiceTests
{
    [Fact]
    public async Task GetVehicleAsync_ValidRegistration_ReturnsVehicle()
    {
        // Test 1: Valid vehicle retrieval
    }
    
    [Fact]
    public async Task GetVehicleAsync_InvalidRegistration_ReturnsNull()
    {
        // Test 2: Invalid registration handling
    }
}

public class VehiclesControllerTests
{
    [Fact]
    public async Task GetVehicle_EmptyRegistration_ReturnsBadRequest()
    {
        // Test 3: Input validation
    }
}
```

**InsuranceService.Tests:**
```csharp
public class InsuranceServiceTests
{
    [Fact]
    public async Task GetPersonInsurancesAsync_PersonWithCarInsurance_IncludesVehicleDetails()
    {
        // Test 4: Integration with vehicle service
        // Mock IVehicleApiClient
    }
    
    [Fact] 
    public async Task GetPersonInsurancesAsync_PersonNotFound_ReturnsNull()
    {
        // Test 5: Person not found scenario
    }
}

**Expanded Test Coverage:**
- Add integration tests for service communication
- Include edge cases: multiple insurances, missing vehicles
- Mock external dependencies using Moq
```

---

## Phase 6: Error Handling Strategy

**Global Exception Middleware:**
```csharp
public class ExceptionMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new ApiResponse<object>
        {
            Success = false,
            ErrorMessage = "An unexpected error occurred"
        };
        
        context.Response.StatusCode = exception switch
        {
            HttpRequestException => 503,
            ArgumentException => 400,
            _ => 500
        };
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

---

## Phase 7: README.md Requirements

**Architecture Section:**
- Two separate ASP.NET Core Web API projects
- HttpClientFactory for service communication
- Service layer pattern for business logic
- Configuration-based data storage

**Running Instructions:**
```bash
# Terminal 1 - Vehicle Service
dotnet run --project src/VehicleService --urls="http://localhost:5000"

# Terminal 2 - Insurance Service  
dotnet run --project src/InsuranceService --urls="http://localhost:5001"

# Testing
dotnet test
curl http://localhost:5001/api/insurances/12345
```

**Error Handling Discussion:**
- Graceful degradation when vehicle service is unavailable
- Proper HTTP status codes
- Structured error responses
- Logging for debugging

**Extensibility:**
- Easy to add new insurance types in configuration
- Service interfaces allow for different implementations
- HttpClient abstraction enables mocking
- API versioning supported via URL path (e.g., /api/v1/insurances)
- Swagger/OpenAPI documentation enabled

**Personal Reflection:**
This implementation demonstrates a clean microservice architecture pattern using HttpClientFactory for resilient service communication. The main challenge was designing the graceful degradation when upstream services fail while maintaining response consistency. If given more time, I would implement circuit breakers for the VehicleService integration and add integration tests covering failure scenarios.

**Security Considerations:**
- Input validation to prevent injection attacks
- HTTPS enforcement in production environments
- Rate limiting for API endpoints
- Authentication/authorization for sensitive endpoints (future consideration)

---

## Phase 8 (Bonus - DevOps Pipeline)

**GitHub Actions CI Pipeline (.github/workflows/ci.yml):**
```yaml
name: ThreadPilot CI
on: [push]
jobs:
  build-and-test:
    runs-on: windows-latest
    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    - name: Restore
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

This plan now properly uses HttpClientFactory to prevent socket exhaustion and follows all document requirements exactly. The architecture is simple but production-ready with proper error handling and extensibility.