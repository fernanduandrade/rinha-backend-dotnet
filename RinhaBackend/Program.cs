using System.Text.Json;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RinhaBackend;
using RinhaBackend.Handlers;

var builder = WebApplication.CreateBuilder(args);

string dbConnection = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, ContextSerializer.Default);
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});
var app = builder.Build();

Dictionary<char, string> operations = new()
{
    { 'c', "creditar" },
    { 'd', "debitar" }
};

app.MapGet("/clientes/{id}/extrato", async (int id) =>
{
    if (id is < 1 or > 5)
        return Results.NotFound();
    
    await using var connection = new NpgsqlConnection(dbConnection);
    await connection.OpenAsync();

    var saldo = await connection
        .QueryFirstAsync<SaldoResponse>("SELECT saldo as Total, limite FROM clients WHERE id = @Id",
            new
            {
                Id = id,
            });
    var transactions = await connection
    .QueryAsync<UltimaTransacaoResponse>("""
        SELECT valor, tipo, descricao, realizada_em as RealizadaEm FROM transactions WHERE cliente_id = @Id
        ORDER BY realizada_em DESC
        limit 10
        """, 
    new {
        Id = id
    });

    return Results.Ok(new ExtratoResponse(Saldo: saldo, UltimasTransacoes: transactions));
});


app.MapPost("/clientes/{id}/transacoes", async ([FromRoute] int id,[FromBody] TransactionRequest request) =>
{
    if (id is < 1 or > 5)
            return Results.NotFound();

    if (string.IsNullOrEmpty(request.Descricao)
        || request.Descricao.Length > 10
        || request.Valor % 1 != 0
        || request.Valor < 0
        || request.Tipo != 'c' && request.Tipo != 'd')
        return Results.UnprocessableEntity();
    
    await using var connection = new NpgsqlConnection(dbConnection);
    await connection.OpenAsync();

    int valor = Convert.ToInt32(request.Valor);
    var result = await connection
        .QueryFirstAsync<TransactionDbResponse>($"""
            select novo_saldo, limite_cliente, possui_erro, mensagem
            FROM {operations[request.Tipo]}(@ClientId, @Valor, @Descricao)
            """,
        new
        {
            ClientId = id,
            Valor = valor,
            Descricao = request.Descricao
        });

    if (result.Possui_Erro)
        return Results.UnprocessableEntity();

    return Results.Ok(new TransactionResponse(Limite: result.Limite_Cliente, Saldo: result.Novo_Saldo));
});

app.Run();


