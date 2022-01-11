﻿using System;
using System.Collections.Generic;

namespace StringMath
{
    /// <inheritdoc />
    internal class ExpressionOptimizer<TNum> : IExpressionVisitor<IExpression> where TNum : INumber<TNum>
    {
        private readonly Dictionary<ExpressionType, Func<IExpression, IExpression>> _expressionOptimizers;
        private readonly IMathContext<TNum> _context;

        /// <summary>Initializez a new instance of an expression optimizer.</summary>
        /// <param name="mathContext">The math context used to evaluate expressions.</param>
        public ExpressionOptimizer(IMathContext<TNum> mathContext)
        {
            mathContext.EnsureNotNull(nameof(mathContext));
            _context = mathContext;

            _expressionOptimizers = new Dictionary<ExpressionType, Func<IExpression, IExpression>>
            {
                [ExpressionType.BinaryExpression] = OptimizeBinaryExpression,
                [ExpressionType.UnaryExpression] = EvaluateUnaryExpression,
                [ExpressionType.ConstantExpression] = OptimizeConstantExpression,
                [ExpressionType.GroupingExpression] = OptimizeGroupingExpression,
                [ExpressionType.VariableExpression] = SkipExpressionOptimization,
                [ExpressionType.ValueExpression] = SkipExpressionOptimization
            };
        }

        /// <summary>Simplifies an expression tree by removing unnecessary nodes and evaluating constant expressions.</summary>
        /// <param name="expression">The expression tree to optimize.</param>
        /// <returns>An optimized expression tree.</returns>
        public IExpression Visit(IExpression expression)
        {
            IExpression result = _expressionOptimizers[expression.Type](expression);
            return result;
        }

        private IExpression OptimizeConstantExpression(IExpression expr)
        {
            ConstantExpression constantExpr = (ConstantExpression)expr;
            return constantExpr.ToValueExpression<TNum>();
        }

        private IExpression OptimizeGroupingExpression(IExpression expr)
        {
            GroupingExpression groupingExpr = (GroupingExpression)expr;
            IExpression innerExpr = Visit(groupingExpr.Inner);
            return innerExpr;
        }

        private IExpression EvaluateUnaryExpression(IExpression expr)
        {
            UnaryExpression unaryExpr = (UnaryExpression)expr;
            IExpression operandExpr = Visit(unaryExpr.Operand);
            if (operandExpr is ValueExpression<TNum> valueExpr)
            {
                TNum result = _context.EvaluateUnary(unaryExpr.OperatorName, valueExpr.Value);
                return new ValueExpression<TNum>(result);
            }

            return new UnaryExpression(unaryExpr.OperatorName, operandExpr);
        }

        private IExpression OptimizeBinaryExpression(IExpression expr)
        {
            BinaryExpression binaryExpr = (BinaryExpression)expr;
            IExpression leftExpr = Visit(binaryExpr.Left);
            IExpression rightExpr = Visit(binaryExpr.Right);

            if (leftExpr is ValueExpression<TNum> leftValue && rightExpr is ValueExpression<TNum> rightValue)
            {
                TNum result = _context.EvaluateBinary(binaryExpr.OperatorName, leftValue.Value, rightValue.Value);
                return new ValueExpression<TNum>(result);
            }

            return new BinaryExpression(leftExpr, binaryExpr.OperatorName, rightExpr);
        }

        private IExpression SkipExpressionOptimization(IExpression expr)
        {
            return expr;
        }
    }
}
