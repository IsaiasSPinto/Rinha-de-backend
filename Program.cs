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
    try
    {
        var result = await transacaoRepository.AddTransacao(transacao, id, dataSource);

        if (!result.Success)
        {
            return result.Error?.StatusCode switch
            {
                400 => Results.BadRequest(),
                404 => Results.NotFound("Cliente não encontrado"),
                422 => Results.UnprocessableEntity(),
                _ => Results.BadRequest()
            };
        }

        return Results.Ok(result.Data);

    }
    catch (Exception ex)
    {
        return Results.BadRequest();
    }
});


app.MapGet("clientes/{id}/extrato", async (
    [FromServices] ITransacaoRepository transacaoRepository,
    int id) =>
{
    var result = await transacaoRepository.GetUltimasTrasacoes(id, dataSource);

    if (!result.Success)
    {
        return Results.NotFound("Cliente não encontrado");
    }


    return Results.Ok(result.Data);
});

app.Run();