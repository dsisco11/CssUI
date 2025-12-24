using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace CssUI.CSS;

/// <summary>
/// Represents a node in a CSS calc() expression tree.
/// </summary>
/// <remarks>
/// Per CSS Values Level 4 §10, calc() expressions are represented as calculation trees
/// where branch nodes are operators and leaf nodes are numeric values.
/// Docs: https://www.w3.org/TR/css-values-4/#calc-func
/// </remarks>
public abstract class CssCalcNode
{
    /// <summary>
    /// Gets the node type for this expression node.
    /// </summary>
    public abstract ECssCalcNodeType NodeType { get; }

    /// <summary>
    /// Evaluates the node, returning a simplified numeric value if possible.
    /// </summary>
    /// <param name="resolver">Optional unit resolver for converting dimensions.</param>
    /// <returns>The evaluated value, or null if evaluation requires more context.</returns>
    public abstract double? Evaluate(CssUnitResolver? resolver);

    /// <summary>
    /// Serializes the node to a CSS string.
    /// </summary>
    public abstract string ToCssString();

    /// <summary>
    /// Gets the result type of this node (the type it evaluates to).
    /// </summary>
    public abstract ECssCalcResultType ResultType { get; }
}

/// <summary>
/// Types of nodes in a calc() expression tree.
/// </summary>
public enum ECssCalcNodeType
{
    /// <summary>A numeric literal value (number, percentage, or dimension).</summary>
    Value,
    /// <summary>Addition operator node.</summary>
    Sum,
    /// <summary>Negation operator node (unary minus).</summary>
    Negate,
    /// <summary>Product operator node (multiplication).</summary>
    Product,
    /// <summary>Inversion operator node (division becomes multiplication by inverse).</summary>
    Invert,
    /// <summary>A parenthesized sub-expression.</summary>
    Parentheses,
    /// <summary>A nested calc() or other math function.</summary>
    Function,
    /// <summary>A min() function node that returns the smallest of its arguments.</summary>
    Min,
    /// <summary>A max() function node that returns the largest of its arguments.</summary>
    Max,
    /// <summary>A clamp() function node that clamps a value between min and max.</summary>
    Clamp
}

/// <summary>
/// The result type of a calc() expression.
/// </summary>
public enum ECssCalcResultType
{
    /// <summary>Expression resolves to a number.</summary>
    Number,
    /// <summary>Expression resolves to a percentage.</summary>
    Percentage,
    /// <summary>Expression resolves to a length.</summary>
    Length,
    /// <summary>Expression resolves to an angle.</summary>
    Angle,
    /// <summary>Expression resolves to a time.</summary>
    Time,
    /// <summary>Expression resolves to a frequency.</summary>
    Frequency,
    /// <summary>Expression resolves to a resolution.</summary>
    Resolution,
    /// <summary>Expression contains incompatible types (invalid).</summary>
    Invalid
}

/// <summary>
/// Represents a numeric value leaf node in a calc() expression.
/// </summary>
public sealed class CssCalcValueNode : CssCalcNode
{
    /// <summary>Gets the numeric value.</summary>
    public double Value { get; }

    /// <summary>Gets the unit (if any).</summary>
    public ECssUnit Unit { get; }

    /// <summary>Gets whether this is a percentage value.</summary>
    public bool IsPercentage { get; }

    public override ECssCalcNodeType NodeType => ECssCalcNodeType.Value;

    public override ECssCalcResultType ResultType
    {
        get
        {
            if (IsPercentage)
                return ECssCalcResultType.Percentage;
            if (Unit == ECssUnit.None)
                return ECssCalcResultType.Number;

            // Determine type from unit
            return Unit switch
            {
                // Length units
                ECssUnit.PX or ECssUnit.EM or ECssUnit.REM or
                ECssUnit.CM or ECssUnit.MM or ECssUnit.IN or
                ECssUnit.PT or ECssUnit.PC or ECssUnit.EX or
                ECssUnit.CH or ECssUnit.VW or ECssUnit.VH or
                ECssUnit.VMIN or ECssUnit.VMAX or ECssUnit.Q
                    => ECssCalcResultType.Length,
                // Angle units
                ECssUnit.DEG or ECssUnit.RAD or ECssUnit.GRAD or ECssUnit.TURN
                    => ECssCalcResultType.Angle,
                // Time units
                ECssUnit.S or ECssUnit.MS
                    => ECssCalcResultType.Time,
                // Frequency units
                ECssUnit.HZ or ECssUnit.KHZ
                    => ECssCalcResultType.Frequency,
                // Resolution units
                ECssUnit.DPI or ECssUnit.DPCM or ECssUnit.DPPX
                    => ECssCalcResultType.Resolution,
                _ => ECssCalcResultType.Number
            };
        }
    }

