using MediaManagement.Application.DependecyInjection;
using MediaManagement.Database.DependecyInjection;
using MediaManagement.S3.DependencyInjection;

using MediaManagement.Api.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Adicionando configuracoes de banco de dados PostgreSQL
builder.Services.AddPostgresqlDatabase(builder.Configuration);
builder.Services.RunDatabaseMigrations(builder.Configuration);

// Adicionando o gerenciador de arquivos S3
builder.Services.AddS3FileManager(builder.Configuration);

// Adicionando o caso de uso do v�deo
builder.Services.AddVideoUseCase();

// Adicionando configuracoes basicas para a aplicacao
builder.Services.AddControllers();

// Adiciona authorizacao do cognito
builder.Services.AddCognitoAuthentication(builder.Configuration, builder.Environment);

builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen();

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthorization();

var controllerMapper = app.MapControllers();

if (app.Environment.IsDevelopment())
    controllerMapper.AllowAnonymous();

// Iniciando a aplicao
app.Run();