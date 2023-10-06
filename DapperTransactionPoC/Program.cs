using DapperTransactionPoC;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(typeof(DatabaseService));

var app = builder.Build();

var databaseService = (DatabaseService)app.Services.GetRequiredService(typeof(DatabaseService));
databaseService.ExecuteScript();