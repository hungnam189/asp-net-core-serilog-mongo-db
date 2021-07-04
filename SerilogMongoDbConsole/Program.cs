using SerilogMongoDbConsole.VulnerabilityDemo;
using System;
using System.Linq;

namespace SerilogMongoDbConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            VulnerabilityRuleDemo vulnerabilityRuleDemo = new VulnerabilityRuleDemo();

            const int TimeCheckOut = 1;
            var lstValid = new int[] { 1, 3, 4 };
            var result =  lstValid.Any(p => p == TimeCheckOut) ? 15
                    : TimeCheckOut == 2 ? 20
                    : 0;

            //var arg = args != null ? args[0] : "null";
            Console.WriteLine("arg:{0}", 0);
            Console.ReadLine();
        }
    }
}
