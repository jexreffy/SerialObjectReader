// See https://aka.ms/new-console-template for more information

using SerialObjectReader.FileTypes;

var JSON_EXTENSION = ".json";
var XML_EXTENSION = ".xml";
var QUIT = "quit";
var X = "x";

//Input, open and parse the serialized file.
var hasValidFile = false;
ISerialFileReader fileReader = null;

while (!hasValidFile)
{
    Console.WriteLine("Input the serialized file to be parsed. Supported File Types are:");
    Console.WriteLine($" - {JSON_EXTENSION}");
    Console.WriteLine($" - {XML_EXTENSION}");
    Console.WriteLine("Type Quit or X at any time to close application.");

    var inputLine = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(inputLine)) continue;

    if (inputLine.Equals(QUIT, StringComparison.InvariantCultureIgnoreCase) ||
         inputLine.Equals(X, StringComparison.InvariantCultureIgnoreCase)) return;

    if (inputLine.EndsWith(JSON_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
    {
        fileReader = new JSONSerialFileReader(inputLine);

        if (fileReader.Parse())
        {
            hasValidFile = true;
        }
    }
    else if (inputLine.EndsWith(XML_EXTENSION, StringComparison.InvariantCultureIgnoreCase))
    {
        //TODO: Implement
    }
    else
    {
        Console.WriteLine("File inputted is not a supported format.");
    }

    Console.WriteLine("End of Loop");
}

Console.WriteLine($"File {fileReader.Filename} is ready to be searched.");

while (true)
{
    Console.WriteLine("Input any search parameters. Type Quit or X at any time to close application.");

    var inputLine = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(inputLine)) continue;

    if (inputLine.Equals(QUIT, StringComparison.InvariantCultureIgnoreCase) ||
        inputLine.Equals(X, StringComparison.InvariantCultureIgnoreCase)) return;

    fileReader.Search(inputLine);
}