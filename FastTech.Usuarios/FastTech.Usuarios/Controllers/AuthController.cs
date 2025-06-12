using FastTech.Usuarios.Domain.Contract.GenerateTokens;
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


    [HttpPost("Token")]
    [ProducesResponseType(typeof(TokensCommandResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] TokensCommand payload)
    {
        try
        {
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send data to the order queue. Error {ex.Message} - {ex.StackTrace} ");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}