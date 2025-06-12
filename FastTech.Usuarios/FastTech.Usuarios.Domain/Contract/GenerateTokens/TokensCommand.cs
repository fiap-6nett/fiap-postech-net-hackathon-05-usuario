namespace FastTech.Usuarios.Domain.Contract.GenerateTokens;

public class TokensCommand
{
    public string User { get; set; }
    public string Password { get; set; }
}