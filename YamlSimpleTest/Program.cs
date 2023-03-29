using System;
using System.IO;
using YamlSimple;

namespace YamlSimpleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Parser.ParseFile(@"E:\Exchange\config.yml").Count);

            //Parser.UpdateFileStringValue(@"E:\Exchange\config.yml", "variables.instance_name", "test44");

            var check = Parser.CheckFileKey(@"C:\Temp\config.yml", "variables.instance_name");
            Parser.AddFileStringValue(@"C:\Temp\config.yml", "variables.instance_name", "test44");
            //Parser.AddFileStringValue(@"C:\Temp\config.yml", "services_config.SungeroHaproxy.instance_name", "test44");

            Console.WriteLine(Parser.ParseFile(@"E:\Exchange\config.yml").Count);
        }
    }
}
