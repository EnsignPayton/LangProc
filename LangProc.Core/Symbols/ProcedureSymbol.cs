using System.Collections.Generic;

namespace LangProc.Core.Symbols
{
    public class ProcedureSymbol : Symbol
    {
        public ProcedureSymbol(string name, IEnumerable<VariableSymbol> parameters = null) : base(name)
        {
            Parameters = parameters;
        }

        public IEnumerable<VariableSymbol> Parameters { get; }
    }
}
