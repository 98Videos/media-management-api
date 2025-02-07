using MediaManagement.Application.DependecyInjection;
using MediaManagement.Database.DependecyInjection;
using MediaManagement.S3.DependencyInjection;
using MediaManagement.Api.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;
using MediaManagement.SQS.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Adicionando configuracoes de banco de dados PostgreSQL
builder.Services.AddPostgresqlDatabase(builder.Configuration);
builder.Services.RunDatabaseMigrations(builder.Configuration);

// Adiciona health check na aplicação
builder.Services
    .AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new Exception("No connection string configured!"));

// Configura limite de requisições por segundo
builder.Services.ConfigureRateLimiting();

// Adicionando o gerenciador de arquivos S3
builder.Services.AddS3FileManager(builder.Configuration);

// Adicionando o caso de uso do video
builder.Services.AddVideoUseCase();

// Adicionando configuracoes basicas para a aplicacao
builder.Services.AddControllers();

// Adiciona authorizacao do cognito
builder.Services.AddCognitoAuthentication(builder.Configuration);

// Adiciona serviço de mensageria do sqs
builder.Services.AddSqsMessagePublisher(builder.Configuration);

builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen();

// Configura Enums para serem retornadas como text nas controllers
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Aumenta limites de tamanho da request
// Necessário para aceitar videos maiores no upload
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});

// Configurando CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.MapHealthChecks("/health");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

var controllerMapper = app.MapControllers();

// Iniciando a aplicao
app.Run();