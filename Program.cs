using System.CommandLine;
using BackupBaseDeDados.Configs;
using BackupBaseDeDados.Utilidades.Backup;
using BackupBaseDeDados.Utilidades.Database;
using BackupBaseDeDados.Utilidades.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var rootCommand = new RootCommand("Database Backup Utility");
    var configOption = new Option<FileInfo>("--config", "Path to configuration file") { IsRequired = true };
    var commandOption = new Option<string>("--command", () => "backup", "Operation to perform (backup/restore)");

    rootCommand.AddOption(configOption);
    rootCommand.AddOption(commandOption);

    rootCommand.SetHandler(async (configFile, command) =>
    {
        await RunApplication(configFile, command);
    }, configOption, commandOption);

    await rootCommand.InvokeAsync(args);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

async Task RunApplication(FileInfo configFile, string command)
{
    if (!configFile.Exists)
        throw new FileNotFoundException("Configuration file not found", configFile.FullName);

    var host = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(config =>
        {
            config.AddJsonFile(configFile.FullName, optional: false)
                  .AddEnvironmentVariables();
        })
        .ConfigureServices((context, services) =>
        {
            services.Configure<DatabaseSettings>(context.Configuration.GetSection("Database"));
            services.Configure<StorageSettings>(context.Configuration.GetSection("Storage"));
            services.Configure<NotificationSettings>(context.Configuration.GetSection("Notifications"));

            services.AddSingleton<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
            services.AddSingleton<IBackupService, ResilientBackupService>();
            services.AddSingleton<INotificationService, SlackNotificationService>();
            services.AddTransient<IRetryPolicy>(_ => new ExponentialBackoffRetryPolicy(3));
        })
        .UseSerilog()
        .Build();

    ValidateConfiguration(host.Services.GetRequiredService<IConfiguration>());

    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;

    switch (command.ToLowerInvariant())
    {
        case "backup":
            await services.GetRequiredService<IBackupService>()
                .CreateBackupAsync("backup.sql");
            break;
        default:
            throw new ArgumentException($"Invalid command: {command}");
    }
}

void ValidateConfiguration(IConfiguration config)
{
    if (string.IsNullOrEmpty(config["Database:Type"]))
        throw new InvalidOperationException("Database configuration is invalid");
}