using Npgsql;
using Rinha_de_backend.Dtos;

namespace Rinha_de_backend.Data.Repositories;

public class TransacoesRepository : ITransacaoRepository
{
    public async Task<Result<ClienteDto>> AddTransacao(TransacaoDto transacao, int clienteId, NpgsqlDataSource dataSource)
    {
        if (transacao.Tipo != "d" && transacao.Tipo != "c")
        {
            return Result<ClienteDto>.Failure(new ClienteDto(0, 0), new Error(422, "erro"));
        }

        if (String.IsNullOrEmpty(transacao.Descricao) || transacao.Descricao.Length > 10)
        {
            return Result<ClienteDto>.Failure(new ClienteDto(0, 0), new Error(422, "erro"));
        }

        await using var conn = await dataSource.OpenConnectionAsync();

        var command = conn.CreateCommand();

        command.CommandText = "SELECT * from atualizaSaldo(@clienteId, @valor, @descricao);";
        command.Parameters.AddWithValue("clienteId", clienteId);
        command.Parameters.AddWithValue("valor", transacao.Tipo == "d" ? transacao.Valor * -1 : transacao.Valor);
        command.Parameters.AddWithValue("descricao", transacao.Descricao);

        var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if ((int)reader["result"] == 1)
            {
                await conn.CloseAsync();
                return Result<ClienteDto>.Ok(new ClienteDto((int)reader["limitenovo"], (int)reader["saldoAtual"]));
            }

            if ((int)reader["result"] == -1)
            {
                await conn.CloseAsync();
                return Result<ClienteDto>.Failure(new ClienteDto(0, 0), new Error(404, "Cliente nao encontrado"));

            }

            if ((int)reader["result"] == 0)
            {
                await conn.CloseAsync();
                return Result<ClienteDto>.Failure(new ClienteDto(0, 0), new Error(422, "Saldo Insuficiente"));
            }
        }

        await conn.CloseAsync();
        return Result<ClienteDto>.Failure(new ClienteDto(0, 0), new Error(400, "erro"));
    }

    public async Task<Result<ExtratoDto>> GetUltimasTrasacoes(int clienteId, NpgsqlDataSource dataSource)
    {
        if (clienteId < 1 || clienteId > 5)
        {
            return Result<ExtratoDto>.Failure(new ExtratoDto(new ClienteExtratoDto(0, 0), Enumerable.Empty<UltimasTrasacoesDto>().ToList()), new Error(404, "Cliente nao encontrado"));
        }
        await using var conn = await dataSource.OpenConnectionAsync();

        var command = conn.CreateCommand();

        command.CommandText = @"
            SELECT 
                * 
            FROM 
                transacoes INNER JOIN 
                clientes ON transacoes.cliente_id = clientes.id
            WHERE 
                cliente_id = @cliente_id 
            ORDER BY realizada_em DESC LIMIT 10";

        command.Parameters.AddWithValue("cliente_id", clienteId);

        var reader = await command.ExecuteReaderAsync();

        var ultimasTransacoes = new List<UltimasTrasacoesDto>();
        ClienteExtratoDto cliente = null;


        while (await reader.ReadAsync())
        {
            cliente ??= new ClienteExtratoDto(
                reader["saldo"] is not null ? (int)reader["saldo"] : 0,
                reader["limite"] is not null ? (int)reader["limite"] : 0);

            ultimasTransacoes.Add(new UltimasTrasacoesDto(
                reader["valor"] is not null ? (int)reader["valor"] : 0,
                reader["tipo"] as string,
                reader["descricao"] as string,
                reader["realizada_em"] is not null ? (DateTime)reader["realizada_em"] : DateTime.MinValue));
        }

        await conn.CloseAsync();


        return Result<ExtratoDto>.Ok(new ExtratoDto(cliente, ultimasTransacoes));
    }
}
