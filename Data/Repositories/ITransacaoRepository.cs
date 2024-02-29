using Npgsql;
using Rinha_de_backend.Dtos;

namespace Rinha_de_backend.Data.Repositories;

public interface ITransacaoRepository
{
    public Task<Result<ExtratoDto>> GetUltimasTrasacoes(int clienteId, NpgsqlDataSource dataSource);
    public Task<Result<ClienteDto>> AddTransacao(TransacaoDto transacao, int clienteId, NpgsqlDataSource dataSource);
}
