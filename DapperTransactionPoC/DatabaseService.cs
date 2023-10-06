using System;
using System.Data;
using System.Transactions;
using Dapper;
using Dapper.Transaction;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DapperTransactionPoC;

public class DatabaseService
{
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
        using var firstConnection = PrimaryDbConnection();
        firstConnection.Open();
        using var firstTransaction = firstConnection.BeginTransaction();
        
        using var secondConnection = SecondaryDbConnection();
        secondConnection.Open();
        using var secondTransaction = secondConnection.BeginTransaction();
        
        try
        {
            const string firstCommand = "insert into PrimaryTable(Name) values (@Name);";
            firstTransaction.Execute(firstCommand, new { Name = "TestValue" });
            
            try
            {
                const string secondCommand = "insert into SecondTable(Name) values (@Name);";
                secondTransaction.Execute(secondCommand, new { Name = "TestValue" });
                
                firstTransaction.Commit();
                secondTransaction.Commit();
            }
            catch (SqlException e)
            {
                firstTransaction.Rollback();
                secondTransaction.Rollback();
                Console.WriteLine(e);
                throw;
            }
        }
        catch (SqlException e)
        {
            if (firstTransaction.Connection != null)
            {
                firstTransaction.Rollback();
            }
            if (secondTransaction.Connection != null)
            {
                secondTransaction.Rollback();
            }
            Console.WriteLine(e);
            throw;
        }
    }
}