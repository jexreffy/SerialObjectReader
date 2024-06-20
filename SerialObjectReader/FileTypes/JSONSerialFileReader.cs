using System;
using System.IO;
using Newtonsoft.Json;

namespace SerialObjectReader.FileTypes
{
    /*
     * JsonSerialFileReader implements SerialFileReader for .json files. Uses Newtonsoft.Json to interact with the json objects
     */
    public class JsonSerialFileReader(string filename) : SerialFileReader(filename)
    {
        #region Variables
        private object? _parsedObject;

        private const string FILE_EXTENSION = ".json";
        #endregion

        #region Fields
        public override string FileExtension => FILE_EXTENSION;
        #endregion

        #region Interface Methods
        /*
         * Asynchronously attempts to read and parse the serialized json file provided.
         * Returns: true if successful, false otherwise
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
            var jsonFile = string.Empty;

            using (var reader = new StreamReader(Filename))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();

                    if (line != null)
                    {
                        jsonFile += line;
                    }
                }
            }

            //Gate on if file is empty
            if (string.IsNullOrWhiteSpace(jsonFile))
            {
                Console.WriteLine($"Contents of {Filename} are empty.");
                return false;
            }

            //Parse json file
            var json = JsonConvert.DeserializeObject(jsonFile);

            if (json != null)
            {
                _parsedObject = json;
                Console.WriteLine($"Contents of {Filename} have been parsed successfully.");
                IsLoaded = true;
            }
            else
            {
                Console.WriteLine($"Contents of {Filename} are not valid json while having a .json extension.");
            }

            return IsLoaded;
        }

        /*
         * Asynchronously attempts to search the file using a given search parameter.
         * Input: term - (string) the parameters used to search the file.
         * Returns: true if successful, false otherwise
         */
        public override async Task<bool> Search(string term)
        {
            Console.WriteLine($"Search parameter is {term}");
            return false;
        }
        #endregion
    }
}
