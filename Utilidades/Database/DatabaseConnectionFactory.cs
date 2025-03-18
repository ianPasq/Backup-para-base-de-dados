using BackupBaseDeDados.Configs;

namespace BackupBaseDeDados.Utilidades.Database;

public interface IDatabaseConnectionFactory
{
    IDatabaseConnection Create(DatabaseSettings config);
}

public class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    public IDatabaseConnection Create(DatabaseSettings config)
    {
        return config.Type switch
        {
            "MySql" => new MySqlConnectionService(config.Host, config.DatabaseName, config.Username, config.Password),
            //"PostgreSql" => new PostgreSqlConnectionService(config.Host, config.DatabaseName, config.Username, config.Password),
            //"MongoDb" => new MongoDbConnectionService(
                //$"mongodb://{config.Username}:{config.Password}@{config.Host}",
               // config.DatabaseName),
            _ => throw new NotSupportedException($"Database type {config.Type} not supported")
        };
    }
}
// Melhorar suporte para postgresql, Mongodb ...