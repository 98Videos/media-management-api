using Amazon.S3;
using MediaManagement.Application.UseCases;
using MediaManagement.Application.UseCases.Interfaces;
using MediaManagement.Database.DependecyInjection;
using MediaManagement.Database.Repositories;
using MediaManagement.S3.Adapters;
using MediaManagement.S3.Options;
using MediaManagementApi.Domain.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSService<IAmazonS3>();
builder.Services.Configure<S3BucketOptions>(builder.Configuration.GetSection("S3BucketOptions"));
builder.Services.AddPostgresqlDatabase(builder.Configuration);
builder.Services.RunDatabaseMigrations(builder.Configuration);

builder.Services.AddScoped<IVideoUseCase, VideoUseCase>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();