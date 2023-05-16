namespace JackCompiler;

public class SymbolsTable
{
    private readonly string _className;
    private Dictionary<string, SymbolInfo> _classSymbols;
    private Dictionary<string, SymbolInfo> _subroutineSymbols;
    private string _subroutineName;
    
    public string ClassName => _className;
    public string SubroutineName => _subroutineName ?? throw new InvalidOperationException("SubroutineName is null");

    public SymbolsTable(string className)
    {
        _className = className;
    }
    
    public void StartSubroutine(string name)
    {
        _subroutineName = name;
        _subroutineSymbols = new Dictionary<string, SymbolInfo>();
    }
    
    public void Define(string name, string type, SymbolInfo.SymbolLocation location)
    {
        var index = GetCount(location);
        var symbolInfo = new SymbolInfo()
        {
            Type = type,
            Location = location,
            Index = index,
        };

        if (location is SymbolInfo.SymbolLocation.Static or SymbolInfo.SymbolLocation.Field)    
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

    private int GetCount(SymbolInfo.SymbolLocation kind) =>
        kind is SymbolInfo.SymbolLocation.Static or SymbolInfo.SymbolLocation.Field
            ? _classSymbols.Count(symbol => symbol.Value.Location == kind)
            : _subroutineSymbols.Count(symbol => symbol.Value.Location == kind);
}

public class SymbolInfo
{
    public string Type;
    public SymbolLocation Location;
    public int Index;

    public enum SymbolLocation
    {
        Static,
        Field,
        Argument,
        Local,
    }
}