using System;
using System.Collections.Generic;
using LangProc.Core.Symbols;
using LangProc.Core.Tree;

namespace LangProc.Core
{
    public class SemanticAnalyzer
    {
        public SymbolTable CurrentScope { get; private set; }

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
            var globalScope = new SymbolTable("GLOBAL", 1, CurrentScope);
            CurrentScope = globalScope;

            Visit(node.BlockNode);

            CurrentScope = CurrentScope.ParentScope;
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
            var typeSymbol = CurrentScope.Lookup(typeName);

            var varName = node.VariableNode.Data.Value.ToString();
            var varSymbol = new VariableSymbol(varName, typeSymbol);

            if (CurrentScope.Lookup(varName, true) != null)
                throw new InvalidOperationException($"Variable {varName} has already been declared.");

            CurrentScope.Insert(varSymbol);
        }

        private void Visit(AssignmentNode node)
        {
            var varName = node.Variable.Data.Value.ToString();
            var varSymbol = CurrentScope.Lookup(varName);
            if (varSymbol == null)
                throw new InvalidOperationException($"Variable {varName} was not declared.");

            Visit(node.Value);
        }

        private void Visit(VariableNode node)
        {
            var varName = node.Data.Value.ToString();
            var varSymbol = CurrentScope.Lookup(varName);
            if (varSymbol == null)
                throw new InvalidOperationException($"Variable {varName} was not declared.");
        }

        private void Visit(ProcedureNode node)
        {
            var procName = node.Data.Value.ToString();

            var procParams = new List<VariableSymbol>();
            foreach (var parameter in node.Parameters)
            {
                var paramType = CurrentScope.Lookup(parameter.Type.Data.Type.ToString());
                var paramName = parameter.Variable.Data.Value.ToString();
                var varSymbol = new VariableSymbol(paramName, paramType);
                CurrentScope.Insert(varSymbol);
                procParams.Add(varSymbol);
            }

            var procSymbol = new ProcedureSymbol(procName, procParams);
            CurrentScope.Insert(procSymbol);

            var procScope = new SymbolTable(procName, 2, CurrentScope);
            CurrentScope = procScope;

            Visit(node.BlockNode);

            CurrentScope = CurrentScope.ParentScope;
        }

        #endregion
    }
}
