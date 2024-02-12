namespace Rinha_de_backend.Dtos;

public class UltimasTrasacoesDto
{
    public UltimasTrasacoesDto(int valor, string tipo, string descricao, DateTime dataRealizada)
    {
        Valor = valor;
        Tipo = tipo;
        Descricao = descricao;
        Realizada_em = dataRealizada;
    }

    public int Valor { get; set; }
    public string Tipo { get; set; }
    public string Descricao { get; set; }
    public DateTime Realizada_em { get; set; }

}
