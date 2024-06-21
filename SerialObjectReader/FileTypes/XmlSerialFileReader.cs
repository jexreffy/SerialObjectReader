using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SerialObjectReader.FileTypes
{
    /**
     * XmlSerialFileReader implements SerialFileReader for .xml files.
     */
    public class XmlSerialFileReader(string filename) : SerialFileReader(filename)
    {
        #region Variables

        private object? _parsedObject;

        private const string FILE_EXTENSION = ".xml";

        #endregion

        #region Fields

        public override string FileExtension => FILE_EXTENSION;

        #endregion

        #region Interface Methods

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

            using (var reader = new StreamReader(Filename))
            {
                using (var xmlReader = new XmlTextReader(reader))
                {

                }
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
            Console.WriteLine($"Search parameter is {key} and {value}");
            return 0;
        }
        #endregion
    }
}
