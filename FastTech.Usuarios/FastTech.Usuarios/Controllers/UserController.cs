using FastTech.Usuarios.Application.Interfaces;
using FastTech.Usuarios.Contract.CreateClient;
using FastTech.Usuarios.Contract.CreateEmployee;
using FastTech.Usuarios.Contract.DeleteUser;
using FastTech.Usuarios.Contract.GetUserById;
using FastTech.Usuarios.Contract.UpdateUser;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FastTech.Usuarios.Controllers;

[ApiController]
[Route("api/v1.0/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IValidator<CreateClientCommand> _validatorCreateClientCommand;
    private readonly IValidator<CreateEmployeeCommand> _validatorCreateEmployeeCommand;
    private readonly IValidator<DeleteUserCommand> _validatorDeleteUserCommand;
    private readonly IValidator<GetUserByIdQuery> _validatorGetUserByIdQuery;
    private readonly IValidator<UpdateUserCommand> _validatorUpdateUserCommand;

    public UserController(ILogger<UserController> logger, IUserService userService, IValidator<CreateClientCommand> validatorCreateClientCommand, IValidator<CreateEmployeeCommand> validatorCreateEmployeeCommand, IValidator<DeleteUserCommand> validatorDeleteUserCommand, IValidator<GetUserByIdQuery> validatorGetUserByIdQuery, IValidator<UpdateUserCommand> validatorUpdateUserCommand)
    {
        _logger = logger;
        _userService = userService;
        _validatorCreateClientCommand = validatorCreateClientCommand;
        _validatorCreateEmployeeCommand = validatorCreateEmployeeCommand;
        _validatorDeleteUserCommand = validatorDeleteUserCommand;
        _validatorGetUserByIdQuery = validatorGetUserByIdQuery;
        _validatorUpdateUserCommand = validatorUpdateUserCommand;
    }

    /// <summary>
    ///     Cadastra um novo cliente no sistema.
    /// </summary>
    [HttpPost("clientes")]
    [ProducesResponseType(typeof(CreateClientCommandResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand cliente)
    {
        //var result = await _userService.CriarClienteAsync(cliente);
        //return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
        return Ok(true);
    }

    /// <summary>
    ///     Cadastra um novo funcionário (ex: atendente, gerente).
    /// </summary>
    [HttpPost("funcionarios")]
    [ProducesResponseType(typeof(CreateEmployeeCommandResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeCommand funcionario)
    {
        //var result = await _usuarioService.CriarFuncionarioAsync(funcionario);
        //return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
        return Ok(true);
    }

    /// <summary>
    ///     Obtém os dados de um usuário específico.
    /// </summary>
    /// <param name="id">ID do usuário</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetUserByIdResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        return Ok(true);
        return NotFound();

        //return Ok(usuario);
    }

    /// <summary>
    ///     Atualiza os dados de um usuário existente.
    /// </summary>
    /// <param name="id">ID do usuário</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateUserCommandResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand dto)
    {
        return Ok(true);
        return NoContent();
    }

    /// <summary>
    ///     Exclui um usuário (por gerente ou o próprio usuário, se for cliente).
    /// </summary>
    /// <param name="id">ID do usuário</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(DeleteUserCommandResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        return Ok(true);
        return NotFound();

        return NoContent();
    }
}