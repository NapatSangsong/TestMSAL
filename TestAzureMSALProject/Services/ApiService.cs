using System.Text.Json;
using Microsoft.Extensions.Logging;
using TestAzureMSALProject.Models;
using TestAzureMSALProject.Exceptions;

namespace TestAzureMSALProject.Services;

/// <summary>
/// Provides authenticated API call functionality.
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _settings;
    private readonly ILogger<ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="settings">The application settings.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public ApiService(HttpClient httpClient, AppSettings settings, ILogger<ApiService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
    }

    /// <summary>
    /// Calls the API endpoint with the provided access token.
    /// </summary>
    /// <param name="accessToken">The Bearer token for authentication.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The API response content.</returns>
    /// <exception cref="ArgumentException">Thrown when accessToken is null or empty.</exception>
    /// <exception cref="ApiException">Thrown when the API call fails.</exception>
    public async Task<string> CallApiAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        ValidateAccessToken(accessToken);

        try
        {
            SetAuthorizationHeader(accessToken);

            using var response = await _httpClient.GetAsync(_settings.ApiUrl, cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API call failed with status {StatusCode}", response.StatusCode);
                throw new ApiException($"API call failed with status code {response.StatusCode}");
            }

            return FormatResponse(content);
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "HTTP request failed");
            throw new ApiException("An error occurred while calling the API", httpEx);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API call error");
            throw new ApiException("An unexpected error occurred during the API call", ex);
        }
    }

    /// <summary>
    /// Validates that the access token is provided.
    /// </summary>
    /// <param name="accessToken">The access token to validate.</param>
    /// <exception cref="ArgumentException">Thrown when token is null or empty.</exception>
    private static void ValidateAccessToken(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new ArgumentException("Access token cannot be null or empty", nameof(accessToken));
        }
    }

    /// <summary>
    /// Sets the Bearer token in the HTTP client's authorization header.
    /// </summary>
    /// <param name="accessToken">The access token.</param>
    private void SetAuthorizationHeader(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", accessToken);
    }

    /// <summary>
    /// Formats the API response (pretty-prints JSON if applicable).
    /// </summary>
    /// <param name="content">The response content.</param>
    /// <returns>The formatted response.</returns>
    private string FormatResponse(string content)
    {
        try
        {
            using var jsonDocument = JsonDocument.Parse(content);
            return JsonSerializer.Serialize(jsonDocument, _jsonOptions);
        }
        catch
        {
            // If not valid JSON, return as-is
            return content;
        }
    }

}
