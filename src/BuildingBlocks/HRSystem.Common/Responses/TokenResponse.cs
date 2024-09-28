namespace HRSystem.Common.Responses;

public class TokenResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpoiryTime { get; set; }
}
