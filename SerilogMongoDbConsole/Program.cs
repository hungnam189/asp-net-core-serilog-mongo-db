using MongoDB.Driver;
using System;
using System.Configuration;

namespace SerilogMongoDbConsole
{
    class Program
    {
        private static void Main(string[] args)
        {
            //var client = new MongoClient("mongodb://root:example@mongo:27017");
            var client = new MongoClient("mongodb://localhost:27017/Logging");
            _ = client.GetDatabase("Logging");

            Console.WriteLine("Hello World!");
        }
    }
}
