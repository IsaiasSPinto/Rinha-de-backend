namespace Rinha_de_backend.Data.Models;

public class Transacao
{
    public Transacao(int id, int valor, string tipo, string descricao, DateTime realizada_em)
    {
        Id = id;
        Valor = valor;
        Tipo = tipo;
        Descricao = descricao;
        Realizada_em = realizada_em;
    }

    public int Id { get; set; }
    public int Valor { get; set; }
    public string Tipo { get; set; }
    public string Descricao { get; set; }
    public DateTime Realizada_em { get; set; }

}
