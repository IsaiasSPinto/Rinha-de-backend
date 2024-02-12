using Rinha_de_backend.Dtos;

namespace Rinha_de_backend.Data.Repositories;

public interface ITransacaoRepository
{
    public Task<List<UltimasTrasacoesDto>> GetUltimasTrasacoes(int clienteId);
    public Task<ClienteDto> AddTransacao(TransacaoDto transacao, int clienteId);
}
