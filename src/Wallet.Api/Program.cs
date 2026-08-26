using Microsoft.EntityFrameworkCore;
using Wallet.Domain.Contracts;
using Wallet.Infrastructure.Issuing;
using Wallet.Infrastructure.Persistence;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<WalletDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.Configure<IssuerOptions>(builder.Configuration.GetSection(IssuerOptions.Seccion));
builder.Services.AddScoped<ICredentialIssuer, HmacCredentialIssuer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Wallet API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();