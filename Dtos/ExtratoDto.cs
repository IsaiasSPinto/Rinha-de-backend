namespace Rinha_de_backend.Dtos;

public class ExtratoDto
{
    public ExtratoDto(ClienteExtratoDto saldo, List<UltimasTrasacoesDto> ultimasTrasacoes)
    {
        Saldo = saldo;
        UltimasTrasacoes = ultimasTrasacoes;
    }

    public ClienteExtratoDto Saldo { get; set; }
    public List<UltimasTrasacoesDto> UltimasTrasacoes { get; set; }
}

