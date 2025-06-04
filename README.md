# ThreadPilot Integration Solution

## Architecture
- Two separate ASP.NET Core Web API projects (VehicleService and InsuranceService)
- HttpClientFactory for resilient service communication
- Service layer pattern for business logic
- Configuration-based data storage
- Polly resilience policies for service-to-service calls
- Global exception handling middleware

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
- Stress testing to identify performance bottlenecks and failure points under load
- Global exception middleware with proper HTTP status codes
- Structured error responses
- Graceful degradation when upstream services fail
- Circuit breaker and retry policies for service dependencies
- Comprehensive logging for debugging

## Extensibility
- API versioning support via URL path (/api/v1/...)
- Swagger/OpenAPI documentation
- Easy to add new insurance types through configuration
- Service interfaces allow for different implementations
- HttpClient abstraction enables mocking

## Security Considerations
- Input validation to prevent injection attacks
- HTTPS enforcement in production
- Rate limiting for API endpoints
- Authentication/authorization for sensitive endpoints (future)

## DevOps Pipeline
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

## Personal Reflection
This implementation demonstrates a clean microservice architecture pattern using HttpClientFactory for resilient service communication. The main challenge was designing the graceful degradation when upstream services fail while maintaining response consistency. If given more time, I would implement circuit breakers for the VehicleService integration and add integration tests covering failure scenarios.