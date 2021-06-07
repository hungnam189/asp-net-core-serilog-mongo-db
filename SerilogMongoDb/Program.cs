using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System;

namespace SerilogMongoDb
{
    public class Program
    {
        private static readonly LoggerProviderCollection Providers = new LoggerProviderCollection();

        public static void Main(string[] args)
        {
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.MongoDB("mongodb://namth123:namth123@localhost:27017/serilog", collectionName: "logapi")
                        .WriteTo.Providers(Providers)
                        .CreateLogger();

            ///Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(providers: Providers)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
