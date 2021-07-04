using SerilogMongoDbConsole.ConsumerKafka;
using SerilogMongoDbConsole.VulnerabilityDemo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SerilogMongoDbConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var brokerServers = "Kafka-1:19092,Kafka-2:29092,Kafka-3:39092";
            var topics = new List<string>() { "dev-partnerora-webapp-infor", "dev-partnerora-webapp-error" };

            TestConsumer.RunConsumer(brokerServers, topics, new System.Threading.CancellationToken());

            Console.WriteLine("arg:{0}", args != null ? args[0] : "0") ;
            Console.ReadLine();
        }
    }
}