    /// <summary>
    /// Creates a numeric value node.
    /// </summary>
    public CssCalcValueNode(double value, ECssUnit unit = ECssUnit.None, bool isPercentage = false)
    {
        Value = value;
        Unit = unit;
        IsPercentage = isPercentage;
    }

    public override double? Evaluate(CssUnitResolver? resolver)
    {
        // If we have a unit and a resolver, convert to canonical unit
        if (resolver is not null && Unit != ECssUnit.None)
        {
            return resolver.Resolve(Value, Unit);
        }

        // Can't resolve percentages without context
        if (IsPercentage)
        {
            return null;
        }

        return Value;
    }

    public override string ToCssString()
    {
        var valueStr = Value.ToString(CultureInfo.InvariantCulture);

        if (IsPercentage)
            return $"{valueStr}%";

        if (Unit == ECssUnit.None)
            return valueStr;

        // Get the unit string from the MetaEnum keyword table
        if (Lookup.TryKeyword(Unit, out var unitStr))
            return $"{valueStr}{unitStr}";

        // Fallback for unmapped units
        return $"{valueStr}{Unit.ToString().ToLowerInvariant()}";
    }
}

/// <summary>
/// Represents a sum operation (addition and subtraction) in a calc() expression.
/// Subtraction is represented as addition of a negated node.
/// </summary>
public sealed class CssCalcSumNode : CssCalcNode
{
    /// <summary>Gets the children of this sum.</summary>
    public ImmutableArray<CssCalcNode> Children { get; }

    public override ECssCalcNodeType NodeType => ECssCalcNodeType.Sum;

    public override ECssCalcResultType ResultType
    {
        get
        {
            if (Children.IsEmpty)
                return ECssCalcResultType.Invalid;

            // The result type must be consistent across all children
            var firstType = Children[0].ResultType;
            foreach (var child in Children)
            {
                var childType = child.ResultType;
                if (childType == ECssCalcResultType.Invalid)
                    return ECssCalcResultType.Invalid;

                // Numbers can be added to anything (as multiplier context)
                if (childType == ECssCalcResultType.Number || firstType == ECssCalcResultType.Number)
                    continue;

                // Percentages can be added to compatible types
                if (childType == ECssCalcResultType.Percentage || firstType == ECssCalcResultType.Percentage)
                    continue;

                // Types must match
                if (childType != firstType)
                    return ECssCalcResultType.Invalid;
            }

            // Find the first non-number type
            foreach (var child in Children)
            {
                var type = child.ResultType;
                if (type != ECssCalcResultType.Number)
                    return type;
            }

            return ECssCalcResultType.Number;
        }
    }

    public CssCalcSumNode(IEnumerable<CssCalcNode> children)
    {
        Children = children.ToImmutableArray();
    }

    public CssCalcSumNode(params CssCalcNode[] children)
    {
        Children = children.ToImmutableArray();
    }

    public override double? Evaluate(CssUnitResolver? resolver)
    {
        double sum = 0;
        foreach (var child in Children)
        {
            var value = child.Evaluate(resolver);
            if (!value.HasValue)
                return null;
            sum += value.Value;
        }
        return sum;
    }

    public override string ToCssString()
    {
        if (Children.IsEmpty)
            return "0";

        var sb = new StringBuilder();
        sb.Append('(');

        for (int i = 0; i < Children.Length; i++)
        {
            var child = Children[i];

            if (i > 0)
            {
                // Check if this is a negation - if so, use minus
                if (child is CssCalcNegateNode negateNode)
                {
                    sb.Append(" - ");
                    sb.Append(negateNode.Child.ToCssString());
                    continue;
                }
                sb.Append(" + ");
            }

            sb.Append(child.ToCssString());
        }

        sb.Append(')');
        return sb.ToString();
    }
}

/// <summary>
/// Represents a negation operation in a calc() expression.
/// </summary>
public sealed class CssCalcNegateNode : CssCalcNode
{
    /// <summary>Gets the child being negated.</summary>
    public CssCalcNode Child { get; }

