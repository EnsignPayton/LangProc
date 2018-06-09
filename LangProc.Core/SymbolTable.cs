using System.Collections.Generic;
using LangProc.Core.Symbols;

namespace LangProc.Core
{
    public class SymbolTable
    {
        private readonly Dictionary<string, Symbol> _symbols;

        public SymbolTable()
        {
            _symbols = new Dictionary<string, Symbol>();

            Define(new BuiltInTypeSymbol(TokenType.Integer.ToString()));
            Define(new BuiltInTypeSymbol(TokenType.Real.ToString()));
        }

        public void Define(Symbol symbol)
        {
            _symbols[symbol.Name] = symbol;
        }

        public Symbol Lookup(string name)
        {
            return _symbols.TryGetValue(name, out var symbol) ? symbol : null;
        }
    }
}
