namespace Zzaia.CoffeeShop.Order.Infrastructure.Authentication;

/// <summary>
/// JWT Bearer authentication configuration options.
/// </summary>
public sealed class JwtBearerConfiguration
{
    public const string SectionName = "Authentication:JwtBearer";

    /// <summary>
    /// Gets or sets the authority URL (Identity service).
    /// </summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience for token validation.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether HTTPS is required for metadata.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets whether token validation should be enabled.
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// Gets or sets whether audience validation should be enabled.
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    /// Gets or sets whether lifetime validation should be enabled.
    /// </summary>
    public bool ValidateLifetime { get; set; } = true;
}
