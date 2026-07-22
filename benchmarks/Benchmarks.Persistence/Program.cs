using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Dapper;
using Npgsql;

namespace Benchmarks.Persistence;

public class CustomerRepositoryBenchmarks
{
    private readonly string _connectionString = "Host=localhost;Port=5432;Database=unitdb;Username=postgres;Password=postgres";

    [GlobalSetup]
    public void Setup()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS benchmarks_customers (
                id UUID PRIMARY KEY,
                name VARCHAR(150) NOT NULL,
                email VARCHAR(150) NOT NULL,
                document VARCHAR(50) NOT NULL,
                created_at TIMESTAMP NOT NULL
            );
        ");
    }

    [Benchmark]
    public async Task InsertCustomer()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(@"
            INSERT INTO benchmarks_customers(id, name, email, document, created_at)
            VALUES(@Id, @Name, @Email, @Document, @CreatedAt)",
            new
            {
                Id = Guid.NewGuid(),
                Name = "Bench",
                Email = "bench@email.com",
                Document = "123",
                CreatedAt = DateTime.UtcNow
            });
    }

    [Benchmark]
    public async Task QueryCustomer()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await connection.QuerySingleOrDefaultAsync<dynamic>(@"
            SELECT id, name, email, document, created_at
            FROM benchmarks_customers
            ORDER BY created_at DESC
            LIMIT 1");
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<CustomerRepositoryBenchmarks>();
    }
}

