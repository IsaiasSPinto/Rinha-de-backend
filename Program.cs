using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Rinha_de_backend;
using Rinha_de_backend.Data;
using Rinha_de_backend.Data.Repositories;
using Rinha_de_backend.Dtos;
using Rinha_de_backend.Exceptions;
using System.Text.Json;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, JsonContextSerialization.Default);
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddScoped<RinhaDbContext>();

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacoesRepository>();


var app = builder.Build();

app.MapPost("clientes/{id}/transacoes", async (
    [FromServices] ITransacaoRepository transacaoRepository,
    [FromServices] IClienteRepository clienteRepository,
    int id,
    TransacaoDto transacao) =>
{
    try
    {
        var result = await transacaoRepository.AddTransacao(transacao, id);
        return Results.Ok(result);
    }
    catch (TransacaoInvalidaException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (ClienteNaoEncontradoException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (SaldoInsuficienteException ex)
    {
        return Results.UnprocessableEntity(ex.Message);
    }
    catch (Exception)
    {
        return Results.BadRequest("Occoreu um erro ao adicionar a transacao.");
    }
});


app.MapGet("clientes/{id}/extrato", async (
    [FromServices] ITransacaoRepository transacaoRepository,
    [FromServices] IClienteRepository clienteRepository,
    int id) =>
{
    var cliente = await clienteRepository.GetCliente(id);

    if (cliente is null)
    {
        return Results.NotFound("Cliente não encontrado");
    }

    var trasacoes = await transacaoRepository.GetUltimasTrasacoes(id);

    return Results.Ok(new ExtratoDto(new ClienteExtratoDto(cliente.Saldo, cliente.Limite), trasacoes));
});

app.Run();