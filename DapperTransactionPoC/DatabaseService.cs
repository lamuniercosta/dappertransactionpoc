using System.Data;
using System.Transactions;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DapperTransactionPoC;

public class DatabaseService
{
    public int CountTransactionsFirstConnection { get; private set; }
    public int CountTransactionsSecondConnection { get; private set; }
    
    private readonly string _primaryConnectionString;
    private readonly string _secondaryConnectionString;
    
    public DatabaseService(IConfiguration configuration)
    {
        _primaryConnectionString = configuration.GetConnectionString("PrimaryConnection")!;
        _secondaryConnectionString = configuration.GetConnectionString("SecondaryConnection")!;
    }

    private IDbConnection PrimaryDbConnection()
        => new SqlConnection(_primaryConnectionString);

    private IDbConnection SecondaryDbConnection()
        => new SqlConnection(_secondaryConnectionString);

    public void ExecuteScript()
    {
        try
        {
            using var transaction = new TransactionScope();
            
            using var firstConnection = PrimaryDbConnection();
            const string firstCommand = "insert into PrimaryTable(Name) values (@Name);";
            firstConnection.Execute(firstCommand, new { Name = "TestValue" });
            
            using var secondConnection = SecondaryDbConnection();
            const string secondCommand = "insert into SecondaryTable(Name) values (@Name);";
            secondConnection.Execute(secondCommand, new { Name = "TestValue" });

            CountTransactionsFirstConnection =
                firstConnection
                    .QuerySingle<int>("select @@trancount");

            CountTransactionsSecondConnection =
                secondConnection
                    .QuerySingle<int>("select @@trancount");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}