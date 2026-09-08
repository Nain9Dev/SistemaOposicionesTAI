using Npgsql;

namespace Oposiciones.Infrastructure.Data;

/// <summary>
/// Implementacion sobre <see cref="NpgsqlDataSource"/>: se registra como singleton para que el
/// pool de conexiones y la cache de sentencias preparadas se reutilicen entre peticiones.
/// </summary>
public sealed class NpgsqlConnectionFactory : IDbConnectionFactory, IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlConnectionFactory(string connectionString)
    {
        var normalized = PostgresConnectionString.Normalize(connectionString);
        var builder = new NpgsqlDataSourceBuilder(normalized);

        // Los valores de parametro pueden contener credenciales o datos personales: nunca se registran.
        builder.EnableParameterLogging(false);

        _dataSource = builder.Build();
    }

    public ValueTask<NpgsqlConnection> OpenAsync(CancellationToken cancellationToken = default) =>
        _dataSource.OpenConnectionAsync(cancellationToken);

    public ValueTask DisposeAsync() => _dataSource.DisposeAsync();
}
