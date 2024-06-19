using System;
using System.IO;
using Newtonsoft.Json;

namespace SerialObjectReader.FileTypes
{
    public class JSONSerialFileReader : ISerialFileReader
    {
        private object parsedObject;

        private const string FILE_EXTENSION = ".json";

        #region Constructors
        public JSONSerialFileReader(string filename)
        {
            Filename = filename;
        }
        #endregion

        #region Properties
        public string Filename { get; private set; }
        public string FileExtension { get { return FILE_EXTENSION; } }
        #endregion

        #region Methods
        public bool Parse()
        {
            if (!File.Exists(Filename))
            {
                Console.WriteLine($"File {Filename} does not exist.");
                return false;
            }

            var jsonFile = File.ReadAllText(Filename);

            if (string.IsNullOrWhiteSpace(jsonFile))
            {
                Console.WriteLine($"Contents of {Filename} are empty.");
                return false;
            }

            var json = JsonConvert.DeserializeObject(jsonFile);

            if (json != null)
            {
                parsedObject = json;
                Console.WriteLine($"Contents of {Filename} have been parsed successfully.");
                return true;
            }
            else
            {
                Console.WriteLine($"Contents of {Filename} are not valid json while having a .json extension.");
                return false;
            }
        }
        #endregion
    }
}
