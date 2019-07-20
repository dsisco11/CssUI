using CssUI.DOM.Internal;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CssUI.DOM.CustomElements
{
    public delegate HTMLElement CustomElementConstructor();

    public sealed class CustomElementRegistry
    {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#customelementregistry */
        #region Properties
        private List<CustomElementDefinition> Definitions = new List<CustomElementDefinition>();
        private bool bDefinitionIsRunning = false;
        private readonly Dictionary<string, TaskCompletionSource<CustomElementDefinition>> PromiseMap = new Dictionary<string, TaskCompletionSource<CustomElementDefinition>>();
        #endregion

        #region Constructor
        public CustomElementRegistry()
        {
        }
        #endregion

        [CEReactions]
        public void define(string name, CustomElementConstructor constructor, ElementDefinitionOptions options);

        /// <summary>
        /// Retrieves the custom element constructor defined for the given name. Returns undefined if there is no custom element definition with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Action get(string name)
        {
            var definition = Definitions.FirstOrDefault(def => def.name.Equals(name));
            return definition?.constructor;
        }

        /// <summary>
        /// Returns a promise that will be fulfilled when a custom element becomes defined with the given name. 
        /// (If such a custom element is already defined, the returned promise will be immediately fulfilled.) 
        /// Returns a promise rejected with a "SyntaxError" DOMException if not given a valid custom element name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TaskCompletionSource<CustomElementDefinition> whenDefined(string name)
        {

        }

        [CEReactions]
        internal void upgrade(Node root);
    }
}
