namespace FastTech.Usuarios.Application.Interfaces;

public interface IUserService
{
    Task<string> GenerateTokenAsync(string name, string passwordBase64);
}