using System;
namespace SerialObjectReader.FileTypes
{
    /*
     * ISerialFileReader aggregates the common methods for interacting with multiple different file serialization types.
     */
    public interface ISerialFileReader
    {
        /*
         * The name of the file being parsed
         */
        public string Filename { get; }
        /*
         * The file extension of the serialized file
         */
        public string FileExtension { get; }
        /*
         * Attempt to parse the file provided to the object.
         * Returns: true if successful, false otherwise
         */
        public bool Parse();
        /*
         * Attempts to search the file using a given search parameter.
         * Input: term - (string) the parameters used to search the file.
         * Returns: true if successful, false otherwise
         */
        public bool Search(string term);
    }
}
