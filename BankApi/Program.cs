using BankApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// AddSingleton ensures ASP.NET Core creates
// it once and reuses that same instance for every request.
builder.Services.AddSingleton<Bank>();

var app = builder.Build();

// Seed the Bank with starting data once, right after the app is built
// and before it starts handling requests.
using (var scope = app.Services.CreateScope())
{
    var bank = scope.ServiceProvider.GetRequiredService<Bank>();
    bank.SeedData();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
