using System;
using System.IO;
using System.Xml;
using Microsoft.Web.XmlTransform;
using log4net;

namespace Clarity.Util.ConfigTransformer
{
    /// Transforms file with .config extensions using an override file specified by environment
    /// <example>web.config -> web.prod.config</example>    
    public class ConfigurationTransformer
    {
        protected readonly ILog Logger = LogManager.GetLogger(typeof(ConfigurationTransformer));

        readonly DirectoryInfo _rootDirectory;
        readonly string _environmentName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootDirectory"></param>
        /// <param name="environmentName"></param>
        public ConfigurationTransformer(DirectoryInfo rootDirectory, string environmentName)
        {
            _rootDirectory = rootDirectory;
            _environmentName = environmentName;
        }

        /// <summary>
        /// Look for each {CFG}.config file and transform it with a {CFG}.{ENV}.config
        /// </summary>
        public bool Transform()
        {
            Console.WriteLine("Transforming application located at: {0} for environment: {1}", _rootDirectory, _environmentName);

            foreach (var configSettingsFile in _rootDirectory.EnumerateFiles("*.config", SearchOption.AllDirectories))
            {
                if (configSettingsFile.Directory == null)
                {
                    Logger.ErrorFormat("Not a directory at: '{0}'", configSettingsFile.FullName);
                    return false;
                }

                var environmentOverrideFileName = string.Format("{0}.{1}.config", Path.GetFileNameWithoutExtension(configSettingsFile.FullName), _environmentName);
                var environmentOverrideFile = Path.Combine(configSettingsFile.Directory.FullName, environmentOverrideFileName);
                if (File.Exists(environmentOverrideFile) == false)
                {
                    continue;
                }

                Console.WriteLine("Transforming cfg file: {0} with override file: {1}", configSettingsFile.FullName, environmentOverrideFile);

                if (Transform(configSettingsFile.FullName, configSettingsFile.FullName, environmentOverrideFile) == false)
                {
                    return false;                    
                }
            }

            return true;
        }

        public bool Transform(string sourceFile, string destinationFile, string transformFile)
        {
            using (var document = new XmlTransformableDocument())
            {
                document.PreserveWhitespace = true;
                document.Load(sourceFile);

                using (var transform = new XmlTransformation(transformFile))
                {
                    var success = transform.Apply(document);
                    document.Save(destinationFile);

                    if (success == false)
                    {
                        Logger.ErrorFormat("\nCould not save transformation at '{0}'\n\n", new FileInfo(destinationFile).FullName);
                    }
                    else
                    {
                        Logger.DebugFormat("\nSaved transformation at '{0}'\n\n", new FileInfo(destinationFile).FullName);
                    }

                    return success;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="transform"></param>
        public void TransformFile(string source, string destination, string transform)
        {
            var xmlTarget = OpenSourceFile(source);
            var transformFile = OpenTransformFile(transform);
            transformFile.Apply(xmlTarget);
            xmlTarget.Save(destination);

            //var transformXml = new TransformXml {Destination = destination, Source = source, Transform = transform};
            //transformXml.Execute();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        public XmlTransformableDocument OpenSourceFile(string sourceFile)
        {
            var xml = File.ReadAllText(sourceFile);
            var document = new XmlTransformableDocument { PreserveWhitespace = true };
            document.LoadXml(xml);
            return document;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformFile"></param>
        /// <returns></returns>
        public XmlTransformation OpenTransformFile(string transformFile)
        {
            XmlTransformation transformation;

            try
            {
                transformation = new XmlTransformation(transformFile);
            }
            catch (XmlException exception)
            {
                throw exception;
            }
            catch (Exception exception2)
            {
                throw;
            }
            return transformation;
        }
    }
}
