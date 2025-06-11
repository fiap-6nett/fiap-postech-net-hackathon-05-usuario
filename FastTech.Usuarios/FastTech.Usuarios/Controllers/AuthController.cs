using Microsoft.AspNetCore.Mvc;

namespace FastTech.Usuarios.Controllers;

[Route("api/v1.0/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }
}