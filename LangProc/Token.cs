﻿using System.Collections.Generic;

namespace LangProc
{
    internal class Token
    {
        #region Static Lookups

        public static readonly IDictionary<char, TokenType> OperatorTypes = new Dictionary<char, TokenType>
        {
            {'+', TokenType.Add },
            {'-', TokenType.Sub },
            {'*', TokenType.Mult },
            {'/', TokenType.Div }
        };

        #endregion

        #region Token Instance

        public Token(TokenType type, object value = null)
        {
            Type = type;
            Value = value;
        }

        public TokenType Type { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return $"Token({Type}, {Value}";
        }

        #endregion

        #region Static Parsing

        public static Token ParseOperator(char value)
        {
            return new Token(OperatorTypes[value], value);
        }

        public static bool TryParseOperator(char value, out Token token)
        {
            try
            {
                token = ParseOperator(value);
                return true;
            }
            catch
            {
                token = null;
                return false;
            }
        }

        #endregion
    }
}
