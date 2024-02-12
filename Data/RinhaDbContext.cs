using Npgsql;

namespace Rinha_de_backend.Data;

public class RinhaDbContext
{
    private readonly IConfiguration _configuration;
    public RinhaDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public NpgsqlConnection GetConnection()
    {
        var connectionString = _configuration.GetConnectionString("Rinha");
        var connection = new NpgsqlConnection(connectionString);
        return connection;
    }




}
