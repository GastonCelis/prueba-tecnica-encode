using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using Wallet.Domain.Contracts;
using Wallet.Infrastructure.Issuing;
using Wallet.Infrastructure.Persistence;
using Wallet.Domain.Services;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<WalletDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.Configure<IssuerOptions>(builder.Configuration.GetSection(IssuerOptions.Seccion));
builder.Services.AddScoped<ICredentialIssuer, HmacCredentialIssuer>();
builder.Services.AddScoped<ICredentialRepository, CredentialRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<EmitirCredencialService>();
builder.Services.AddScoped<ListarCredencialesService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Wallet API v1");
    });
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();