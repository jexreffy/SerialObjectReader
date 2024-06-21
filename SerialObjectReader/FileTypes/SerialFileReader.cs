using System;
namespace SerialObjectReader.FileTypes
{
    /**
     * SerialFileReader aggregates the common methods for interacting with multiple different file serialization types.
     */
    public abstract class SerialFileReader(string filename)
    {
        /**
         * The name of the file being parsed
         */
        public string Filename { get; private set; } = filename;
        /**
         * The file extension of the serialized file
         */
        public virtual string FileExtension => string.Empty;
        /**
         * Whether the file was successfully loaded and parsed.
         */
        public bool IsLoaded { get; protected set; }
        /**
         * Attempts to read and parse the serialized file provided.
         * Returns: (bool) true if successful, false otherwise
         */
        public abstract Task<bool> Parse();
        /**
         * Asynchronously attempts to search the file using a given search parameter.
         * Input: key - (string) the key provided to search the file, if no key was included, this will be null or empty.
         * Input: value - (string) the value provided to search the file, cannot be null or empty.
         * Returns: (int) the number of nodes that match the overall search term.
         */
        public abstract Task<int> Search(string key, string value);
    }
}
