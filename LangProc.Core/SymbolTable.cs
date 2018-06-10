using System.Collections.Generic;
using LangProc.Core.Symbols;

namespace LangProc.Core
{
    public class SymbolTable
    {
        private readonly Dictionary<string, Symbol> _symbols;

        public SymbolTable(string scopeName, int scopeLevel, SymbolTable parentScope = null)
        {
            ScopeName = scopeName;
            ScopeLevel = scopeLevel;
            ParentScope = parentScope;
            _symbols = new Dictionary<string, Symbol>();

            Insert(new BuiltInTypeSymbol(TokenType.Integer.ToString()));
            Insert(new BuiltInTypeSymbol(TokenType.Real.ToString()));
        }

        public string ScopeName { get; }
        public int ScopeLevel { get; }
        public SymbolTable ParentScope { get; }

        public void Insert(Symbol symbol)
        {
            _symbols[symbol.Name] = symbol;
        }

        public Symbol Lookup(string name, bool currentScopeOnly = false)
        {
            return _symbols.TryGetValue(name, out var symbol)
                ? symbol : currentScopeOnly ?
                    null : ParentScope?.Lookup(name);
        }
    }
}
