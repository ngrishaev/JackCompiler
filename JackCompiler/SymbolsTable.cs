namespace JackCompiler;

public class SymbolsTable
{
    private readonly string _className;
    private Dictionary<string, SymbolInfo> _classSymbols;
    private Dictionary<string, SymbolInfo> _subroutineSymbols;
    private string _subroutineName;

    public SymbolsTable(string className)
    {
        _className = className;
    }
    
    public void StartSubroutine(string name)
    {
        _subroutineName = name;
        _subroutineSymbols = new Dictionary<string, SymbolInfo>();
    }
    
    public void Define(string name, string type, string kind)
    {
        var index = GetCount(kind);
        var symbolInfo = new SymbolInfo()
        {
            Type = type,
            Kind = kind,
            Index = index,
        };

        if (kind is "static" or "field")
            _classSymbols[name] = symbolInfo;
        else
            _subroutineSymbols[name] = symbolInfo;
    }
    
    public bool TryGetSymbol(string name, out SymbolInfo symbolInfo)
    {
        if (_subroutineSymbols.TryGetValue(name, out symbolInfo))
            return true;
        
        if (_classSymbols.TryGetValue(name, out symbolInfo))
            return true;

        return false;
    }

    private int GetCount(string kind) =>
        kind is "static" or "field"
            ? _classSymbols.Count(symbol => symbol.Value.Kind == kind)
            : _subroutineSymbols.Count(symbol => symbol.Value.Kind == kind);
}

public class SymbolInfo
{
    public string Type;
    public string Kind;
    public int Index;
}