namespace Rinha_de_backend.Data.Models;

public class Cliente
{
    public Cliente(int id, int limite, int saldoInicial)
    {
        Id = id;
        Limite = limite;
        Saldo = saldoInicial;
    }
    public int Id { get; set; }
    public int Limite { get; set; }
    public int Saldo { get; set; }

}
