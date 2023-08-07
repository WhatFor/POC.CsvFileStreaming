using Dapper;
using Microsoft.Data.SqlClient;

namespace POC.CsvFileStreaming.Services;

public class FileContentService : IDisposable
{
    private readonly string connectionString;
    private readonly SqlConnection _sqlConnection;
    
    private const string SqlQuery = "SELECT * FROM MOCK_DATA";

    public FileContentService(IConfiguration config)
    {
        connectionString = config.GetConnectionString("Default")!;
        _sqlConnection = new SqlConnection(connectionString);
    }
    
    public async IAsyncEnumerable<Model> GetData()
    {
        await foreach (var row in _sqlConnection.QueryUnbufferedAsync<Model>(SqlQuery))
        {
            yield return row;
        }
    }

    public void Dispose()
    {
        _sqlConnection.Dispose();
    }
}

public class Model
{
    public int Id { get; set; }
    
    public string? First_Name { get; set; }
    
    public string? Last_Name { get; set; }
    
    public string? Email { get; set; }
    
    public string? Gender { get; set; }
    
    public string? Ip_Address { get; set; }
}