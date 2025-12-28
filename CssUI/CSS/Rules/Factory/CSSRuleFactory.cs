using System;
using System.Collections.Generic;
using System.Linq;
using CssUI.CSS.Media;
using CssUI.CSS.Parser;
using CssUI.CSS.Selectors;
using CssUI.CSS.Serialization;

namespace CssUI.CSS;

/// <summary>
/// Factory for converting parsed CSS components into CSSOM rule objects.
/// Handles nested style rules and group rules per CSS Nesting Module Level 1.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-nesting-1/"/>
public static class CSSRuleFactory
{
    #region Public Entry Points

    /// <summary>
    /// Builds a list of CSS rules from parsed stylesheet content.
    /// </summary>
    /// <param name="components">The parsed CSS components from the parser.</param>
    /// <param name="parentStyleSheet">The parent stylesheet (optional).</param>
    /// <returns>A list of CSSOM rule objects.</returns>
    public static List<CSSRule> BuildRules(IEnumerable<CssComponent> components, CSSStyleSheet? parentStyleSheet = null)
    {
        var rules = new List<CSSRule>();
        foreach (var component in components)
        {
            var rule = BuildRule(component, parentRule: null, parentStyleSheet);
            if (rule is not null)
            {
                rules.Add(rule);
            }
        }
        return rules;
    }

    /// <summary>
    /// Builds a single CSS rule from a parsed component.
    /// </summary>
    /// <param name="component">The parsed CSS component.</param>
    /// <param name="parentRule">The parent rule for nested rules (null for top-level).</param>
    /// <param name="parentStyleSheet">The parent stylesheet.</param>
    /// <returns>The CSSOM rule object, or null if the component is not a valid rule.</returns>
    public static CSSRule? BuildRule(CssComponent component, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
    {
        return component switch
        {
            CssQualifiedRule qualifiedRule => BuildStyleRule(qualifiedRule, parentRule, parentStyleSheet),
            CssAtRule atRule => BuildAtRule(atRule, parentRule, parentStyleSheet),
            _ => null
        };
    }

    #endregion

    #region Style Rules

    /// <summary>
    /// Builds a CSSStyleRule from a parsed qualified rule.
    /// Processes nested rules and at-rules within the style block.
    /// </summary>
    private static CSSStyleRule? BuildStyleRule(CssQualifiedRule qualifiedRule, CSSRule? parentRule, CSSStyleSheet? parentStyleSheet)
    {
        // Parse the selector from the prelude
        var selectorText = string.Join("", qualifiedRule.Prelude.Select(t => t.Encode()));
        CssSelector? selector = null;
        if (!string.IsNullOrWhiteSpace(selectorText))
        {
            try
            {
                selector = new CssSelector(selectorText);
            }
            catch
            {
                // Invalid selector - rule is invalid
                return null;
            }
        }

        // Create rule first (needed for CSSStyleDeclaration constructor)
        var styleRule = new CSSStyleRule(selector, null, parentRule, parentStyleSheet);

        // Parse the block contents as style-block (allows declarations and nested rules)
        if (qualifiedRule.Block is not null)
        {
            // Block.Values already contains parsed component values - pass directly to parser
            var parser = new CssParser(qualifiedRule.Block.Values);
            var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock);

            foreach (var content in contents)
            {
                switch (content)
                {
                    case CssDecleration declaration:
                        // Declarations are stored in the style property
                        // Note: Full implementation would use CSSStyleDeclaration
                        break;

                    case CssQualifiedRule nestedQualified:
                        // Nested style rule - build recursively with this rule as parent
                        var nestedStyleRule = BuildStyleRule(nestedQualified, styleRule, parentStyleSheet);
                        if (nestedStyleRule is not null)
                        {
                            styleRule.cssRules.Add(nestedStyleRule);
                        }
                        break;

                    case CssAtRule nestedAt:
                        // Nested at-rule (@media, @supports, etc.) - build with context
                        var nestedAtRule = BuildNestedGroupRule(nestedAt, styleRule, parentStyleSheet);
                        if (nestedAtRule is not null)
                        {
                            styleRule.cssRules.Add(nestedAtRule);
                        }
                        break;
                }
            }
        }

        return styleRule;
    }