    public override ECssCalcNodeType NodeType => ECssCalcNodeType.Negate;

    public override ECssCalcResultType ResultType => Child.ResultType;

    public CssCalcNegateNode(CssCalcNode child)
    {
        Child = child ?? throw new ArgumentNullException(nameof(child));
    }

    public override double? Evaluate(CssUnitResolver? resolver)
    {
        var value = Child.Evaluate(resolver);
        return value.HasValue ? -value.Value : null;
    }

    public override string ToCssString()
    {
        return $"(-1 * {Child.ToCssString()})";
    }
}

/// <summary>
/// Represents a product operation (multiplication and division) in a calc() expression.
/// Division is represented as multiplication by an inverted node.
/// </summary>
public sealed class CssCalcProductNode : CssCalcNode
{
    /// <summary>Gets the children of this product.</summary>
    public ImmutableArray<CssCalcNode> Children { get; }

    public override ECssCalcNodeType NodeType => ECssCalcNodeType.Product;

    public override ECssCalcResultType ResultType
    {
        get
        {
            if (Children.IsEmpty)
                return ECssCalcResultType.Invalid;

            // For products, we need exactly one dimension (or all numbers)
            ECssCalcResultType? dimensionType = null;

            foreach (var child in Children)
            {
                var childType = child.ResultType;
                if (childType == ECssCalcResultType.Invalid)
                    return ECssCalcResultType.Invalid;

                if (childType != ECssCalcResultType.Number)
                {
                    if (dimensionType.HasValue && dimensionType.Value != childType)
                    {
                        // Can't multiply two different dimension types
                        // (unless doing unit algebra which we don't support yet)
                        return ECssCalcResultType.Invalid;
                    }
                    dimensionType = childType;
                }
            }

            return dimensionType ?? ECssCalcResultType.Number;
        }
    }

    public CssCalcProductNode(IEnumerable<CssCalcNode> children)
    {
        Children = children.ToImmutableArray();
    }

    public CssCalcProductNode(params CssCalcNode[] children)
    {
        Children = children.ToImmutableArray();
    }

    public override double? Evaluate(CssUnitResolver? resolver)
    {
        double product = 1;
        foreach (var child in Children)
        {
            var value = child.Evaluate(resolver);
            if (!value.HasValue)
                return null;
            product *= value.Value;
        }
        return product;
    }

    public override string ToCssString()
    {
        if (Children.IsEmpty)
            return "1";

        var sb = new StringBuilder();
        sb.Append('(');

        for (int i = 0; i < Children.Length; i++)
        {
            var child = Children[i];

            if (i > 0)
            {
                // Check if this is an inversion - if so, use divide
                if (child is CssCalcInvertNode invertNode)
                {
                    sb.Append(" / ");
                    sb.Append(invertNode.Child.ToCssString());
                    continue;
                }
                sb.Append(" * ");
            }

            sb.Append(child.ToCssString());
        }

        sb.Append(')');
        return sb.ToString();
    }
}

/// <summary>
/// Represents an inversion operation (1/x) in a calc() expression.
/// </summary>
public sealed class CssCalcInvertNode : CssCalcNode
{
    /// <summary>Gets the child being inverted.</summary>
    public CssCalcNode Child { get; }

    public override ECssCalcNodeType NodeType => ECssCalcNodeType.Invert;

    public override ECssCalcResultType ResultType
    {
        get
        {
            // Inversion only makes sense for numbers
            // 1 / 5px doesn't have a meaningful CSS type
            var childType = Child.ResultType;
            return childType == ECssCalcResultType.Number
                ? ECssCalcResultType.Number
                : ECssCalcResultType.Invalid;
        }
    }

    public CssCalcInvertNode(CssCalcNode child)
    {
        Child = child ?? throw new ArgumentNullException(nameof(child));
    }

    public override double? Evaluate(CssUnitResolver? resolver)
    {
        var value = Child.Evaluate(resolver);
        if (!value.HasValue || value.Value == 0)
            return null;
        return 1.0 / value.Value;
    }

    public override string ToCssString()
    {
        return $"(1 / {Child.ToCssString()})";
    }
}

