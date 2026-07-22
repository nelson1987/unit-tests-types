using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using UnitTestsTypes.Domain.Entities;
using UnitTestsTypes.Domain.Repositories;

namespace UnitTestsTypes.Infrastructure.Repositories;

public class DapperCustomerRepository : ICustomerRepository
{
    private readonly string _connectionString;

    public DapperCustomerRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres") ?? "Host=localhost;Port=5432;Database=unitdb;Username=postgres;Password=postgres";
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        const string sql = "SELECT id, name, email, document, created_at FROM customers WHERE id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Customer>(sql, new { Id = id });
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        const string sql = "SELECT id, name, email, document, created_at FROM customers ORDER BY created_at DESC";
        var result = await connection.QueryAsync<Customer>(sql);
        return result.ToList();
    }

    public async Task<Guid> AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        const string sql = @"
            INSERT INTO customers (id, name, email, document, created_at)
            VALUES (@Id, @Name, @Email, @Document, @CreatedAt)
            RETURNING id";
        return await connection.ExecuteScalarAsync<Guid>(sql, customer);
    }
}
