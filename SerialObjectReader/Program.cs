using SerialObjectReader.FileTypes;

namespace SerialObjectReader;

class Program
{
    private static SerialFileReader? _fileReader;

    private const string JSON_EXTENSION = ".json";
    private const string XML_EXTENSION = ".xml";
    private const string QUIT = "quit";
    private const string X = "x";

    public static bool HasValidFile { get; private set; }
    public static bool HasQuit { get; private set; }

    /*
     * Reused method for getting a line of text input from the console. Allows for the user to quit the application when desired.
     */
    private static string? GetInputFromConsole()
    {
        Console.WriteLine("Type Quit or X at any time to close application.");

        var inputLine = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(inputLine)) return null;

        if (inputLine.Equals(QUIT, StringComparison.InvariantCultureIgnoreCase) ||
            inputLine.Equals(X, StringComparison.InvariantCultureIgnoreCase))
        {
            HasQuit = true;
            return null;
        }

        return inputLine;
    }

    /*
     * Main Method of the Application. Continue to attempt to read a serialized file until one is loaded, then allow the user to search until finished.
     */
    static async Task Main(string[] args)
    {
        //Attempt to read file while one is not loaded and parsed or the user quits.
        while (!(HasValidFile || HasQuit))
        {
            Console.WriteLine("Input the serialized file to be parsed. Supported File Types are:");
            Console.WriteLine($" - {JSON_EXTENSION}");
            Console.WriteLine($" - {XML_EXTENSION}");

            var inputLine = GetInputFromConsole();

            if (inputLine == null) continue;

            //is .json file
            if (inputLine.EndsWith(JSON_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
            {
                _fileReader = new JsonSerialFileReader(inputLine);

                var result = await _fileReader.Parse();

                if (result)
                {
                    HasValidFile = true;
                }
            }
            //is .xml file
            else if (inputLine.EndsWith(XML_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
            {
                //TODO: Implement
            }
            //unsupported file
            else
            {
                Console.WriteLine("File inputted is not a supported format.");
            }

            if (HasValidFile) Console.WriteLine($"File {_fileReader.Filename} is ready to be searched.");
        }

        while (!HasQuit)
        {
            Console.WriteLine("Input any search parameters.");

            var inputLine = GetInputFromConsole();

            if (HasQuit) return;

            _fileReader.Search(inputLine);
        }
    }
}