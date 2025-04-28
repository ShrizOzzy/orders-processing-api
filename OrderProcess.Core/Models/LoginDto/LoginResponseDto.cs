namespace OrderProcess.Core.Models.LoginDto;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}
