using CssUI.DOM.Exceptions;
using CssUI.DOM.Internal;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CssUI.DOM.CustomElements
{
    public delegate HTMLElement CustomElementConstructor();

    /// <summary>
    /// 
    /// </summary>
    public sealed class CustomElementRegistry
    {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#customelementregistry */
        #region Properties
        private List<CustomElementDefinition> Definitions = new List<CustomElementDefinition>();
        private bool bDefinitionIsRunning = false;
        private readonly Dictionary<AtomicString, TaskCompletionSource<CustomElementDefinition>> PromiseMap = new Dictionary<AtomicString, TaskCompletionSource<CustomElementDefinition>>();
        private readonly Window window;
        #endregion

        #region Constructor
        public CustomElementRegistry(Window window)
        {
            this.window = window;
        }
        #endregion

        public CustomElementDefinition this[int index]
        {
            get => Definitions[index];
        }

        public CustomElementDefinition Lookup(Document document, string NamespaceURI, string localName, string isValue)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#look-up-a-custom-element-definition */
            if (!StringCommon.StrEq(NamespaceURI.AsSpan(), DOMCommon.HTMLNamespace.AsSpan()))
            {
                return null;
            }

            if (document?.defaultView == null)
            {
                return null;
            }

            /* 4) If there is custom element definition in registry with name and local name both equal to localName, return that custom element definition. */
            /* 5) If there is a custom element definition in registry with name equal to is and local name equal to localName, return that custom element definition. */
            foreach (CustomElementDefinition definition in Definitions)
            {
                if (StringCommon.StrEq(localName.AsSpan(), definition.name.AsSpan()) && StringCommon.StrEq(localName.AsSpan(), definition.localName.AsSpan()))
                {
                    return definition;
                }

                if (StringCommon.StrEq(isValue.AsSpan(), definition.name.AsSpan()) && StringCommon.StrEq(localName.AsSpan(), definition.localName.AsSpan()))
                {
                    return definition;
                }
            }

            /* 6) Return null. */
            return null;
        }


        /// <summary>
        /// Defines a new custom element, mapping the given name to the given constructor as an autonomous custom element.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="constructor"></param>
        /// <param name="options"></param>
        [CEReactions]
        public void define(string name, CustomElementConstructor constructor, ElementDefinitionOptions options)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#dom-customelementregistry-define */

            /* 1) If IsConstructor(constructor) is false, then throw a TypeError. */
            if (constructor == null)
            {
                throw new TypeError();
            }
            /* 2) If name is not a valid custom element name, then throw a "SyntaxError" DOMException. */
            if (!HTMLCommon.Is_Valid_Custom_Element_Name(name.AsMemory()))
            {
                throw new DomSyntaxError($"\"{name}\" is not a valid custom element name!");
            }
            /* 3) If this CustomElementRegistry contains an entry with name name, then throw a "NotSupportedError" DOMException. */
            if (Definitions.Any(def => StringCommon.StrEq(def.name.AsSpan(), name.AsSpan())))
            {
                throw new NotSupportedError($"A custom element definition for \"{name}\" already exists");
            }
            /* 4) If this CustomElementRegistry contains an entry with constructor constructor, then throw a "NotSupportedError" DOMException. */
            if (Definitions.Any(def => def.constructor.Equals(constructor) ))
            {
                throw new NotSupportedError($"A custom element definition using that constructor already exists");
            }

            var localName = name;
            string extends = options?.extends;

            if (!ReferenceEquals(null, extends))
            {
                /* 1) If extends is a valid custom element name, then throw a "NotSupportedError" DOMException. */
                if (HTMLCommon.Is_Valid_Custom_Element_Name(extends.AsMemory()))
                {
                    throw new NotSupportedError($"Custom element cannot extend \"{extends}\"");
                }
                /* 2) If the element interface for extends and the HTML namespace is HTMLUnknownElement (e.g., if extends does not indicate an element definition in this specification), then throw a "NotSupportedError" DOMException. */
                Type interfaceType = DOMCommon.Lookup_Element_Interface(extends, DOMCommon.HTMLNamespace);
                if (interfaceType == typeof(HTMLUnknownElement))
                {
                    throw new NotSupportedError($"Cannot extend non existant element type \"{extends}\"");
                }
                /* 3) Set localName to extends. */
                localName = extends;
            }
            /* 8) If this CustomElementRegistry's element definition is running flag is set, then throw a "NotSupportedError" DOMException. */
            if (bDefinitionIsRunning)
            {
                throw new NotSupportedError($"{nameof(CustomElementRegistry)} definition reentry");
            }
            /* 9) Set this CustomElementRegistry's element definition is running flag. */
            bDefinitionIsRunning = true;

            bool formAssociated = false;
            bool disableInternals = false;
            bool disableShadow = false;
            bool observeAllAttributes = false;
            HashSet<AtomicName<EAttributeName>> observedAttributes = new HashSet<AtomicName<EAttributeName>>();
            Dictionary<AtomicName<EReactionName>, ReactionHandler> lifecycleCallbacks = CustomElementDefinition.New_LifecycleCallbacks_Dictionary();

            /* 14) Run the following substeps while catching any exceptions: */
            /* Note: the method for this whole process was written with the intention that it would be used by javascript, however we are using custom elements in a very different implementation scenario.
             *      so our method for doing all this will be different.
             */

            try
            {
                /* 5) If the value of the entry in lifecycleCallbacks with key "attributeChangedCallback" is not null, then: */
                if (!options.lifecycleCallbacks.TryGetValue(EReactionName.AttributeChanged, out ReactionHandler attrChangeCallback))
                {
                    observeAllAttributes = options.observeAllAttributes;
                    if (options.observedAttributes != null)
                    {
                        observedAttributes = options.observedAttributes;
                    }
                }

                disableInternals = options.bDisableInternals;
                disableShadow = options.bDisableShadow;
                ReactionHandler outHandle;

                if (options.lifecycleCallbacks.TryGetValue(EReactionName.Connected, out outHandle) && outHandle != null)
                    lifecycleCallbacks[EReactionName.Connected] = outHandle;

                if (options.lifecycleCallbacks.TryGetValue(EReactionName.Disconnected, out outHandle) && outHandle != null)
                    lifecycleCallbacks[EReactionName.Disconnected] = outHandle;

                if (options.lifecycleCallbacks.TryGetValue(EReactionName.Adopted, out outHandle) && outHandle != null)
                    lifecycleCallbacks[EReactionName.Adopted] = outHandle;

                if (options.lifecycleCallbacks.TryGetValue(EReactionName.AttributeChanged, out outHandle) && outHandle != null)
                    lifecycleCallbacks[EReactionName.AttributeChanged] = outHandle;



                formAssociated = options.bFormAssociated;
                if (formAssociated)
                {
                    if (options.lifecycleCallbacks.TryGetValue(EReactionName.FormAssociated, out outHandle) && outHandle != null)
                        lifecycleCallbacks[EReactionName.FormAssociated] = outHandle;

                    if (options.lifecycleCallbacks.TryGetValue(EReactionName.FormDisabled, out outHandle) && outHandle != null)
                        lifecycleCallbacks[EReactionName.FormDisabled] = outHandle;

                    if (options.lifecycleCallbacks.TryGetValue(EReactionName.FormReset, out outHandle) && outHandle != null)
                        lifecycleCallbacks[EReactionName.FormReset] = outHandle;

                    if (options.lifecycleCallbacks.TryGetValue(EReactionName.FormStateRestore, out outHandle) && outHandle != null)
                        lifecycleCallbacks[EReactionName.FormStateRestore] = outHandle;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {/* Then, perform the following substep, regardless of whether the above steps threw an exception or not: */
                bDefinitionIsRunning = false;
            }

            /* 15) Let definition be a new custom element definition with name name, local name localName, constructor constructor, observed attributes observedAttributes, lifecycle callbacks lifecycleCallbacks, form-associated formAssociated, disable internals disableInternals, and disable shadow disableShadow. */
            CustomElementDefinition definition = new CustomElementDefinition(name, localName, constructor, observedAttributes, lifecycleCallbacks, observeAllAttributes, formAssociated, disableInternals, disableShadow);
            Definitions.Add(definition);

            Document document = window.activeDocument;
            FilterLocalName_Namespace Filter = new FilterLocalName_Namespace(localName, DOMCommon.HTMLNamespace);
            IReadOnlyCollection<Node> upgradeCandidates = DOMCommon.Get_Shadow_Including_Descendents(document.documentElement, Filter, Enums.ENodeFilterMask.SHOW_ELEMENT);

            /* 19) For each element element in upgrade candidates, enqueue a custom element upgrade reaction given element and definition. */
            foreach (Node candidate in upgradeCandidates)
            {
                CEReactions.Enqueue_Upgrade(candidate as Element, definition);
            }

            /* 20) If this CustomElementRegistry's when-defined promise map contains an entry with key name: */
            if (PromiseMap.TryGetValue(name, out TaskCompletionSource<CustomElementDefinition> outPromise))
            {
                outPromise.TrySetResult(definition);
                PromiseMap.Remove(name);
            }
        }

        /// <summary>
        /// Retrieves the custom element constructor defined for the given name. Returns undefined if there is no custom element definition with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CustomElementConstructor get(string name)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#dom-customelementregistry-get */
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
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#dom-customelementregistry-whendefined */
            if (!HTMLCommon.Is_Valid_Custom_Element_Name(name.AsMemory()))
            {
                throw new DomSyntaxError($"\"{name}\" is not a valid name for a custom element");
            }

            if (PromiseMap.TryGetValue(name, out TaskCompletionSource<CustomElementDefinition> outPromise))
            {
                var result = new TaskCompletionSource<CustomElementDefinition>();
                result.TrySetResult(null);
                return result;
            }
            /* 4) If map does not contain an entry with key name, create an entry in map with key name and whose value is a new promise. */
            else
            {
                var newPromise = new TaskCompletionSource<CustomElementDefinition>();
                PromiseMap.Add(name, newPromise);
                return newPromise;
            }
        }

        /// <summary>
        /// Tries to upgrade all shadow-including inclusive descendant elements of root, even if they are not connected.
        /// </summary>
        /// <param name="root"></param>
        [CEReactions]
        internal void upgrade(Node root)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#dom-customelementregistry-upgrade */
            var candidates = DOMCommon.Get_Shadow_Including_Inclusive_Descendents(root, null, Enums.ENodeFilterMask.SHOW_ELEMENT);
            foreach (Node candidate in candidates)
            {
                Element element = candidate as Element;
                CEReactions.Wrap_CEReaction(element, () =>
                {
                    CEReactions.Try_Upgrade_Element(element);
                });
            }
        }
    }
}
