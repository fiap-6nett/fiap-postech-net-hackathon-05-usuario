namespace FastTech.Usuarios.Domain.Contract.GenerateTokens;

public class TokensCommand
{
    /// <summary>
    ///     Nome de usuário utilizado para autenticação.
    ///     <para>Exemplo: <c>admin</c></para>
    /// </summary>
    public string User { get; set; }

    /// <summary>
    ///     Senha codificada em Base64 correspondente ao usuário.
    ///     <para>Exemplo (Base64 de <c>admin123</c>): <c>YWRtaW4xMjM=</c></para>
    /// </summary>
    public string PasswordBase64 { get; set; }
}