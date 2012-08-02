using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace configtransformer
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = new List<string>(args);
            if(arguments.Count == 0)
            {
                Console.WriteLine("You must supply atleast the configuration you want. ");
                Console.WriteLine("Usage: configtransformer.exe config [Uses current directory]");
                Console.WriteLine("Usage: configtransformer.exe directory config ");
                return;
            }

            DirectoryInfo rootDirectory;

            if(arguments.Count == 1)
            {
                rootDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            }
            else
            {
                if (Directory.Exists(arguments[0]) == false)
                {
                    Console.WriteLine("Cannot find directory {0}.", arguments[0]);
                    return;
                }

                rootDirectory = new DirectoryInfo(arguments[0]);    
            }
            
            var config = arguments[1];

            var transformer = new ConfigurationTransformer(rootDirectory, config);
            Console.WriteLine("Starting transform in root directory {0} with configuration {1}", rootDirectory.FullName, config);
            transformer.Transform();
            
            Console.WriteLine("Completed transform...");
        }
    }
}