/// <summary>
/// Represents a min() function node in a calc() expression.
/// Returns the smallest (most negative) of its arguments.
/// </summary>
/// <remarks>
/// Per CSS Values Level 4 §10.2:
/// - min() contains one or more comma-separated calculations
/// - Returns the smallest (most negative) value
/// Docs: https://www.w3.org/TR/css-values-4/#comp-func
/// </remarks>
public sealed class CssCalcMinNode : CssCalcNode
{
    /// <summary>Gets the children of this min() function.</summary>
    public ImmutableArray<CssCalcNode> Children { get; }

    public override ECssCalcNodeType NodeType => ECssCalcNodeType.Min;

    public override ECssCalcResultType ResultType
    {
        get
        {
            if (Children.IsEmpty)
                return ECssCalcResultType.Invalid;

            // All children must have consistent types
            var firstType = Children[0].ResultType;
            if (firstType == ECssCalcResultType.Invalid)
                return ECssCalcResultType.Invalid;

            foreach (var child in Children)
            {
                var childType = child.ResultType;
                if (childType == ECssCalcResultType.Invalid)
                    return ECssCalcResultType.Invalid;

                // Numbers are compatible with anything
                if (childType == ECssCalcResultType.Number || firstType == ECssCalcResultType.Number)
                    continue;

                // Percentages are compatible with dimensions
                if (childType == ECssCalcResultType.Percentage || firstType == ECssCalcResultType.Percentage)
                    continue;

                // Types must match
                if (childType != firstType)
                    return ECssCalcResultType.Invalid;
            }

            // Find the first non-number type
            foreach (var child in Children)
            {
                var type = child.ResultType;
                if (type != ECssCalcResultType.Number)
                    return type;
            }

            return ECssCalcResultType.Number;
        }
    }

    public CssCalcMinNode(IEnumerable<CssCalcNode> children)
    {
        Children = children.ToImmutableArray();
        if (Children.IsEmpty)
            throw new ArgumentException("min() requires at least one argument", nameof(children));
    }

    public CssCalcMinNode(params CssCalcNode[] children)
    {
        Children = children.ToImmutableArray();
        if (Children.IsEmpty)
            throw new ArgumentException("min() requires at least one argument", nameof(children));
    }

    public override double? Evaluate(CssUnitResolver? resolver)
    {
        if (Children.IsEmpty)
            return null;

        double? minValue = null;
        foreach (var child in Children)
        {
            var value = child.Evaluate(resolver);
            if (!value.HasValue)
                return null;

            if (!minValue.HasValue || value.Value < minValue.Value)
                minValue = value.Value;
        }
        return minValue;
    }

    public override string ToCssString()
    {
        if (Children.IsEmpty)
            return "min()";

        var sb = new StringBuilder();
        sb.Append("min(");

        for (int i = 0; i < Children.Length; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(Children[i].ToCssString());
        }

        sb.Append(')');
        return sb.ToString();
    }
}

/// <summary>
/// Represents a max() function node in a calc() expression.
/// Returns the largest (most positive) of its arguments.
/// </summary>
/// <remarks>
/// Per CSS Values Level 4 §10.2:
/// - max() contains one or more comma-separated calculations
/// - Returns the largest (most positive) value
/// Docs: https://www.w3.org/TR/css-values-4/#comp-func
/// </remarks>
public sealed class CssCalcMaxNode : CssCalcNode
{
    /// <summary>Gets the children of this max() function.</summary>
    public ImmutableArray<CssCalcNode> Children { get; }

    public override ECssCalcNodeType NodeType => ECssCalcNodeType.Max;

    public override ECssCalcResultType ResultType
    {
        get
        {
            if (Children.IsEmpty)
                return ECssCalcResultType.Invalid;

            // All children must have consistent types
            var firstType = Children[0].ResultType;
            if (firstType == ECssCalcResultType.Invalid)
                return ECssCalcResultType.Invalid;

            foreach (var child in Children)
            {
                var childType = child.ResultType;
                if (childType == ECssCalcResultType.Invalid)
                    return ECssCalcResultType.Invalid;

                // Numbers are compatible with anything
                if (childType == ECssCalcResultType.Number || firstType == ECssCalcResultType.Number)
                    continue;

                // Percentages are compatible with dimensions
                if (childType == ECssCalcResultType.Percentage || firstType == ECssCalcResultType.Percentage)
                    continue;

                // Types must match
                if (childType != firstType)
                    return ECssCalcResultType.Invalid;
            }

            // Find the first non-number type
            foreach (var child in Children)
            {
                var type = child.ResultType;
                if (type != ECssCalcResultType.Number)
                    return type;
            }

            return ECssCalcResultType.Number;
        }
    }

