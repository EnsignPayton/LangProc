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
                case NumberNode node1:
                    return Visit(node1);
                case BinaryOperationNode node1:
                    return Visit(node1);
                case UnaryOperationNode node1:
                    return Visit(node1);
                case CompoundNode node1:
                    Visit(node1);
                    return null;
                case AssignmentNode node1:
                    Visit(node1);
                    return null;
                case VariableNode node1:
                    return Visit(node1);
                case NopNode node1:
                    Visit(node1);
                    return null;
                case ProgramNode node1:
                    Visit(node1);
                    return null;
                case BlockNode node1:
                    Visit(node1);
                    return null;
                case DeclarationNode node1:
                    Visit(node1);
                    return null;
                case TypeNode node1:
                    Visit(node1);
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
                case TokenType.FloatDiv:
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
            string varName = node.Variable.Data.Value.ToString();
            GlobalScope[varName] = Visit(node.Value);
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
        }

        private void Visit(ProgramNode node)
        {
            Visit(node.BlockNode);
        }

        private void Visit(BlockNode node)
        {
            foreach (var declaration in node.Declarations)
            {
                Visit(declaration);
            }

            Visit(node.CompoundNode);
        }

        private void Visit(DeclarationNode node)
        {
        }

        private void Visit(TypeNode node)
        {
        }
    }
}
