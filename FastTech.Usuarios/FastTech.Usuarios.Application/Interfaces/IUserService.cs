using FastTech.Usuarios.Domain.Entities;
using FastTech.Usuarios.Domain.Enums;

namespace FastTech.Usuarios.Application.Interfaces;

public interface IUserService
{
    Task<TokenEntity> GenerateTokenAsync(string user, string passwordBase64, LoginIdentifierType loginIdentifierType);
    Task<UserEntity> RegisterUserAsync(string name, string cpf, string Email, string passwordBase64, string passwordHash, UserRole role);
}