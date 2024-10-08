namespace HRSystem.Common.Requests.Identity;

public class TokenRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}


public class RefreshTokenRequest
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
