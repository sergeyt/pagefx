using System.Collections.Generic;

namespace DataDynamics.PageFX.CodeModel
{
    /// <summary>
    /// Base interface for expressions
    /// </summary>
    public interface IExpression : ICodeNode
    {
        /// <summary>
        /// Gets the expression result type.
        /// </summary>
        IType ResultType { get; }
    }

    /// <summary>
    /// Represents list of <see cref="IExpression"/>s.
    /// </summary>
    public interface IExpressionCollection : IList<IExpression>, IExpression
    {
    }

    public interface IEnclosingExpression : IExpression
    {
        /// <summary>
        /// Get or sets expression
        /// </summary>
        IExpression Expression { get; set; }
    }

    /// <summary>
    /// Represents literal constant expression.
    /// </summary>
    public interface IConstantExpression : IExpression
    {
        /// <summary>
        /// Gets or sets value of the constant.
        /// </summary>
        object Value { get; set; }
    }

    /// <summary>
    /// Represents sizeof expression
    /// </summary>
    public interface ISizeOfExpression : IExpression
    {
        IType Type { get; set; }
    }

    /// <summary>
    /// Represents typeof expresion
    /// </summary>
    public interface ITypeOfExpression : IExpression
    {
        IType Type { get; set; }
    }

    /// <summary>
    /// Represents binary expression.
    /// </summary>
    public interface IBinaryExpression : IExpression
    {
        /// <summary>
        /// Gets or sets left subexpression.
        /// </summary>
        IExpression Left { get; set; }

        /// <summary>
        /// Gets or sets binary operator to compute.
        /// </summary>
        BinaryOperator Operator { get; set; }

        /// <summary>
        /// Get or sets right subexpression.
        /// </summary>
        IExpression Right { get; set; }
    }
    
    /// <summary>
    /// Represents unary expression.
    /// </summary>
    public interface IUnaryExpression : IEnclosingExpression
    {
        /// <summary>
        /// Gets or sets operator to compute.
        /// </summary>
        UnaryOperator Operator { get; set; }
    }

    /// <summary>
    /// Represents ternary conditional expression.
    /// </summary>
    public interface IConditionExpression : IExpression
    {
        /// <summary>
        /// Gets or sets condition of the ternary expression.
        /// </summary>
        IExpression Condition { get; set; }

        /// <summary>
        /// Gets or sets expression for value when condition will be true.
        /// </summary>
        IExpression TrueExpression { get; set; }

        /// <summary>
        /// Gets or sets expression for value when condition will be false.
        /// </summary>
        IExpression FalseExpression { get; set; }
    }

    /// <summary>
    /// stackalloc type [expression]
    /// </summary>
    public interface IStackAllocateExpression : IExpression
    {
        IType Type { get; set; }
        IExpression Expression { get; set; }
    }

    /// <summary>
    /// Specifies constructor invocation expression to create new object
    /// </summary>
    public interface INewObjectExpression : IExpression
    {
        IType ObjectType { get; set; }
        IMethod Constructor { get; set; }
        IExpressionCollection Arguments { get; }
    }

    public interface ICastExpression : IEnclosingExpression
    {
        IType SourceType { get; }
        IType TargetType { get; set; }
        CastOperator Operator { get; set; }
    }

    public interface IBoxExpression : IEnclosingExpression
    {
        IType SourceType { get; set; }
    }

    public interface IUnboxExpression : IEnclosingExpression
    {
        IType TargetType { get; set; }
    }
    
    public interface IIndexerExpression : IExpression
    {
        IPropertyReferenceExpression Property { get; set; }
        IExpressionCollection Index { get; }
    }

    public interface ICallExpression : IExpression
    {
        IMethodReferenceExpression Method { get; set; }
        IExpressionCollection Arguments { get; }
    }
}