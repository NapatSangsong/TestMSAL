namespace TestAzureMSALProject.Models;

/// <summary>
/// Application configuration settings for Azure AD authentication and API access.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Gets or sets the Azure AD tenant ID.
    /// </summary>
    public required string TenantId { get; set; }

    /// <summary>
    /// Gets or sets the Azure AD client ID (application ID).
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the API endpoint URL.
    /// </summary>
    public required string ApiUrl { get; set; }

    /// <summary>
    /// Gets or sets the API scopes for authentication.
    /// </summary>
    public string[] Scopes { get; set; } = [];

    /// <summary>
    /// Gets or sets the OAuth redirect URI.
    /// </summary>
    public required string RedirectUri { get; set; }
}
