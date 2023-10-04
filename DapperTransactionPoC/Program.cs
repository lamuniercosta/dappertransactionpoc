using DapperTransactionPoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(typeof(DatabaseService));

var app = builder.Build();

var databaseService = (DatabaseService)app.Services.GetRequiredService(typeof(DatabaseService));
databaseService.ExecuteScript();