namespace JackCompiler;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }

    private static string SaveCode(string filePath, string vmCode)
    {
        var directoryName = Path.GetDirectoryName(filePath);
        var listingFileName = Path.GetFileNameWithoutExtension(filePath);

        var outputFilePath = Path.Combine(directoryName, listingFileName) + ".vm";

        if (File.Exists(outputFilePath))
            File.Delete(outputFilePath);

        File.AppendAllText(outputFilePath, vmCode);

        return outputFilePath;
    }
}