using System;
using System.Collections.Generic;
using LangProc.Core.Tree;

namespace LangProc.Core
{
    public class Interpreter
    {
        public Interpreter()
        {
            GlobalScope = new Dictionary<string, object>();
        }

        public IDictionary<string, object> GlobalScope { get; }

        public void Interpret(string text)
        {
            var tokens = Tokenizer.GetTokens(text, true);

            using (var parser = new Parser(tokens))
            {
                var tree = parser.Parse();

                Visit(tree);
            }
        }

        public int? Visit(TreeNode<Token> node)
        {
            switch (node)
            {
                case NumberNode numberNode:
                    return Visit(numberNode);
                case BinaryOperationNode binNode:
                    return Visit(binNode);
                case UnaryOperationNode unNode:
                    return Visit(unNode);
                case CompoundNode compoundNode:
                    Visit(compoundNode);
                    return null;
                case AssignmentNode assignmentNode:
                    Visit(assignmentNode);
                    return null;
                case VariableNode varNode:
                    return Visit(varNode);
                case NopNode unNode:
                    Visit(unNode);
                    return null;
                default:
                    throw new InvalidOperationException("Unsupported node type.");
            }
        }

        private int Visit(NumberNode node)
        {
            return (int) node.Data.Value;
        }

        private int Visit(BinaryOperationNode node)
        {
            switch (node.Data.Type)
            {
                case TokenType.Add:
                    return Visit(node.LeftChild).Value + Visit(node.RightChild).Value;
                case TokenType.Sub:
                    return Visit(node.LeftChild).Value - Visit(node.RightChild).Value;
                case TokenType.Mult:
                    return Visit(node.LeftChild).Value * Visit(node.RightChild).Value;
                case TokenType.Div:
                    return Visit(node.LeftChild).Value / Visit(node.RightChild).Value;
                default:
                    throw new InvalidOperationException($"Token type {node.Data.Type} not expected for binary operations.");
            }
        }

        private int Visit(UnaryOperationNode node)
        {
            switch (node.Data.Type)
            {
                case TokenType.Add:
                    return +Visit(node.Value).Value;
                case TokenType.Sub:
                    return -Visit(node.Value).Value;
                default:
                    throw new InvalidOperationException($"Token type {node.Data.Type} not expected for unary operations.");
            }
        }

        private void Visit(CompoundNode node)
        {
            foreach (var child in node.Children)
            {
                Visit(child);
            }
        }

        private void Visit(AssignmentNode node)
        {
            string varName = node.LeftChild.Data.Value.ToString();
            GlobalScope[varName] = Visit(node.RightChild);
        }

        private int Visit(VariableNode node)
        {
            string varName = node.Data.Value.ToString();

            if (GlobalScope.TryGetValue(varName, out var value))
                return (int) value;

            throw new InvalidOperationException($"Variable {varName} not defined");
        }

        private void Visit(NopNode node)
        {
            // Nop
        }
    }
}