    public CssCalcMaxNode(IEnumerable<CssCalcNode> children)
    {
        Children = children.ToImmutableArray();
        if (Children.IsEmpty)
            throw new ArgumentException("max() requires at least one argument", nameof(children));
    }

    public CssCalcMaxNode(params CssCalcNode[] children)
    {
        Children = children.ToImmutableArray();
        if (Children.IsEmpty)
            throw new ArgumentException("max() requires at least one argument", nameof(children));
    }

    public override double? Evaluate(CssUnitResolver? resolver)
    {
        if (Children.IsEmpty)
            return null;

        double? maxValue = null;
        foreach (var child in Children)
        {
            var value = child.Evaluate(resolver);
            if (!value.HasValue)
                return null;

            if (!maxValue.HasValue || value.Value > maxValue.Value)
                maxValue = value.Value;
        }
        return maxValue;
    }

    public override string ToCssString()
    {
        if (Children.IsEmpty)
            return "max()";

        var sb = new StringBuilder();
        sb.Append("max(");

        for (int i = 0; i < Children.Length; i++)
        {
            if (i > 0)
                sb.Append(", ");
            sb.Append(Children[i].ToCssString());
        }

        sb.Append(')');
        return sb.ToString();
    }
}

/// <summary>
/// Represents a clamp() function node in a calc() expression.
/// Clamps a value between a minimum and maximum.
/// </summary>
/// <remarks>
/// Per CSS Values Level 4 §10.2:
/// - clamp(MIN, VAL, MAX) is equivalent to max(MIN, min(VAL, MAX))
/// - If MIN conflicts with MAX, MIN wins
/// - MIN or MAX can be 'none' to indicate no clamping from that side
/// Docs: https://www.w3.org/TR/css-values-4/#comp-func
/// </remarks>
public sealed class CssCalcClampNode : CssCalcNode
{
    /// <summary>Gets the minimum value, or null if 'none'.</summary>
    public CssCalcNode? Min { get; }

    /// <summary>Gets the central value to be clamped.</summary>
    public CssCalcNode Value { get; }

    /// <summary>Gets the maximum value, or null if 'none'.</summary>
    public CssCalcNode? Max { get; }

    public override ECssCalcNodeType NodeType => ECssCalcNodeType.Clamp;

    public override ECssCalcResultType ResultType
    {
        get
        {
            // The central value must be valid
            var valueType = Value.ResultType;
            if (valueType == ECssCalcResultType.Invalid)
                return ECssCalcResultType.Invalid;

            // Check min compatibility if present
            if (Min is not null)
            {
                var minType = Min.ResultType;
                if (minType == ECssCalcResultType.Invalid)
                    return ECssCalcResultType.Invalid;

                // Must be compatible types
                if (minType != ECssCalcResultType.Number && valueType != ECssCalcResultType.Number &&
                    minType != ECssCalcResultType.Percentage && valueType != ECssCalcResultType.Percentage &&
                    minType != valueType)
                    return ECssCalcResultType.Invalid;
            }

            // Check max compatibility if present
            if (Max is not null)
            {
                var maxType = Max.ResultType;
                if (maxType == ECssCalcResultType.Invalid)
                    return ECssCalcResultType.Invalid;

                // Must be compatible types
                if (maxType != ECssCalcResultType.Number && valueType != ECssCalcResultType.Number &&
                    maxType != ECssCalcResultType.Percentage && valueType != ECssCalcResultType.Percentage &&
                    maxType != valueType)
                    return ECssCalcResultType.Invalid;
            }

            // Return the most specific type
            if (valueType != ECssCalcResultType.Number)
                return valueType;
            if (Min is not null && Min.ResultType != ECssCalcResultType.Number)
                return Min.ResultType;
            if (Max is not null && Max.ResultType != ECssCalcResultType.Number)
                return Max.ResultType;

            return ECssCalcResultType.Number;
        }
    }

    /// <summary>
    /// Creates a new clamp() node.
    /// </summary>
    /// <param name="min">The minimum value, or null for 'none'.</param>
    /// <param name="value">The central value to be clamped.</param>
    /// <param name="max">The maximum value, or null for 'none'.</param>
    public CssCalcClampNode(CssCalcNode? min, CssCalcNode value, CssCalcNode? max)
    {
        Min = min;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Max = max;
    }

