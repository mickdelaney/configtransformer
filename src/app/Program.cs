using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace Clarity.Util.ConfigTransformer
{
    class Program
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            var arguments = new List<string>(args);
            if(arguments.Count == 0)
            {
                Logger.Error("You must supply atleast the configuration you want. ");
                Logger.Error("Usage: configtransformer.exe config [Uses current directory]");
                Logger.Error("Usage: configtransformer.exe directory config ");
                Environment.Exit(1);
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
                    Logger.ErrorFormat("Cannot find directory {0}.", arguments[0]);
                    Environment.Exit(1);
                }

                rootDirectory = new DirectoryInfo(arguments[0]);    
            }
            
            var config = arguments[1];

            Logger.DebugFormat("Starting transform in root directory {0} with configuration {1}", rootDirectory.FullName, config);

            var transformer = new ConfigurationTransformer(rootDirectory, config);
            if (transformer.Transform() == false)
            {
                Environment.Exit(1);
            }

            Logger.DebugFormat("Completed transform...");

            Environment.Exit(0);
        }
    }
}
