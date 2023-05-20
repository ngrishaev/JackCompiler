namespace JackCompiler;

/// <summary>
/// Collect all the source files and return a list of absolute paths to them
/// </summary>
public class SourcesCollector
{
    private readonly List<string> _filePathes = new List<string>();

    /// <summary>
    /// Collect all the source files and return a list of sources
    /// </summary>
    /// <param name="path">Path to the source file or directory</param>
    public SourcesCollector(string path)
    {
        if (File.Exists(path))
        {
            AddSingleFile(path);
            return;
        }

        if (Directory.Exists(path))
        {
            AddDirectoryFiles(path);
            return;
        }

        throw new ArgumentException($"Cant find source files at {path}");
    }

    private void AddDirectoryFiles(string path)
    {
        _filePathes.AddRange(Directory.EnumerateFiles(path).Where(file => file.EndsWith(".jack")));
    }

    private void AddSingleFile(string path)
    {
        if (path.EndsWith(".jack") == false)
            throw new ArgumentException($"File at {path} shoud have '.jack' extension");

        _filePathes.Add(path);
    }
    
    /// <summary>
    /// Return a source files as string, one by one
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Source> GetSources()
    {
        foreach (var filePath in _filePathes)
        {
            using var file = new StreamReader(filePath);
            yield return new Source(filePath, file.ReadToEnd());
        }
    }

    public class Source
    {
        public string Path { get; }
        public string Content { get; }
        
        public Source(string path, string content)
        {
            Path = path;
            Content = content;
        }
    }
}