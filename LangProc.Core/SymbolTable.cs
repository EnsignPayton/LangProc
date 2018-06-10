using System.Collections.Generic;
using LangProc.Core.Symbols;

namespace LangProc.Core
{
    public class SymbolTable
    {
        private readonly Dictionary<string, Symbol> _symbols;

        public SymbolTable(string scopeName, int scopeLevel)
        {
            ScopeName = scopeName;
            ScopeLevel = scopeLevel;
            _symbols = new Dictionary<string, Symbol>();

            Insert(new BuiltInTypeSymbol(TokenType.Integer.ToString()));
            Insert(new BuiltInTypeSymbol(TokenType.Real.ToString()));
        }

        public string ScopeName { get; }
        public int ScopeLevel { get; }

        public void Insert(Symbol symbol)
        {
            _symbols[symbol.Name] = symbol;
        }

        public Symbol Lookup(string name)
        {
            return _symbols.TryGetValue(name, out var symbol) ? symbol : null;
        }
    }
}
