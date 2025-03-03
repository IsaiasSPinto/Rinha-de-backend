using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Rinha_de_backend;
using Rinha_de_backend.Data.Repositories;
using Rinha_de_backend.Dtos;
using System.Text.Json;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, JsonContextSerialization.Default);
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});


builder.Services.AddScoped<ITransacaoRepository, TransacoesRepository>();
NpgsqlDataSource dataSource = new NpgsqlSlimDataSourceBuilder(builder.Configuration.GetConnectionString("Rinha")).Build();

var app = builder.Build();

app.MapPost("clientes/{id}/transacoes", async (
    [FromServices] ITransacaoRepository transacaoRepository,
    int id,
    TransacaoDto transacao) =>
{
    var retry = 0;
    var conn = await dataSource.OpenConnectionAsync();
    try
    {
        while (retry <= 10)
        {
            try
            {

                var result = await transacaoRepository.AddTransacao(transacao, id, conn);

                if (!result.Success)
                {
                    return result.Error?.StatusCode switch
                    {
                        400 => Results.BadRequest(),
                        404 => Results.NotFound("Cliente n�o encontrado"),
                        422 => Results.UnprocessableEntity(),
                        _ => Results.BadRequest()
                    };
                }

                return Results.Ok(result.Data);

            }
            catch (NpgsqlException)
            {
                retry++;
            }
        }
    }
    catch (Exception)
    {

        return Results.UnprocessableEntity();
    }
    finally
    {
        await conn.CloseAsync();
    }

    return Results.UnprocessableEntity();

});




app.MapGet("clientes/{id}/extrato", async (
    [FromServices] ITransacaoRepository transacaoRepository,
    int id) =>
{
    var conn = await dataSource.OpenConnectionAsync();
    try
    {

        var result = await transacaoRepository.GetUltimasTrasacoes(id, conn);

        if (!result.Success)
        {
            return Results.NotFound("Cliente n�o encontrado");
        }


        return Results.Ok(result.Data);
    }
    catch (Exception)
    {
        return Results.BadRequest();
    }
    finally
    {
        await conn.CloseAsync();
    }
});

app.Run();