    public override double? Evaluate(CssUnitResolver? resolver)
    {
        var value = Value.Evaluate(resolver);
        if (!value.HasValue)
            return null;

        double result = value.Value;

        // Apply max first (min wins if they conflict)
        if (Max is not null)
        {
            var maxValue = Max.Evaluate(resolver);
            if (!maxValue.HasValue)
                return null;
            result = Math.Min(result, maxValue.Value);
        }

        // Apply min (this wins if it conflicts with max)
        if (Min is not null)
        {
            var minValue = Min.Evaluate(resolver);
            if (!minValue.HasValue)
                return null;
            result = Math.Max(result, minValue.Value);
        }

        return result;
    }

    public override string ToCssString()
    {
        var sb = new StringBuilder();
        sb.Append("clamp(");

        if (Min is not null)
            sb.Append(Min.ToCssString());
        else
            sb.Append("none");

        sb.Append(", ");
        sb.Append(Value.ToCssString());
        sb.Append(", ");

        if (Max is not null)
            sb.Append(Max.ToCssString());
        else
            sb.Append("none");

        sb.Append(')');
        return sb.ToString();
    }
}

/// <summary>
/// Represents an immutable CSS calc() expression.
/// </summary>
/// <remarks>
/// Per CSS Values Level 4 §10.1:
/// - calc() contains a single calculation
/// - Standard operator precedence applies (* and / bind tighter than + and -)
/// - Operators are evaluated left-to-right within same precedence
/// Docs: https://www.w3.org/TR/css-values-4/#calc-func
/// </remarks>
public sealed class CssCalcExpression : IEquatable<CssCalcExpression>
{
    #region Fields
    /// <summary>
    /// The root node of the expression tree.
    /// </summary>
    private readonly CssCalcNode _root;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the root node of the expression tree.
    /// </summary>
    public CssCalcNode Root => _root;

    /// <summary>
    /// Gets the result type of this expression.
    /// </summary>
    public ECssCalcResultType ResultType => _root.ResultType;

    /// <summary>
    /// Gets whether this expression is valid (has compatible types).
    /// </summary>
    public bool IsValid => ResultType != ECssCalcResultType.Invalid;

    /// <summary>
    /// Gets whether this expression can be simplified to a single numeric value
    /// without additional context (no percentages or unresolvable dimensions).
    /// </summary>
    public bool CanSimplify
    {
        get
        {
            // Try to evaluate without a resolver
            return Evaluate(null).HasValue;
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new calc() expression from a root node.
    /// </summary>
    public CssCalcExpression(CssCalcNode root)
    {
        _root = root ?? throw new ArgumentNullException(nameof(root));
    }
    #endregion

    #region Evaluation
    /// <summary>
    /// Evaluates the expression to a numeric value if possible.
    /// </summary>
    /// <param name="resolver">Optional unit resolver for converting dimensions.</param>
    /// <returns>The evaluated value, or null if evaluation requires more context.</returns>
    public double? Evaluate(CssUnitResolver? resolver)
    {
        return _root.Evaluate(resolver);
    }

    /// <summary>
    /// Evaluates the expression using a percentage basis value.
    /// </summary>
    /// <param name="percentBasis">The value that 100% represents.</param>
    /// <param name="resolver">Optional unit resolver for converting dimensions.</param>
    /// <returns>The evaluated value.</returns>
    public double EvaluateWithPercentage(double percentBasis, CssUnitResolver? resolver)
    {
        // @todo: Implement percentage resolution by walking the tree
        // For now, attempt simple evaluation
        var result = Evaluate(resolver);
        return result ?? 0;
    }
    #endregion

    #region Serialization
    /// <summary>
    /// Returns the CSS serialization of this calc() expression.
    /// </summary>
    public string ToCssString()
    {
        return $"calc{_root.ToCssString()}";
    }

    /// <inheritdoc/>
    public override string ToString() => ToCssString();
    #endregion

    #region Equality
    /// <inheritdoc/>
    public bool Equals(CssCalcExpression? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        // Compare by CSS string representation
        return ToCssString() == other.ToCssString();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as CssCalcExpression);

    /// <inheritdoc/>
    public override int GetHashCode() => ToCssString().GetHashCode();

    public static bool operator ==(CssCalcExpression? left, CssCalcExpression? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(CssCalcExpression? left, CssCalcExpression? right)
        => !(left == right);
    #endregion
}
