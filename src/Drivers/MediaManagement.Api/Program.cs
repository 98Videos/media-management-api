using MediaManagement.Application.DependecyInjection;
using MediaManagement.Database.DependecyInjection;
using MediaManagement.S3.DependencyInjection;

using MediaManagement.Api.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Adicionando configura��es de banco de dados PostgreSQL
builder.Services.AddPostgresqlDatabase(builder.Configuration);
builder.Services.RunDatabaseMigrations(builder.Configuration);

// Adicionando o gerenciador de arquivos S3
builder.Services.AddS3FileManager(builder.Configuration);

// Adicionando o caso de uso do v�deo
builder.Services.AddVideoUseCase();

// Adicionando configura��es b�sicas para a aplica��o
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCognitoAuthentication(builder.Configuration);

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
app.MapControllers();

// Iniciando a aplica��o
app.Run();