namespace Rinha_de_backend.Exceptions;

public class TransacaoInvalidaException : Exception
{
    public TransacaoInvalidaException() : base("Tipo de transação invalida")
    {
    }
}
