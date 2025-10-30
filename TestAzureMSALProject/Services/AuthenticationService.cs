using Microsoft.Identity.Client;
using Microsoft.Extensions.Logging;
using TestAzureMSALProject.Models;
using TestAzureMSALProject.Exceptions;

namespace TestAzureMSALProject.Services;

/// <summary>
/// Provides Azure AD authentication using MSAL (Microsoft Authentication Library).
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly AppSettings _settings;
    private readonly ILogger<AuthenticationService> _logger;
    private IPublicClientApplication? _clientApp;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    /// <param name="settings">The application settings.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown when settings or logger is null.</exception>
    public AuthenticationService(AppSettings settings, ILogger<AuthenticationService> logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Acquires an access token for the configured scopes.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var app = InitializeClientApp();

            var result = await app.AcquireTokenInteractive(_settings.Scopes)
                .ExecuteAsync(cancellationToken);

            return result.AccessToken;
        }
        catch (MsalException msalEx)
        {
            _logger.LogError(msalEx, "Authentication failed");
            throw new AuthenticationException($"Failed to authenticate with Azure AD: {msalEx.Message}", msalEx);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication error");
            throw new AuthenticationException("An unexpected error occurred during authentication", ex);
        }
    }

    /// <summary>
    /// Initializes the MSAL public client application.
    /// </summary>
    /// <returns>The initialized public client application.</returns>
    private IPublicClientApplication InitializeClientApp()
    {
        return _clientApp ??= PublicClientApplicationBuilder
            .Create(_settings.ClientId)
            .WithAuthority($"https://login.microsoftonline.com/{_settings.TenantId}")
            .WithRedirectUri(_settings.RedirectUri)
            .Build();
    }
}
