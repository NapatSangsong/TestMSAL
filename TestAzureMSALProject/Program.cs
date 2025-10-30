using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestAzureMSALProject.Models;
using TestAzureMSALProject.Services;
using TestAzureMSALProject.Exceptions;

namespace TestAzureMSALProject;

/// <summary>
/// Main entry point for the TestAzureMSALProject application.
/// Demonstrates Azure AD authentication and API calls with dependency injection.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Main entry point of the application.
    /// </summary>
    /// <returns>Exit code: 0 for success, 1 for failure.</returns>
    static async Task<int> Main()
    {
        try
        {
            var services = ConfigureServices();
            using var serviceProvider = services.BuildServiceProvider();

            var application = serviceProvider.GetRequiredService<IApplication>();
            await application.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    /// <summary>
    /// Configures the dependency injection services.
    /// </summary>
    /// <returns>The configured service collection.</returns>
    private static ServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();

        // Configure logging (errors only)
        services.AddLogging(config =>
        {
            config.SetMinimumLevel(LogLevel.Error);
        });

        // Configure application settings
        var settings = new AppSettings
        {
            TenantId = "b1304b00-cd3f-49a1-bf72-2ffcc6e7cec0",
            ClientId = "0e5414fa-b234-4175-b893-43ed2e254ac9",
            ApiUrl = "https://tlmetcmas01.azurewebsites.net/api/hi",
            Scopes = ["api://0e5414fa-b234-4175-b893-43ed2e254ac9/api.access"],
            RedirectUri = "http://localhost"
        };

        services.AddSingleton(settings);

        // Configure HttpClient
        services.AddHttpClient<IApiService, ApiService>();

        // Register services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IApplication, Application>();

        return services;
    }
}

/// <summary>
/// Defines the main application workflow.
/// </summary>
public interface IApplication
{
    /// <summary>
    /// Runs the application.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task RunAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implements the main application workflow.
/// </summary>
public class Application(
    IAuthenticationService authenticationService,
    IApiService apiService,
    ILogger<Application> logger) : IApplication
{
    private readonly IAuthenticationService _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    private readonly IApiService _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
    private readonly ILogger<Application> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Runs the application workflow.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
    /// <exception cref="ApiException">Thrown when the API call fails.</exception>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: Authenticate
            var accessToken = await _authenticationService.GetAccessTokenAsync(cancellationToken);

            // Step 2: Call API
            var response = await _apiService.CallApiAsync(accessToken, cancellationToken);

            // Step 3: Display result
            Console.WriteLine(response);
        }
        catch (AuthenticationException authEx)
        {
            _logger.LogError(authEx, "Authentication failed");
            throw;
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(apiEx, "API call failed");
            throw;
        }
    }
}
