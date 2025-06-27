using KarizmaPlatform.Connection.Server.Extensions;
using Microsoft.OpenApi.Models;
using QuizoDotnet.Application.Extensions;
using QuizoDotnet.Extensions;
using QuizoDotnet.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureCors();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddKarizmaConnection();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API v1", Version = "v1" });
    options.AddKarizmaConnectionSwaggerDocs();
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddServices();

var app = builder.Build();
app.ConfigureCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapKarizmaConnectionHub("/Hub");

app.Run();