using Microsoft.AspNetCore.Mvc;

namespace FastTech.Usuarios.Controllers;


[Route("api/v1.0/pedidos")]
public class UsuarioController : ControllerBase
{
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(ILogger<UsuarioController> logger)
    {
        _logger = logger;
    }
}