    #endregion

    #region At-Rules

    /// <summary>
    /// Builds a CSS at-rule from a parsed at-rule component.
    /// </summary>
    private static CSSRule? BuildAtRule(CssAtRule atRule, CSSRule? parentRule, CSSStyleSheet? parentStyleSheet)
    {
        var name = atRule.Name.ToLowerInvariant();

        return name switch
        {
            "media" => BuildMediaRule(atRule, parentRule, parentStyleSheet),
            "supports" => BuildSupportsRule(atRule, parentRule, parentStyleSheet),
            "layer" => BuildLayerRule(atRule, parentRule, parentStyleSheet),
            // @scope, @container can be added here in future
            _ => null // Unknown at-rule, ignore
        };
    }

    /// <summary>
    /// Builds a nested group rule (one that appears inside a style rule).
    /// Per CSS Nesting spec, these rules inherit the parent selector context.
    /// </summary>
    private static CSSRule? BuildNestedGroupRule(CssAtRule atRule, CSSStyleRule parentStyleRule, CSSStyleSheet? parentStyleSheet)
    {
        var name = atRule.Name.ToLowerInvariant();

        // For nested group rules, the contents are parsed as <style-block>
        // Properties directly in the group rule act as if wrapped in & { ... }
        return name switch
        {
            "media" => BuildNestedMediaRule(atRule, parentStyleRule, parentStyleSheet),
            "supports" => BuildNestedSupportsRule(atRule, parentStyleRule, parentStyleSheet),
            "layer" => BuildNestedLayerRule(atRule, parentStyleRule, parentStyleSheet),
            _ => null
        };
    }

    #endregion

    #region Media Rules

    /// <summary>
    /// Builds a top-level @media rule.
    /// </summary>
    private static CSSMediaRule BuildMediaRule(CssAtRule atRule, CSSRule? parentRule, CSSStyleSheet? parentStyleSheet)
    {
        // Parse media query list from prelude
        var mediaQueryText = string.Join("", atRule.Prelude.Select(t => t.Encode()));
        var mediaQueries = ParseMediaQueryList(mediaQueryText);

        var mediaRule = new CSSMediaRule(mediaQueries, parentRule, parentStyleSheet);

        // Parse the block contents as rule-list (top-level @media contains regular rules)
        if (atRule.Block is not null)
        {
            // Block.Values already contains parsed component values - pass directly to parser
            var parser = new CssParser(atRule.Block.Values);
            var contents = parser.Parse_Rule_List();

            foreach (var content in contents)
            {
                var childRule = BuildRule(content, mediaRule, parentStyleSheet);
                if (childRule is not null)
                {
                    mediaRule.cssRules.Add(childRule);
                }
            }
        }

        return mediaRule;
    }

