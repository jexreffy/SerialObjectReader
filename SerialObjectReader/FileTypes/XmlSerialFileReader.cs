using Newtonsoft.Json;
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

        private XmlDocument _xmlDocument;

        private const string FILE_EXTENSION = ".xml";

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
            var xmlFile = string.Empty;

            using (var reader = new StreamReader(Filename))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();

                    if (line != null)
                    {
                        xmlFile += line;
                    }
                }
            }

            //Gate on if file is empty
            if (string.IsNullOrWhiteSpace(xmlFile))
            {
                Console.WriteLine($"Contents of {Filename} are empty.");
                return false;
            }

            //Parse xml file
            try
            {
                _xmlDocument = new XmlDocument();
                _xmlDocument.LoadXml(xmlFile);
                _xmlDocument.DocumentElement?.Normalize();

                Console.WriteLine($"Contents of {Filename} have been parsed successfully.");
                IsLoaded = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
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

            if (_xmlDocument.DocumentElement != null)
            {
                for (var i = 0; i < _xmlDocument.ChildNodes.Count; i++)
                {
                    if (_xmlDocument.ChildNodes[i] == null) continue;

                    retVal += await SearchNode(_xmlDocument.ChildNodes[i], hasKey, keyInProgress, currentKey, value);
                }
            }

            return retVal;
        }
        #endregion

        #region Private Methods
        /**
         * After determining that an XML node is valid to search,
         * either evaluate the XML node if it does not have any child nodes, or iterate through the child nodes.
         * Input: node - (XmlNode) the XML Node currently being evaluated.
         * Input: hasKey - (bool) true if a key was provided in the search term, false otherwise.
         * Input: keyInProgress - (bool) true if a multi-level key has started being searched, false otherwise.
         * Input: currentKey - (string) the current part of the search key being evaluated.
         * Input: value - (string) the value being searched for.
         * Returns: (int) the number of nodes that match the overall search term.
         */
        public async Task<int> SearchNode(XmlNode node, bool hasKey, bool keyInProgress, string currentKey, string value)
        {
            var retVal = 0;

            if (node.ChildNodes.Count == 1 && node.ChildNodes[0] != null && node.ChildNodes[0].Name.Equals("#text"))
            {
                if ((hasKey && currentKey.Equals(node.Name) && value.Equals(node.InnerText)) ||
                    (!hasKey && value.Equals(node.InnerText)))
                {
                    retVal++;
                    Console.WriteLine($"Found node that matches term.");
                    Console.WriteLine(node.ParentNode.OuterXml);
                }
            }
            else
            {
                var shouldEvaluate = !hasKey;
                var newCurrentKey = currentKey;

                if (currentKey.StartsWith($"{node.Name}."))
                {
                    shouldEvaluate = true;
                    newCurrentKey = currentKey[(node.Name.Length + 1)..];
                    keyInProgress = true;
                }
                else if (!keyInProgress)
                {
                    shouldEvaluate = true;
                }

                if (shouldEvaluate)
                {
                    for (var i = 0; i < node.ChildNodes.Count; i++)
                    {
                        if (node.ChildNodes[i] == null) continue;

                        retVal += await SearchNode(node.ChildNodes[i], hasKey, keyInProgress, newCurrentKey, value);
                    }
                }
            }

            return retVal;
        }

        #endregion
    }
}
