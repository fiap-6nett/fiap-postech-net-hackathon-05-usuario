using Microsoft.AspNetCore.Mvc;

namespace FastTech.Usuarios.Controllers;

[Route("api/v1.0/pedidos")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }
}