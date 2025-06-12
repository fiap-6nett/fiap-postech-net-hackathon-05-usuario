namespace FastTech.Usuarios.Domain.Dtos;

public class TokenPairDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}