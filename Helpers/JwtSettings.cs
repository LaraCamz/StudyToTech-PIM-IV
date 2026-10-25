namespace StudyToTech.Api.Helpers;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "StudyToTech";
    public string Audience { get; set; } = "StudyToTechClients";
    public int ExpirationMinutes { get; set; } = 60;
}
