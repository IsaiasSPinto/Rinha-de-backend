using Npgsql;

namespace Rinha_de_backend.Data;

public sealed class RinhaDbContext(IConfiguration configuration)
{
    private NpgsqlConnection _connection;
    private readonly NpgsqlDataSource dataSource =
            new NpgsqlSlimDataSourceBuilder(configuration.GetConnectionString("Rinha"))
            .Build();

    public NpgsqlConnection CreateConnection()
    {
        return dataSource.CreateConnection();
    }


}
