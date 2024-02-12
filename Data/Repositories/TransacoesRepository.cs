using Npgsql;
using Rinha_de_backend.Dtos;
using Rinha_de_backend.Exceptions;

namespace Rinha_de_backend.Data.Repositories;

public class TransacoesRepository : ITransacaoRepository
{
    private readonly NpgsqlConnection _connection;

    public TransacoesRepository(RinhaDbContext context)
    {
        _connection = context.GetConnection();
    }

    public async Task<ClienteDto> AddTransacao(TransacaoDto transacao, int clienteId)
    {
        if (transacao.Tipo != "d" && transacao.Tipo != "c")
        {
            throw new TransacaoInvalidaException();
        }

        await _connection.OpenAsync();

        var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM clientes WHERE id = @id";
        command.Parameters.AddWithValue("id", clienteId);

        var reader = await command.ExecuteReaderAsync();

        ClienteDto cliente = null;

        while (await reader.ReadAsync())
        {
            cliente = new ClienteDto(
                reader["limite"] is not null ? (int)reader["limite"] : 0,
                reader["saldo"] is not null ? (int)reader["saldo"] : 0);
        }

        if (cliente is null)
        {
            throw new ClienteNaoEncontradoException();
        }

        cliente.Saldo -= transacao.Valor;

        if (transacao.Tipo == "d" && (cliente.Saldo * -1) < cliente.Limite)
        {
            throw new SaldoInsuficienteException();
        }

        var insertTransaction = _connection.CreateCommand();
        insertTransaction.CommandText = "INSERT INTO transacoes (valor, tipo, descricao, cliente_id,realizada_em) VALUES (@valor, @tipo, @descricao, @cliente_id,@realizada_em)";

        insertTransaction.Parameters.AddWithValue("valor", transacao.Valor);
        insertTransaction.Parameters.AddWithValue("tipo", transacao.Tipo);
        insertTransaction.Parameters.AddWithValue("descricao", transacao.Descricao);
        insertTransaction.Parameters.AddWithValue("cliente_id", clienteId);
        insertTransaction.Parameters.AddWithValue("realizada_em", DateTime.Now);

        await insertTransaction.ExecuteNonQueryAsync();


        var updateSaldo = _connection.CreateCommand();
        updateSaldo.CommandText = "UPDATE clientes SET saldo = @valor WHERE cliente_id = @id";
        updateSaldo.Parameters.AddWithValue("valor", cliente.Saldo);
        updateSaldo.Parameters.AddWithValue("id", clienteId);

        await updateSaldo.ExecuteNonQueryAsync();

        return cliente;
    }

    public async Task<List<UltimasTrasacoesDto>> GetUltimasTrasacoes(int clienteId)
    {
        await _connection.OpenAsync();

        var command = _connection.CreateCommand();

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

        return ultimasTransacoes;
    }
}
