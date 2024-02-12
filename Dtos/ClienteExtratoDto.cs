namespace Rinha_de_backend.Dtos;

public class ClienteExtratoDto
{
    public ClienteExtratoDto(int total, int limite)
    {
        Total = total;
        Limite = limite;
        Data_Extrato = DateTime.Now;
    }

    public int Total { get; set; }
    public DateTime Data_Extrato { get; set; }
    public int Limite { get; set; }
}


