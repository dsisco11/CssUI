using System;
using System.Collections.Generic;
using CssUI.CSS.Parser;

namespace CssUI.CSS.Functions
{
    /// <summary>
    /// Manages CSS Custom Properties (CSS Variables).
    /// Spec: https://www.w3.org/TR/css-variables-1/
    /// </summary>
    public class CssCustomPropertyRegistry
    {
        #region Fields

        private readonly Dictionary<string, CssValue> _properties = new Dictionary<string, CssValue>(StringComparer.OrdinalIgnoreCase);
        private readonly CssCustomPropertyRegistry _parent;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a root custom property registry (no parent).
        /// </summary>
        public CssCustomPropertyRegistry()
        {
            _parent = null;
        }

        /// <summary>
        /// Creates a custom property registry with inheritance from a parent.
        /// </summary>
        /// <param name="parent">The parent registry (from parent element)</param>
        public CssCustomPropertyRegistry(CssCustomPropertyRegistry parent)
        {
            _parent = parent;
        }

        #endregion

        #region Property Management

        /// <summary>
        /// Sets a custom property value.
        /// </summary>
        /// <param name="name">Property name (e.g., "--my-color")</param>
        /// <param name="value">The CSS value</param>
        public void Set(string name, CssValue value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            
            if (!IsValidCustomPropertyName(name))
                throw new ArgumentException($"Invalid custom property name: {name}. Must start with '--'", nameof(name));

            _properties[name] = value;
        }

        /// <summary>
        /// Gets a custom property value, checking parent registries if not found locally.
        /// </summary>
        /// <param name="name">Property name (e.g., "--my-color")</param>
        /// <returns>The value, or null if not found</returns>
        public CssValue Get(string name)
        {
            if (_properties.TryGetValue(name, out var value))
            {
                return value;
            }

            // Check parent (inheritance)
            return _parent?.Get(name);
        }

        /// <summary>
        /// Checks if a custom property is defined (locally or inherited).
        /// </summary>
        public bool Contains(string name)
        {
            if (_properties.ContainsKey(name))
                return true;

            return _parent?.Contains(name) ?? false;
        }

        /// <summary>
        /// Removes a custom property from this registry.
        /// </summary>
        public bool Remove(string name)
        {
            return _properties.Remove(name);
        }

        /// <summary>
        /// Clears all locally defined custom properties.
        /// </summary>
        public void Clear()
        {
            _properties.Clear();
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates that a name is a valid custom property name (starts with "--").
        /// </summary>
        public static bool IsValidCustomPropertyName(string name)
        {
            return !string.IsNullOrEmpty(name) && name.StartsWith("--", StringComparison.Ordinal);
        }

        #endregion

        #region Enumeration

        /// <summary>
        /// Gets all locally defined custom property names.
        /// </summary>
        public IEnumerable<string> LocalPropertyNames => _properties.Keys;

        /// <summary>
        /// Gets all custom property names (local + inherited).
        /// </summary>
        public IEnumerable<string> AllPropertyNames
        {
            get
            {
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                
                // Add local properties
                foreach (var name in _properties.Keys)
                {
                    seen.Add(name);
                    yield return name;
                }

                // Add inherited properties not overridden locally
                if (_parent != null)
                {
                    foreach (var name in _parent.AllPropertyNames)
                    {
                        if (!seen.Contains(name))
                        {
                            seen.Add(name);
                            yield return name;
                        }
                    }
                }
            }
        }

        #endregion
    }
}

