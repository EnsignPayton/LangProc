﻿namespace LangProc.Core
{
    public enum TokenType
    {
        Integer,
        Real,
        ParenOpen,
        ParenClose,
        Add,
        Sub,
        Mult,
        Div,
        FloatDiv,
        Program,
        Var,
        DeclInteger,
        DeclReal,
        Begin,
        End,
        Dot,
        Assign,
        Semi,
        Colon,
        Comma,
        Id,
        EndOfFile,
        Unknown
    }
}
