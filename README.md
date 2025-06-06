# ThreadPilot Integration Solution

## Architecture
- Two separate ASP.NET Core Web API projects (VehicleService and InsuranceService)
- HttpClientFactory for resilient service communication
- Service layer pattern for business logic
- Configuration-based data storage
- Standard ASP.NET Core error handling with ProblemDetails
- Consistent ApiResponse<T> pattern for all endpoints

## Running Instructions
```bash
# Terminal 1 - Vehicle Service
dotnet run --project src/VehicleService --urls="http://localhost:5000"

# Terminal 2 - Insurance Service
dotnet run --project src/InsuranceService --urls="http://localhost:5001"

# Testing
dotnet test

# Sample requests:
curl http://localhost:5000/api/vehicles/ABC123
curl http://localhost:5001/api/insurances/12345

# Stress Testing (requires running services)
dotnet run --project tests/StressTests
```

## Error Handling Strategy
- Standard ASP.NET Core exception handling with ProblemDetails
- Consistent ApiResponse<T> wrapper for all API responses with Success/Error information
- Proper HTTP status codes for different error scenarios
- Graceful degradation when upstream services fail
- Comprehensive logging for debugging
- Stress testing to identify performance bottlenecks

## Extensibility
- OpenAPI documentation with .NET 9's AddOpenApi/MapOpenApi
- Consistent ApiResponse<T> pattern makes it easy to extend with additional metadata
- Easy to add new insurance types through configuration
- Service interfaces allow for different implementations
- HttpClient abstraction for resilient service communication

## Security Considerations
- Input validation to prevent injection attacks
- HTTPS enforcement in production
- Rate limiting for API endpoints
- Authentication/authorization for sensitive endpoints (future)

## DevOps Pipeline
We've implemented a CI pipeline using Azure DevOps. The pipeline configuration is located in [azure-pipelines.yml](azure-pipelines.yml).

The pipeline performs the following steps:
- Triggers on changes to the main branch
- Uses Windows build agents
- Restores NuGet packages
- Builds the solution in Release configuration
- Runs all unit tests
- Publishes build artifacts

To set up the pipeline in Azure DevOps:
1. Create a new pipeline and select "Existing Azure Pipelines YAML file"
2. Point to the azure-pipelines.yml file in your repository
3. Save and run the pipeline

## Personal Reflection
This implementation demonstrates a balanced microservice architecture using ASP.NET Core's capabilities with a consistent ApiResponse pattern. The focus was on creating a maintainable solution with appropriate abstractions. The main challenge was ensuring consistent error handling and response formats across services while maintaining test compatibility. If given more time, I would add integration tests using WebApplicationFactory and implement more comprehensive validation with FluentValidation.
