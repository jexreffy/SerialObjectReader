using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SerialObjectReader.FileTypes
{
    /**
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

        #region Inherited Methods
        /**
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
            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return IsLoaded;
        }

        /**
         * Asynchronously attempts to search the file using a given search parameter.
         * Input: key - (string) the key provided to search the JSON file, if no key was included, this will be null or empty.
         * Input: value - (string) the value provided to search the JSON file, cannot be null or empty.
         * Returns: (int) the number of nodes that match the overall search term.
         */
        public override async Task<int> Search(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;

            var retVal = 0;

            var hasKey = !string.IsNullOrWhiteSpace(key);
            var keyInProgress = key.StartsWith("this.") || key.StartsWith("root.");
            var currentKey = keyInProgress ? key[5..] : key;

            if (_parsedObject is JObject)
            {
                retVal += await SearchObject(_parsedObject as JObject, hasKey, keyInProgress, currentKey, value);
            }
            else if (_parsedObject is JArray)
            {
                retVal += await SearchArray(_parsedObject as JArray, hasKey, keyInProgress, currentKey, value);
            }

            return retVal;
        }
        #endregion

        #region Private Methods
        /**
         * Iterate through the indices of a JSON Array to search for the overall search term.
         * Input: node - (JArray) the JSON Array currently being evaluated.
         * Input: hasKey - (bool) true if a key was provided in the search term, false otherwise.
         * Input: keyInProgress - (bool) true if a multi-level key has started being searched, false otherwise.
         * Input: currentKey - (string) the current part of the search key being evaluated.
         * Input: value - (string) the value being searched for.
         * Returns: (int) the number of nodes that match the overall search term.
         */
        public async Task<int> SearchArray(JArray node, bool hasKey, bool keyInProgress, string currentKey, string value)
        {
            var retVal = 0;

            for (var i = 0; i < node.Count; i++)
            {
                var item = node[i];

                if (hasKey)
                {
                    if (currentKey.Equals(i.ToString()) && IsValidValue(item.Type) && DoesNodeMatchSearchValue(item, value))
                    {
                        retVal++;
                        Console.WriteLine($"Found node that matches term.");
                        Console.WriteLine(node.Parent.ToString());
                    }
                    else if (currentKey.StartsWith($"{i.ToString()}."))
                    {
                        var newKey = currentKey[(i.ToString().Length + 1)..];
                        retVal += await EvaluateNode(item, true, true, newKey, value);
                    }
                    else if (!keyInProgress)
                    {
                        retVal += await EvaluateNode(item, true, false, currentKey, value);
                    }
                }
                else
                {
                    retVal += await EvaluateNode(item, false, false, currentKey, value);
                }
            }

            return retVal;
        }

        /**
         * Iterate through the properties of a JSON Object to search for the overall search term.
         * Input: node - (JObject) the JSON Object currently being evaluated.
         * Input: hasKey - (bool) true if a key was provided in the search term, false otherwise.
         * Input: keyInProgress - (bool) true if a multi-level key has started being searched, false otherwise.
         * Input: currentKey - (string) the current part of the search key being evaluated.
         * Input: value - (string) the value being searched for.
         * Returns: (int) the number of nodes that match the overall search term.
         */
        public async Task<int> SearchObject(JObject node, bool hasKey, bool keyInProgress, string currentKey, string value)
        {
            if (!node.HasValues) return 0;

            var retVal = 0;

            foreach (var item in node.Properties())
            {
                if (hasKey)
                {
                    if (currentKey.Equals(item.Name) && IsValidValue(item.Value.Type) && DoesNodeMatchSearchValue(item.Value, value))
                    {
                        retVal++;
                        Console.WriteLine($"Found node that matches term.");
                        Console.WriteLine(node.Parent.ToString());
                    }
                    else if (currentKey.StartsWith($"{item.Name}."))
                    {
                        var newKey = currentKey[(item.Name.Length + 1)..];
                        retVal += await EvaluateNode(item.Value, true, true, newKey, value);
                    }
                    else if (!keyInProgress)
                    {
                        retVal += await EvaluateNode(item.Value, true, false, currentKey, value);
                    }
                }
                else
                {
                    retVal += await EvaluateNode(item.Value, false, false, currentKey, value);
                }
            }

            return retVal;
        }

        /**
         * After determining that a node is valid to search based on the key,
         * evaluate the node's type to determine how to process it in the search.
         * Input: node - (JToken) the JSON Node currently being evaluated.
         * Input: hasKey - (bool) true if a key was provided in the search term, false otherwise.
         * Input: keyInProgress - (bool) true if a multi-level key has started being searched, false otherwise.
         * Input: currentKey - (string) the current part of the search key being evaluated.
         * Input: value - (string) the value being searched for.
         * Returns: (int) the number of nodes that match the overall search term.
         */
        private async Task<int> EvaluateNode(JToken node, bool hasKey, bool keyInProgress, string currentKey, string value)
        {
            var retVal = 0;

            if (IsValidObject(node.Type))
            {
                retVal += await SearchObject(node as JObject, hasKey, keyInProgress, currentKey, value);
            }
            else if (IsValidArray(node.Type))
            {
                retVal += await SearchArray(node as JArray, hasKey, keyInProgress, currentKey, value);
            }
            else if (IsValidValue(node.Type) && !hasKey && DoesNodeMatchSearchValue(node, value))
            {
                retVal++;
                Console.WriteLine($"Found node that matches term.");
                Console.WriteLine(node.Parent.ToString());
            }

            return retVal;
        }
        #endregion

        #region Utility Methods
        /**
         * Determines if the node in question is a JSON Object node.
         * Input: nodeType - (JTokenType) the type of the node being evaluated.
         * Returns: (bool) true if type Object, false otherwise.
         */
        private static bool IsValidObject(JTokenType nodeType)
        {
            return nodeType == JTokenType.Object;
        }

        /**
         * Determines if the node in question is a JSON Array node.
         * Input: nodeType - (JTokenType) the type of the node being evaluated.
         * Returns: (bool) true if type Array, false otherwise.
         */
        private static bool IsValidArray(JTokenType nodeType)
        {
            return nodeType == JTokenType.Array;
        }

        /**
         * Determines if the node in question is a JSON Value node that can be evaluated.
         * Input: nodeType - (JTokenType) the type of the node being evaluated.
         * Returns: (bool) true if type can be evaluated, false otherwise.
         */
        private static bool IsValidValue(JTokenType nodeType)
        {
            return nodeType is not (JTokenType.Object or JTokenType.Array or JTokenType.None or JTokenType.Null or JTokenType.Undefined or JTokenType.Constructor);
        }

        /**
         * Determines if the Value Node matches the search value.
         * Assumes that IsValidValue is true on node being evaluated.
         * Input: node - (JToken) the node being evaluated.
         * Returns: (bool) true if node matches the value given, false otherwise.
         */
        private static bool DoesNodeMatchSearchValue(JToken node, string value)
        {
            switch (node.Type) {
                case JTokenType.Integer:
                    if (int.TryParse(value, out var intValue))
                    {
                        return intValue == node.Value<int>();
                    }
                    else
                    {
                        return false;
                    }
                case JTokenType.Float:
                    if (int.TryParse(value, out var floatValue))
                    {
                        return Math.Abs(floatValue - node.Value<float>()) < 1e-5;
                    }
                    else
                    {
                        return false;
                    }
                case JTokenType.Boolean:
                    return node.Value<bool>() == value.ToLowerInvariant().Equals("true");
                    break;
                case JTokenType.String:
                case JTokenType.Date:
                case JTokenType.Raw:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                    return node.Value<string>()?.Equals(value) ?? false;
                default:
                    return false;
            }
        }
        #endregion
    }
}
