using BankApi;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// AddSingleton ensures ASP.NET Core creates
// it once and reuses that same instance for every request.

builder.Services.AddSingleton<Bank>();

// --- MongoDB wiring ---

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

const string FrontendCorsPolicy = "FrontendCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

app.UseCors(FrontendCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();