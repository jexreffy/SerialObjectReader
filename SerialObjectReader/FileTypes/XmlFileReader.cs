using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SerialObjectReader.FileTypes
{
    /*
     * XmlSerialFileReader implements SerialFileReader for .xml files.
     */
    public class XmlFileReader(string filename) : SerialFileReader(filename)
    {
        #region Variables

        private object? _parsedObject;

        private const string FILE_EXTENSION = ".xml";

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

            using (var reader = new StreamReader(Filename))
            {
                using (var xmlReader = new XmlTextReader(reader))
                {

                }
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