    /// <summary>
    /// Builds a nested @media rule (inside a style rule).
    /// Per CSS Nesting, the contents are parsed as style-block, not rule-list.
    /// </summary>
    private static CSSMediaRule BuildNestedMediaRule(CssAtRule atRule, CSSStyleRule parentStyleRule, CSSStyleSheet? parentStyleSheet)
    {
        var mediaQueryText = string.Join("", atRule.Prelude.Select(t => t.Encode()));
        var mediaQueries = ParseMediaQueryList(mediaQueryText);

        var mediaRule = new CSSMediaRule(mediaQueries, parentStyleRule, parentStyleSheet);

        // Per CSS Nesting spec §2.2: nested group rule contents are parsed as <style-block>
        // Declarations directly in the @media act as if wrapped in & { ... }
        if (atRule.Block is not null)
        {
            var parser = new CssParser(atRule.Block.Values);
            var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock);

            // Collect declarations and nested rules separately
            var declarations = new List<CssDecleration>();
            var nestedRules = new List<CssComponent>();

            foreach (var content in contents)
            {
                if (content is CssDecleration decl)
                {
                    declarations.Add(decl);
                }
                else
                {
                    nestedRules.Add(content);
                }
            }

            // If there are declarations, wrap them in an implicit & { } rule
            if (declarations.Count > 0)
            {
                // The implicit rule uses & selector (nesting selector)
                // Create a CssSelector with just the nesting selector
                var implicitSelector = new CssSelector("&");
                var implicitRule = new CSSStyleRule(implicitSelector, null, mediaRule, parentStyleSheet);
                mediaRule.cssRules.Add(implicitRule);
            }

            // Process nested rules
            foreach (var rule in nestedRules)
            {
                var childRule = BuildRule(rule, mediaRule, parentStyleSheet);
                if (childRule is not null)
                {
                    mediaRule.cssRules.Add(childRule);
                }
            }
        }

