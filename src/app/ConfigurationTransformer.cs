using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Web.Publishing.Tasks;

namespace configtransformer
{
    /// Transforms file with .config extensions using an override file specified by environment
    /// <example>web.config -> web.prod.config</example>    
    /// </summary>
    public class ConfigurationTransformer
    {
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
        public void Transform()
        {
            Console.WriteLine("Transforming application located at: {0} for environment: {1}", _rootDirectory, _environmentName);

            foreach (var configSettingsFile in _rootDirectory.EnumerateFiles("*.config", SearchOption.AllDirectories))
            {
                var environmentOverrideFileName = string.Format("{0}.{1}.config", Path.GetFileNameWithoutExtension(configSettingsFile.FullName), _environmentName);
                var environmentOverrideFile = Path.Combine(configSettingsFile.Directory.FullName, environmentOverrideFileName);
                if (File.Exists(environmentOverrideFile) == false)
                {
                    continue;
                }

                Console.WriteLine("Transforming cfg file: {0} with override file: {1}", configSettingsFile.FullName, environmentOverrideFile);

                TransformFile(configSettingsFile.FullName, configSettingsFile.FullName, environmentOverrideFile);
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
