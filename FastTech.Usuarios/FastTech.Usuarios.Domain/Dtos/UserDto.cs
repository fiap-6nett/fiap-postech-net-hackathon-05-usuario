using FastTech.Usuarios.Domain.Enums;

namespace FastTech.Usuarios.Domain.Dtos;

public class UserDto
{
    /// <summary>
    ///     Unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Name of the user.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Email address of the user.
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    ///     Cpf of the user.
    /// </summary>
    public string Cpf { get; set; }

    /// <summary>
    ///     Role of the user in the system.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    ///     Indicates whether the user is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    ///     Created timestamp for the user.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Last updated timestamp for the user.
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }
}