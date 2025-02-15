using Npgsql;
using Rinha_de_backend.Dtos;

namespace Rinha_de_backend.Data.Repositories;

public interface ITransacaoRepository
{
    public Task<Result<ExtratoDto>> GetUltimasTrasacoes(int clienteId, NpgsqlConnection conn);
    public Task<Result<ClienteDto>> AddTransacao(TransacaoDto transacao, int clienteId, NpgsqlConnection conn);
}
