using System;
using System.Collections.Generic;
using CssUI.CSS.Parser;

namespace CssUI.CSS.Functions
{
    /// <summary>
    /// Resolves CSS var() function references to their computed values.
    /// Spec: https://www.w3.org/TR/css-variables-1/#using-variables
    /// </summary>
    public static class CssVarResolver
    {
        /// <summary>
        /// Maximum depth for var() resolution to prevent infinite loops.
        /// </summary>
        private const int MaxResolutionDepth = 100;

        /// <summary>
        /// Resolves a var() function to its value.
        /// </summary>
        /// <param name="function">The CssFunction representing var()</param>
        /// <param name="registry">The custom property registry to look up values</param>
        /// <param name="depth">Current resolution depth (for cycle detection)</param>
        /// <returns>The resolved CssValue, or null if resolution fails</returns>
        internal static CssValue Resolve(CssFunction function, CssCustomPropertyRegistry registry, int depth = 0)
        {
            if (function == null)
                return null;

            if (!string.Equals(function.Name, "var", StringComparison.OrdinalIgnoreCase))
                return null;

            if (registry == null)
                return null;

            if (depth > MaxResolutionDepth)
            {
                // Cycle detected or too deeply nested
                return null;
            }

            // Parse var() arguments: var(--property-name, fallback)
            var (propertyName, fallback) = ParseVarArguments(function.Arguments);

            if (string.IsNullOrEmpty(propertyName))
                return null;

            // Look up the custom property
            var value = registry.Get(propertyName);

            if (value != null && !value.IsNull)
            {
                // If the value contains var() references, resolve them recursively
                return ResolveNestedVars(value, registry, depth + 1);
            }

            // Property not found or null - use fallback if provided
            if (fallback != null)
            {
                return ResolveNestedVars(fallback, registry, depth + 1);
            }

            // No value and no fallback - return null (invalid)
            return null;
        }

        /// <summary>
        /// Parses var() function arguments to extract property name and optional fallback.
        /// </summary>
        private static (string propertyName, CssValue fallback) ParseVarArguments(List<CssToken> arguments)
        {
            if (arguments == null || arguments.Count == 0)
                return (null, null);

            string propertyName = null;
            CssValue fallback = null;
            bool foundComma = false;
            var fallbackTokens = new List<CssToken>();

            foreach (var token in arguments)
            {
                if (token.Type == ECssTokenType.Whitespace)
                    continue;

                if (!foundComma)
                {
                    if (token.Type == ECssTokenType.Comma)
                    {
                        foundComma = true;
                        continue;
                    }

                    // First argument should be the property name (an ident starting with --)
                    if (token.Type == ECssTokenType.Ident && token is IdentToken identToken)
                    {
                        propertyName = identToken.Value;
                    }
                    else if (token is ValuedTokenBase valuedToken)
                    {
                        // Could be a dashed-ident
                        propertyName = valuedToken.Value;
                    }
                }
                else
                {
                    // Everything after the comma is the fallback
                    fallbackTokens.Add(token);
                }
            }

            // Parse fallback tokens into a CssValue if present
            if (fallbackTokens.Count > 0)
            {
                // Simple fallback parsing - just take the first meaningful token
                foreach (var token in fallbackTokens)
                {
                    if (token.Type == ECssTokenType.Whitespace)
                        continue;

                    switch (token.Type)
                    {
                        case ECssTokenType.Number:
                            if (token is NumberToken numToken)
                            {
                                fallback = CssValue.From(Convert.ToDouble(numToken.Number));
                            }
                            break;
                        case ECssTokenType.Dimension:
                            if (token is DimensionToken dimToken)
                            {
                                // Create dimension value
                                fallback = CssValue.From_CSS(dimToken.Encode());
                            }
                            break;
                        case ECssTokenType.Percentage:
                            if (token is PercentageToken pctToken)
                            {
                                fallback = CssValue.From_Percent(pctToken.Number);
                            }
                            break;
                        case ECssTokenType.Hash:
                            // Color value
                            fallback = CssValue.From_CSS(token.Encode());
                            break;
                        case ECssTokenType.Ident:
                            if (token is IdentToken ident)
                            {
                                fallback = CssValue.From_CSS(ident.Value);
                            }
                            break;
                        case ECssTokenType.String:
                            if (token is StringToken strToken)
                            {
                                fallback = CssValue.From_CSS($"\"{strToken.Value}\"");
                            }
                            break;
                    }

                    if (fallback != null)
                        break;
                }
            }

            return (propertyName, fallback);
        }

        /// <summary>
        /// Resolves any nested var() references within a value.
        /// </summary>
        private static CssValue ResolveNestedVars(CssValue value, CssCustomPropertyRegistry registry, int depth)
        {
            if (value == null || value.IsNull)
                return value;

            // If this value is a var() function, resolve it
            if (value.Type == ECssValueTypes.FUNCTION)
            {
                // Get the function from the value
                var func = value.AsFunction();
                if (func != null && string.Equals(func.Name, "var", StringComparison.OrdinalIgnoreCase))
                {
                    return Resolve(func, registry, depth);
                }
            }

            // Value doesn't contain var() - return as-is
            return value;
        }

        /// <summary>
        /// Checks if a value contains var() references that need resolution.
        /// </summary>
        public static bool ContainsVarReference(CssValue value)
        {
            if (value == null || value.IsNull)
                return false;

            if (value.Type == ECssValueTypes.FUNCTION)
            {
                var func = value.AsFunction();
                return func != null && string.Equals(func.Name, "var", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
