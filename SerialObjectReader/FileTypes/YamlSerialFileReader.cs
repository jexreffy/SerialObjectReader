using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SerialObjectReader.FileTypes
{
    /**
     * YamlSerialFileReader implements SerialFileReader for .yaml files.
     */
    public class YamlSerialFileReader(string filename) : SerialFileReader(filename)
    {
        #region Variables
        private object? _parsedObject;

        private const string FILE_EXTENSION = ".yml";

        #endregion

        #region Fields

        public override string FileExtension => FILE_EXTENSION;

        #endregion

        #region Inherited Methods
        /**
         * Asynchronously attempts to read and parse the serialized json file provided.
         * Returns: (bool) true if successful, false otherwise
         */
        public override async Task<bool> Parse()
        {
            IsLoaded = false;

            //Does file exists?
            if (!File.Exists(Filename))
            {
                Console.WriteLine($"File {Filename} does not exist.");
                return false;
            }

            //Read file asynchronously via StreamReader
            var yamlFile = string.Empty;

            using (var reader = new StreamReader(Filename))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();

                    if (line != null)
                    {
                        yamlFile += line;
                    }
                }
            }

            //Gate on if file is empty
            if (string.IsNullOrWhiteSpace(yamlFile))
            {
                Console.WriteLine($"Contents of {Filename} are empty.");
                return false;
            }

            //Parse yaml file
            try
            {
                var builder = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

                var yaml = builder.Deserialize(yamlFile);

                if (yaml != null)
                {
                    _parsedObject = yaml;
                    Console.WriteLine($"Contents of {Filename} have been parsed successfully.");
                    IsLoaded = true;
                } else
                {
                    Console.WriteLine($"Contents of {Filename} are not valid yaml while having a .yml extension.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return IsLoaded;
        }

        /**
         * Asynchronously attempts to search the file using a given search parameter.
         * Input: key - (string) the key provided to search the XML file, if no key was included, this will be null or empty.
         * Input: value - (string) the value provided to search the XML file, cannot be null or empty.
         * Returns: (int) the number of nodes that match the overall search term.
         */
        public override async Task<int> Search(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;

            var retVal = 0;

            var hasKey = !string.IsNullOrWhiteSpace(key);
            var keyInProgress = key.StartsWith("this.") || key.StartsWith("root.");
            var currentKey = keyInProgress ? key[5..] : key;

            return retVal;
        }
        #endregion
    }
}
