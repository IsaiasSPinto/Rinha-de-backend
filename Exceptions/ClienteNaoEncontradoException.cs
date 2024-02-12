namespace Rinha_de_backend.Exceptions;

public class ClienteNaoEncontradoException : Exception
{
    public ClienteNaoEncontradoException() : base("Cliente nao encontrado")
    {
    }
}
