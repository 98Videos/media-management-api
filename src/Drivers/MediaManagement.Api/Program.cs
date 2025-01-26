using MediaManagement.Application.DependecyInjection;
using MediaManagement.Database.DependecyInjection;
using MediaManagement.S3.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Adicionando configurações de banco de dados PostgreSQL
builder.Services.AddPostgresqlDatabase(builder.Configuration);
builder.Services.RunDatabaseMigrations(builder.Configuration);

// Adicionando o gerenciador de arquivos S3
builder.Services.AddS3FileManager(builder.Configuration);

// Adicionando o caso de uso do vídeo
builder.Services.AddVideoUseCase();

// Adicionando configurações básicas para a aplicação
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configurando middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthorization();
app.MapControllers();

// Iniciando a aplicação
app.Run();