using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System;
using System.Diagnostics;

namespace SerilogMongoDb
{
    public static class Program
    {
        private static readonly LoggerProviderCollection Providers = new LoggerProviderCollection();

        internal static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            const bool isReloadOnChange = false;
            var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: isReloadOnChange)
                        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: isReloadOnChange)
                        .Build();

            // "mongodb://namth123:namth123@localhost:27017/serilog"
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.MongoDB(
                            config.GetSection("MongoDbLogging:ConnectionString").Value,
                            collectionName: config.GetSection("MongoDbLogging:CollectionName").Value)
                        .WriteTo.Providers(Providers)
                        .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(providers: Providers)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
