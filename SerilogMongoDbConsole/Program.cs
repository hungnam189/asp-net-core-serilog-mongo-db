using SerilogMongoDbConsole.ConsumerKafka;
using SerilogMongoDbConsole.Infrastructure;
using SerilogMongoDbConsole.VulnerabilityDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerilogMongoDbConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string brokerServers = "Kafka-1:19092,Kafka-2:29092,Kafka-3:39092";
            var topics = new List<string>() { "dev-partnerora-webapp-infor", "dev-partnerora-webapp-error" };

            TestConsumer.RunConsumer(brokerServers, topics, new System.Threading.CancellationToken());

            const int objid = 12334;
            StringBuilder builder = new StringBuilder();
            builder.Append("http://trinhhungnam.com.vn/");
            builder.Append("GenFile/GetInfo?");
            builder.Append("ObjId=").Append(objid).Append("&TemplateId=2&Source=member");

            Console.WriteLine(builder.ToString());

            Console.WriteLine("arg:{0}", args != null ? args[0] : "0") ;
            Console.ReadLine();
        }
    }
}
