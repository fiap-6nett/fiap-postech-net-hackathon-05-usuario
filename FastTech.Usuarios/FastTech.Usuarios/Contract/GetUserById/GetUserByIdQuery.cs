namespace FastTech.Usuarios.Contract.GetUserById;

public class GetUserByIdQuery
{
    /// <summary>
    ///     Identificador único do cliente no sistema.
    ///     <para>Exemplo: <c>d290f1ee-6c54-4b01-90e6-d701748f0851</c></para>
    /// </summary>
    public Guid Id { get; set; }
}