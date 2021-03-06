﻿using System;
using System.Collections.Generic;
using LangProc.Core.Tree;

namespace LangProc.Core
{
    public class Interpreter
    {
        public Interpreter()
        {
            GlobalMemory = new Dictionary<string, object>();
        }

        public IDictionary<string, object> GlobalMemory { get; }

        public void Interpret(TreeNode<Token> tree)
        {
            Visit(tree);
        }

        #region Visit Tree

        private object Visit(TreeNode<Token> node)
        {
            switch (node)
            {
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

                case BinaryOperationNode node1:
                    return Visit(node1);
                case NumberNode node1:
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
                case ProcedureNode node1:
                    Visit(node1);
                    return null;

                default:
                    throw new InvalidOperationException("Unsupported node type.");
            }
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

        private object Visit(BinaryOperationNode node)
        {
            bool isLeftInteger = node.LeftChild.Data.Type == TokenType.Integer;
            bool isRightInteger = node.RightChild.Data.Type == TokenType.Integer;

            switch (node.Data.Type)
            {
                case TokenType.Add:
                    return (isLeftInteger ? (int) Visit(node.LeftChild) : (double) Visit(node.LeftChild)) +
                           (isRightInteger ? (int) Visit(node.RightChild) : (double) Visit(node.RightChild));
                case TokenType.Sub:
                    return (isLeftInteger ? (int)Visit(node.LeftChild) : (double)Visit(node.LeftChild)) -
                           (isRightInteger ? (int)Visit(node.RightChild) : (double)Visit(node.RightChild));
                case TokenType.Mult:
                    return (isLeftInteger ? (int)Visit(node.LeftChild) : (double)Visit(node.LeftChild)) *
                           (isRightInteger ? (int)Visit(node.RightChild) : (double)Visit(node.RightChild));
                case TokenType.Div:
                    return (int) Visit(node.LeftChild) / (int) Visit(node.RightChild);
                case TokenType.FloatDiv:
                    return (double) Visit(node.LeftChild) / (double) Visit(node.RightChild);
                default:
                    throw new InvalidOperationException($"Token type {node.Data.Type} not expected for binary operations.");
            }
        }

        private object Visit(NumberNode node)
        {
            return node.Data.Value;
        }

        private object Visit(UnaryOperationNode node)
        {
            bool isInteger = node.Value.Data.Type == TokenType.Integer;

            switch (node.Data.Type)
            {
                case TokenType.Add:
                    return isInteger ? +(int) Visit(node.Value) : +(double) Visit(node.Value);
                case TokenType.Sub:
                    return isInteger ? -(int) Visit(node.Value) : -(double) Visit(node.Value);
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
            GlobalMemory[varName] = Visit(node.Value);
        }

        private object Visit(VariableNode node)
        {
            string varName = node.Data.Value.ToString();

            if (GlobalMemory.TryGetValue(varName, out var value))
                return value;

            throw new InvalidOperationException($"Variable {varName} not assigned a value");
        }

        private void Visit(NopNode node)
        {
        }

        private void Visit(ProcedureNode node)
        {
        }

        #endregion
    }
}
