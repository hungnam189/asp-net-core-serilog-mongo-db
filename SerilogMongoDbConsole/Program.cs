using MongoDB.Driver;
using System;
using System.Configuration;

namespace SerilogMongoDbConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            //var client = new MongoClient("mongodb://root:example@mongo:27017");
            var client = new MongoClient("mongodb://localhost:27017/Logging");
            _ = client.GetDatabase("Logging");

            var arg = args[0];
            Console.WriteLine("arg:{0}", arg);
        }
    }
}
