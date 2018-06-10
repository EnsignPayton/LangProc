using System;
using LangProc.Core.Symbols;
using LangProc.Core.Tree;

namespace LangProc.Core
{
    public class SemanticAnalyzer
    {
        public SymbolTable Scope { get; } = new SymbolTable("GLOBAL", 1);

        public void Build(TreeNode<Token> tree)
        {
            Visit(tree);
        }

        #region Visit Tree

        private void Visit(TreeNode<Token> node)
        {
            switch (node)
            {
                case BlockNode node1:
                    Visit(node1);
                    break;
                case ProgramNode node1:
                    Visit(node1);
                    break;
                case BinaryOperationNode node1:
                    Visit(node1);
                    break;
                case NumberNode node1:
                    Visit(node1);
                    break;
                case UnaryOperationNode node1:
                    Visit(node1);
                    break;
                case CompoundNode node1:
                    Visit(node1);
                    break;
                case NopNode node1:
                    Visit(node1);
                    break;
                case DeclarationNode node1:
                    Visit(node1);
                    break;
                case AssignmentNode node1:
                    Visit(node1);
                    break;
                case VariableNode node1:
                    Visit(node1);
                    break;
                case ProcedureNode node1:
                    Visit(node1);
                    break;
            }
        }

        private void Visit(BlockNode node)
        {
            foreach (var declaration in node.Declarations)
            {
                Visit(declaration);
            }

            Visit(node.CompoundNode);
        }

        private void Visit(ProgramNode node)
        {
            Visit(node.BlockNode);
        }

        private void Visit(BinaryOperationNode node)
        {
            Visit(node.LeftChild);
            Visit(node.RightChild);
        }

        private void Visit(NumberNode node)
        {
        }

        private void Visit(UnaryOperationNode node)
        {
            Visit(node.Value);
        }

        private void Visit(CompoundNode node)
        {
            foreach (var child in node.Children)
            {
                Visit(child);
            }
        }

        private void Visit(NopNode node)
        {
        }

        private void Visit(DeclarationNode node)
        {
            var typeName = node.TypeNode.Data.Type.ToString();
            var typeSymbol = Scope.Lookup(typeName);

            var varName = node.VariableNode.Data.Value.ToString();
            var varSymbol = new VariableSymbol(varName, typeSymbol);

            //if (Scope.Lookup(varName) != null)
            //    throw new InvalidOperationException($"Variable {varName} has already been declared.");

            Scope.Insert(varSymbol);
        }

        private void Visit(AssignmentNode node)
        {
            var varName = node.Variable.Data.Value.ToString();
            var varSymbol = Scope.Lookup(varName);
            if (varSymbol == null)
                throw new InvalidOperationException($"Variable {varName} was not declared.");

            Visit(node.Value);
        }

        private void Visit(VariableNode node)
        {
            var varName = node.Data.Value.ToString();
            var varSymbol = Scope.Lookup(varName);
            if (varSymbol == null)
                throw new InvalidOperationException($"Variable {varName} was not declared.");
        }

        private void Visit(ProcedureNode node)
        {
        }

        #endregion
    }
}
