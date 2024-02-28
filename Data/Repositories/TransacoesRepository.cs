using Npgsql;
using Rinha_de_backend.Dtos;
using Rinha_de_backend.Exceptions;
using System.Collections.Generic;

namespace Rinha_de_backend.Data.Repositories;

public class TransacoesRepository : ITransacaoRepository
{
    private readonly NpgsqlConnection _connection;

    public TransacoesRepository(RinhaDbContext context)
    {
        _connection = _connection = context.CreateConnection();

    }

    public async Task<Result<ClienteDto>> AddTransacao(TransacaoDto transacao, int clienteId)
    {
        if ((transacao.Tipo != "d" && transacao.Tipo != "c") || transacao.Descricao.Length > 10)
        {
            return Result<ClienteDto>.Failure(new ClienteDto(0, 0), new Error(400, "erro"));
        }

        var command = _connection.CreateCommand();
        await _connection.OpenAsync();

        command.CommandText = "SELECT * from atualizaSaldo(@clienteId, @valor, @descricao);";
        command.Parameters.AddWithValue("clienteId", clienteId);
        command.Parameters.AddWithValue("valor", transacao.Tipo == "d" ? transacao.Valor * -1 : transacao.Valor);
        command.Parameters.AddWithValue("descricao", transacao.Descricao);

        var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if ((int)reader["result"] == 1)
            {
                return Result<ClienteDto>.Ok(new ClienteDto((int)reader["limitenovo"], (int)reader["saldoAtual"]));
            }

            if ((int)reader["result"] == -1)
            {
                return Result<ClienteDto>.Failure(new ClienteDto(0, 0), new Error(404, "Cliente nao encontrado"));

            }

            if ((int)reader["result"] == 0)
            {
                return Result<ClienteDto>.Failure(new ClienteDto(0, 0), new Error(422, "Saldo Insuficiente"));
            }
        }

        return Result<ClienteDto>.Failure(new ClienteDto(0, 0), new Error(400, "erro"));
    }

    public async Task<Result<List<UltimasTrasacoesDto>>> GetUltimasTrasacoes(int clienteId)
    {
        var command = _connection.CreateCommand();
        await _connection.OpenAsync();

        command.CommandText = "SELECT * FROM transacoes WHERE cliente_id = @cliente_id ORDER BY realizada_em DESC LIMIT 10";

        command.Parameters.AddWithValue("cliente_id", clienteId);

        var reader = await command.ExecuteReaderAsync();

        var ultimasTransacoes = new List<UltimasTrasacoesDto>();

        while (await reader.ReadAsync())
        {
            ultimasTransacoes.Add(new UltimasTrasacoesDto(
                reader["valor"] is not null ? (int)reader["valor"] : 0,
                reader["tipo"] as string,
                reader["descricao"] as string,
                reader["realizada_em"] is not null ? (DateTime)reader["realizada_em"] : DateTime.MinValue));
        }

        return Result<List<UltimasTrasacoesDto>>.Ok(ultimasTransacoes);
    }
}
