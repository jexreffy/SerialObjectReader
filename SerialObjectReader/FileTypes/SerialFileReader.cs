using System;
namespace SerialObjectReader.FileTypes
{
    /*
     * SerialFileReader aggregates the common methods for interacting with multiple different file serialization types.
     */
    public abstract class SerialFileReader(string filename)
    {
        /*
         * The name of the file being parsed
         */
        public string Filename { get; private set; } = filename;
        /*
         * The file extension of the serialized file
         */
        public virtual string FileExtension => string.Empty;
        /*
         * Whether the file was successfully loaded and parsed.
         */
        public bool IsLoaded { get; protected set; }
        /*
         * Attempts to read and parse the serialized file provided.
         * Returns: true if successful, false otherwise
         */
        public abstract Task<bool> Parse();
        /*
         * Attempts to search the file using a given search parameter.
         * Input: term - (string) the parameters used to search the file.
         * Returns: true if successful, false otherwise
         */
        public abstract Task<bool> Search(string term);
    }
}