        return mediaRule;
    }

    /// <summary>
    /// Parses a media query list from text.
    /// </summary>
    private static IEnumerable<MediaQuery> ParseMediaQueryList(string text)
    {
        // Basic parsing - use default media query for now
        // A full implementation would use a proper media query parser
        var queries = new List<MediaQuery>();
        var parts = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            if (!string.IsNullOrWhiteSpace(part))
            {
                // Create a media query with the text as the media type
                // More complex parsing would be needed for full media query support
                queries.Add(new MediaQuery(
                    EMediaQueryModifier.None,
                    ParseMediaType(part),
                    new LinkedList<IMediaCondition>()
                ));
            }
        }

        // If no queries parsed, add a default "all" query
        if (queries.Count == 0)
        {
            queries.Add(new MediaQuery(
                EMediaQueryModifier.None,
                EMediaType.All,
                new LinkedList<IMediaCondition>()
            ));
        }

        return queries;
    }

    /// <summary>
    /// Parses a media type keyword.
    /// </summary>
    private static EMediaType ParseMediaType(string text)
    {
        var trimmed = text.Trim().ToLowerInvariant();

        // Extract just the media type if there are conditions
        var spaceIndex = trimmed.IndexOf(' ');
        if (spaceIndex > 0)
        {
            trimmed = trimmed[..spaceIndex];
        }

        return trimmed switch
        {
            "all" => EMediaType.All,
            "screen" => EMediaType.Screen,
            "print" => EMediaType.Print,
            "speech" => EMediaType.Speech,
            _ => EMediaType.All // Default to "all" for unknown or complex queries
        };
    }

    #endregion

    #region Supports Rules

    /// <summary>
    /// Builds a top-level @supports rule.
    /// </summary>
    private static CSSSupportsRule BuildSupportsRule(CssAtRule atRule, CSSRule? parentRule, CSSStyleSheet? parentStyleSheet)
    {
        var conditionText = string.Join("", atRule.Prelude.Select(t => t.Encode())).Trim();
        var supportsRule = new CSSSupportsRule(conditionText, parentRule, parentStyleSheet);

        if (atRule.Block is not null)
        {
            var parser = new CssParser(atRule.Block.Values);
            var contents = parser.Parse_Rule_List();

            foreach (var content in contents)
            {
                var childRule = BuildRule(content, supportsRule, parentStyleSheet);
                if (childRule is not null)
                {
                    supportsRule.cssRules.Add(childRule);
                }
            }
        }

        return supportsRule;
    }

    /// <summary>
    /// Builds a nested @supports rule (inside a style rule).
    /// </summary>
    private static CSSSupportsRule BuildNestedSupportsRule(CssAtRule atRule, CSSStyleRule parentStyleRule, CSSStyleSheet? parentStyleSheet)
    {
        var conditionText = string.Join("", atRule.Prelude.Select(t => t.Encode())).Trim();
        var supportsRule = new CSSSupportsRule(conditionText, parentStyleRule, parentStyleSheet);

        // Same pattern as nested @media: parse as style-block
        if (atRule.Block is not null)
        {
            var parser = new CssParser(atRule.Block.Values);
            var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock);

            var declarations = new List<CssDecleration>();
            var nestedRules = new List<CssComponent>();

            foreach (var content in contents)
            {
                if (content is CssDecleration decl)
                {
                    declarations.Add(decl);
                }
                else
                {
                    nestedRules.Add(content);
                }
            }

            // Wrap declarations in implicit & { } rule
            if (declarations.Count > 0)
            {
                var implicitSelector = new CssSelector("&");
                var implicitRule = new CSSStyleRule(implicitSelector, null, supportsRule, parentStyleSheet);
                supportsRule.cssRules.Add(implicitRule);
            }

            foreach (var rule in nestedRules)
            {
                var childRule = BuildRule(rule, supportsRule, parentStyleSheet);
                if (childRule is not null)
                {
                    supportsRule.cssRules.Add(childRule);
                }
            }
        }

        return supportsRule;
    }

    #endregion

    #region Layer Rules

    /// <summary>
    /// Builds an @layer rule.
    /// </summary>
    private static CSSRule? BuildLayerRule(CssAtRule atRule, CSSRule? parentRule, CSSStyleSheet? parentStyleSheet)
    {
        var nameText = string.Join("", atRule.Prelude.Select(t => t.Encode())).Trim();

        // @layer can be either a statement (@layer foo, bar;) or a block (@layer foo { })
        if (atRule.Block is null)
        {
            // Statement form: @layer name1, name2;
            var names = nameText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return new CSSLayerStatementRule(names, parentRule, parentStyleSheet);
        }
        else
        {
            // Block form: @layer name { rules }
            var layerRule = new CSSLayerBlockRule(nameText, parentRule, parentStyleSheet);

            var parser = new CssParser(atRule.Block.Values);
            var contents = parser.Parse_Rule_List();

            foreach (var content in contents)
            {
                var childRule = BuildRule(content, layerRule, parentStyleSheet);
                if (childRule is not null)
                {
                    layerRule.cssRules.Add(childRule);
                }
            }

            return layerRule;
        }
    }

    /// <summary>
    /// Builds a nested @layer rule (inside a style rule).
    /// </summary>
    private static CSSLayerBlockRule BuildNestedLayerRule(CssAtRule atRule, CSSStyleRule parentStyleRule, CSSStyleSheet? parentStyleSheet)
    {
        var nameText = string.Join("", atRule.Prelude.Select(t => t.Encode())).Trim();
        var layerRule = new CSSLayerBlockRule(nameText, parentStyleRule, parentStyleSheet);

        if (atRule.Block is not null)
        {
            var parser = new CssParser(atRule.Block.Values);
            var contents = parser.Parse_Block_Contents(ECssBlockContentsType.StyleBlock);

            var declarations = new List<CssDecleration>();
            var nestedRules = new List<CssComponent>();

            foreach (var content in contents)
            {
                if (content is CssDecleration decl)
                {
                    declarations.Add(decl);
                }
                else
                {
                    nestedRules.Add(content);
                }
            }

            // Wrap declarations in implicit & { } rule
            if (declarations.Count > 0)
            {
                var implicitSelector = new CssSelector("&");
                var implicitRule = new CSSStyleRule(implicitSelector, null, layerRule, parentStyleSheet);
                layerRule.cssRules.Add(implicitRule);
            }

            foreach (var rule in nestedRules)
            {
                var childRule = BuildRule(rule, layerRule, parentStyleSheet);
                if (childRule is not null)
                {
                    layerRule.cssRules.Add(childRule);
                }
            }
        }

        return layerRule;
    }

    #endregion
}
