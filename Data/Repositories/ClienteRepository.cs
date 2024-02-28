using Npgsql;
using Rinha_de_backend.Dtos;

namespace Rinha_de_backend.Data.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly NpgsqlConnection _connection;
    public ClienteRepository(RinhaDbContext context)
    {
        _connection = context.CreateConnection();
    }
    public async Task<Result<ClienteDto>> GetCliente(int clienteId)
    {
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

        return Result<ClienteDto>.Ok(cliente);
    }
}
