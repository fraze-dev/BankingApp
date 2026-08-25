using BankApi;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// AddSingleton ensures ASP.NET Core creates
// it once and reuses that same instance for every request.
// Bank stays registered/seeded here because AuthController still runs
// against it — login/Admin haven't moved to MongoDB yet, that's a
// deliberately separate next step (see the session recap).
builder.Services.AddSingleton<Bank>();

// --- MongoDB wiring ---
// Reads MongoDb:ConnectionString from configuration — locally that comes
// from dotnet user-secrets (never from appsettings.json), so the real
// Atlas credentials never touch a git-tracked file.
builder.Services.AddSingleton<IMongoClient>(_ =>
{
    string connectionString = builder.Configuration["MongoDb:ConnectionString"];
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    IMongoClient client = sp.GetRequiredService<IMongoClient>();
    string databaseName = builder.Configuration["MongoDb:DatabaseName"] ?? "BankApiDb";
    return client.GetDatabase(databaseName);
});

builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();

var app = builder.Build();

// Seed the in-memory Bank (for Auth) and MongoDB (for Customers/Accounts)
// once, right after the app is built and before it starts handling requests.
using (var scope = app.Services.CreateScope())
{
    var bank = scope.ServiceProvider.GetRequiredService<Bank>();
    bank.SeedData();

    var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    await MongoSeedData.SeedAsync(database);
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