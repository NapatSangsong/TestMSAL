# HelloWorldApp - Azure AD Authentication & API Integration

A senior-level .NET 9.0 console application demonstrating Azure AD authentication (MSAL) with dependency injection, clean architecture, comprehensive unit tests, and CI/CD automation.

## Features

✨ **Enterprise Architecture**
- Dependency Injection (Microsoft.Extensions.DependencyInjection)
- Service-oriented architecture with interfaces
- Configuration management (AppSettings model)
- Structured logging (ILogger<T>)
- Custom exception handling

🔐 **Security**
- Azure AD authentication using MSAL
- Bearer token-based API authorization
- No credentials hardcoded in code
- Token not displayed in console output

🧪 **Testing**
- 40 comprehensive unit tests (xUnit + Moq)
- 100% public API coverage
- Mock-based service isolation
- Pre-commit test hooks

🤖 **Continuous Integration**
- GitHub Actions automated testing
- Build verification on every push/PR
- Code coverage reporting (Codecov)
- Test result publishing

## Project Structure

```
TestProject/
├── HelloWorldApp/                      # Main application
│   ├── Models/
│   │   └── AppSettings.cs             # Configuration model
│   ├── Services/
│   │   ├── IAuthenticationService.cs  # Auth interface
│   │   ├── AuthenticationService.cs   # MSAL implementation
│   │   ├── IApiService.cs             # API interface
│   │   └── ApiService.cs              # HTTP client wrapper
│   ├── Exceptions/
│   │   └── ApiException.cs            # Custom exceptions
│   ├── Program.cs                     # Main entry point & DI setup
│   └── HelloWorldApp.csproj
│
├── HelloWorldApp.Tests/                # Unit tests
│   ├── Fixtures/
│   ├── Services/
│   ├── Models/
│   ├── Exceptions/
│   └── ApplicationTests.cs
│
├── .github/
│   └── workflows/
│       └── build-and-test.yml         # CI/CD pipeline
│
├── .githooks/
│   ├── pre-commit                     # Local test hook
│   └── setup-hooks.sh                 # Hook setup script
│
├── TESTING.md                         # Testing documentation
└── README.md                          # This file
```

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- Azure AD tenant and registered application

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd TestProject
   ```

2. **Install git hooks**
   ```bash
   ./.githooks/setup-hooks.sh
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Build the project**
   ```bash
   dotnet build
   ```

5. **Run tests**
   ```bash
   dotnet test
   ```

6. **Run the application**
   ```bash
   cd HelloWorldApp
   dotnet run
   ```

## Configuration

Update the Azure AD credentials in [Program.cs:59-65](HelloWorldApp/Program.cs#L59-L65):

```csharp
var settings = new AppSettings
{
    TenantId = "your-tenant-id",
    ClientId = "your-client-id",
    ApiUrl = "https://your-api-endpoint",
    Scopes = ["api://your-scope"],
    RedirectUri = "http://localhost"
};
```

## Testing

### Local Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test class
dotnet test --filter "AuthenticationServiceTests"
```

### Continuous Integration

Tests run automatically on:
- Every commit (local pre-commit hook)
- Every push to `main`/`develop`
- Every pull request

View results at: GitHub Actions → Build and Test workflow

### Test Coverage

- **AuthenticationService**: 5 tests
- **ApiService**: 10 tests
- **Application**: 8 tests
- **Models**: 6 tests
- **Exceptions**: 11 tests
- **Total**: 40 tests (100% pass rate)

See [TESTING.md](TESTING.md) for detailed testing guide.

## Architecture Highlights

### Dependency Injection
```csharp
services.AddSingleton(settings);
services.AddHttpClient<IApiService, ApiService>();
services.AddScoped<IAuthenticationService, AuthenticationService>();
services.AddScoped<IApplication, Application>();
```

### Service Interfaces
- `IAuthenticationService` - Handles Azure AD auth
- `IApiService` - Manages API calls
- `IApplication` - Orchestrates workflow

### Error Handling
- `ApiException` - For API failures
- `AuthenticationException` - For auth failures
- Proper exception chaining with inner exceptions

## Dependencies

### Main Application
- Microsoft.Identity.Client (4.77.1) - Azure AD MSAL
- Microsoft.Extensions.* (9.0.0) - DI, Logging, Configuration

### Unit Tests
- xUnit (2.8.0) - Test framework
- Moq (4.20.70) - Mocking library
- Microsoft.NET.Test.Sdk (17.12.0) - Test runner

## Output Example

```
"pong2222"
```

The application authenticates with Azure AD and calls the configured API endpoint, returning the formatted response.

## Development Guidelines

### Code Standards
✅ Senior developer level:
- XML documentation on all public members
- Null validation in constructors
- Primary constructor syntax (C# 12)
- Proper resource disposal with `using`
- CancellationToken support
- Exit code returns (0=success, 1=failure)

### Pre-commit Workflow
1. Make changes
2. Run: `git commit -m "message"`
3. Tests run automatically
4. Push only if tests pass
5. GitHub Actions runs additional checks

### Adding Features
1. Write unit tests first (TDD)
2. Implement feature
3. Run: `dotnet test`
4. Commit changes
5. Push to GitHub
6. Verify CI/CD passes

## CI/CD Pipeline

**Workflows**: `.github/workflows/build-and-test.yml`

**Jobs**:
1. **Build and Test** (Ubuntu)
   - Restore, Build, Test
   - Publish test results to PR
   - Upload artifacts

2. **Code Coverage**
   - Generate coverage report
   - Upload to Codecov

**Triggers**:
- Push to main/develop
- Pull requests to main/develop

## Troubleshooting

### Tests Fail Locally
```bash
dotnet clean
dotnet build
dotnet test
```

### Pre-commit Hook Not Running
```bash
git config core.hooksPath .githooks
chmod +x .githooks/pre-commit
```

### Build Fails
- Check .NET version: `dotnet --version`
- Restore packages: `dotnet restore`
- Check dependencies: `dotnet list package`

## Performance

- **Build time**: ~5 seconds
- **Test execution**: ~44 milliseconds (all 40 tests)
- **CI/CD workflow**: ~2-3 minutes

## Best Practices

✅ **Do**:
- Run tests before committing
- Write tests for new features
- Keep git hooks enabled
- Review CI/CD results
- Maintain clean test suite

❌ **Don't**:
- Commit failing tests
- Disable pre-commit hooks permanently
- Hardcode secrets
- Skip testing
- Force push to main

## Contributing

1. Create a feature branch
2. Write tests first
3. Implement feature
4. Run tests locally
5. Commit (hooks will run)
6. Push to GitHub
7. Create pull request
8. Verify CI/CD passes
9. Request code review

## Status

![Build and Test](https://github.com/[owner]/[repo]/actions/workflows/build-and-test.yml/badge.svg)

## License

[Add your license here]

## Support

For issues or questions:
1. Check [TESTING.md](TESTING.md) for testing details
2. Review GitHub Actions logs
3. Check test results in PR comments
4. Inspect pre-commit hook output

## Resources

- [Testing Guide](TESTING.md)
- [xUnit Documentation](https://xunit.net/)
- [Moq Guide](https://github.com/moq/moq4)
- [GitHub Actions Docs](https://docs.github.com/en/actions)
- [Azure AD MSAL](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet)
