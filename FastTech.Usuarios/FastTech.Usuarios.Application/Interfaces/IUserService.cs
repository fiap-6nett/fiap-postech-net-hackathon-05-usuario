using FastTech.Usuarios.Domain.Dtos;

namespace FastTech.Usuarios.Application.Interfaces;

public interface IUserService
{
    Task<TokenPairDto> GenerateTokenAsync(string name, string passwordBase64);
}