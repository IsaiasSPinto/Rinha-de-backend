namespace Rinha_de_backend.Dtos;

public class ClienteDto
{
    public ClienteDto(int limite, int saldo)
    {
        Limite = limite;
        Saldo = saldo;
    }

    public int Limite { get; set; }
    public int Saldo { get; set; }
}
