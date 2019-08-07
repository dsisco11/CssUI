using CssUI.DOM.Internal;
using CssUI.DOM.Nodes;
using CssUI.HTML;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.DOM.CustomElements
{
    /// <summary>
    /// Attribute which indicates that the marked Operation, Setter, Deleter, etc is to be supplemented with additional steps in order to appropriately track and invoke custom element reactions.
    /// <para>Also provides utility functions related to wrapping and handling custom element related methods</para>
    /// </summary>
    public class CEReactions : Attribute
    {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#cereactions */

        /* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#custom-element-reaction-queue */



        public static void Upgrade_Element(Element element, ref CustomElementDefinition definition)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#upgrades */
            if (element.isCustom)
            {
                return;
            }

            if (element.CustomElementState == Enums.ECustomElement.Failed)
            {
                return;
            }

            element.Definition = new WeakReference<CustomElementDefinition>(definition);
            /* 4) For each attribute in element's attribute list, in order, enqueue a custom element callback reaction with element, callback name "attributeChangedCallback", and an argument list containing attribute's local name, null, attribute's value, and attribute's namespace. */
            foreach (Attr attr in element.AttributeList)
            {
                Enqueue_Reaction(element, EReactionName.AttributeChanged, attr.localName, null, attr.Value, attr.namespaceURI);
            }

            if (element.isConnected)
            {
                Enqueue_Reaction(element, EReactionName.Connected);
            }

            definition.constructionStack.Push(new WeakReference<Element>(element));

            var C = definition.constructor;
            /* 8) Run the following substeps while catching any exceptions: */
            try
            {
                if (definition.bDisableShadow && element.shadowRoot != null)
                {
                    throw new NotSupportedException($"Cannot upgrade a custom element which has a shadow root and whose definition has ShadowDOM disabled");
                }

                /* Notes:
                 * Okay so the specifications intended this system to be used by javascript, and because of this it relies on being able to sort of tack on prototype functions and stuff to an element object.
                 * We are implementing this (and using it) with C#, as such we cannot simply "tack-on" things to this element instance to upgrade it to this class.
                 * So we need to actually completely replace the element within the document with an instance of the class it should be which also contains all of its data & children...
                 * Fortunately creating a clone of this element and "stealing" its children will do exactly that!
                 */

                /* UPDATE: 
                 *      We cannot just replace the element because we cannot update any instances of it being referenced in memory, 
                 *      so we need to figure out a way we can do the equivalent of this upgrade to the actual live element instance in memory */

                throw new NotImplementedException($"Custom element upgrading is not yet supported");

                /* Clone this node to get an upgraded version of it *//*
                Element upgradeResult = (Element)Node._clone_node(element, null);

                *//* Steal all of the old nodes children *//*
                Node lastNode = null;
                foreach (Node child in element.childNodes)
                {
                    Node._insert_node_into_parent_before(child, upgradeResult, lastNode, true);
                    lastNode = child;
                }
                */
            }
            catch(Exception ex)
            {
                element.CustomElementState = Enums.ECustomElement.Failed;
                element.Definition = null;
                element.Custom_Element_Reaction_Queue.Clear();

                throw ex;
            }
            finally
            {
                definition.constructionStack.Pop();
            }

            /* 9) If element is a form-associated custom element, then: */
            if (element is FormAssociatedElement formElement)
            {
                /* 1) Reset the form owner of element. If element is associated with a form element, then enqueue a custom element callback reaction with element, callback name "formAssociatedCallback", and « the associated form ». */
                FormCommon.Reset_Form_Owner(element);
                if (formElement.form != null)
                {
                    Enqueue_Reaction(element, EReactionName.FormAssociated, formElement.form);
                }

                /* 2) If element is disabled, then enqueue a custom element callback reaction with element, callback name "formDisabledCallback" and « true ». */
                if (element is HTMLElement htmlElement && htmlElement.disabled)
                {
                    Enqueue_Reaction(element, EReactionName.FormDisabled, true);
                }
            }

            element.CustomElementState = Enums.ECustomElement.Custom;
        }


        public static void Try_Upgrade_Element(Element element)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#concept-try-upgrade */
            var definition = element.nodeDocument.defaultView.customElements.Lookup(element.nodeDocument, element.NamespaceURI, element.localName, element.is_value);
            if (definition != null)
            {
                Enqueue_Upgrade(element, definition);
            }
        }


        public static void Enqueue_Upgrade(Element element, CustomElementDefinition definition)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#enqueue-a-custom-element-upgrade-reaction */
            UpgradeReaction reaction = new UpgradeReaction(definition);
            element.Custom_Element_Reaction_Queue.Enqueue(reaction);
            element.ownerDocument.defaultView.Reactions.Enqueue_Element(element);
        }


        public static void Enqueue_Reaction(Element element, AtomicName<EReactionName> Reaction, params object[] Args)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#enqueue-a-custom-element-callback-reaction */
            if (!element.isCustom)
                return;
            /* 1) Let definition be element's custom element definition. */
            CustomElementDefinition def = element.ownerDocument.defaultView.customElements.Lookup(element.ownerDocument, element.NamespaceURI, element.tagName, element.is_value);
            if (def == null)
                return;

            /* 2) Let callback be the value of the entry in definition's lifecycle callbacks with key callbackName. */
            var callback = def.lifecycleCallbacks[Reaction];
            /* 3) If callback is null, then return */
            if (callback == null)
                return;

            /* 4) If callbackName is "attributeChangedCallback", then: */
            if (Reaction == EReactionName.AttributeChanged)
            {
                /* 1) Let attributeName be the first element of args. */
                AtomicName<EAttributeName> attributeName = Args[0] as AtomicName<EAttributeName>;
                /* 2) If definition's observed attributes does not contain attributeName, then return. */
                if (!def.observedAttributes.Contains(attributeName))
                    return;
            }

            /* 5) Add a new callback reaction to element's custom element reaction queue, with callback function callback and arguments args. */
            element.Custom_Element_Reaction_Queue.Enqueue(new ReactionCallback(callback, Args));
            /* 6) Enqueue an element on the appropriate element queue given element. */
            element.ownerDocument.defaultView.Reactions.Enqueue_Element(element);
        }


        /// <summary>
        /// Convenient wrapper for any Operations, attributes, setters, or deleters marked with [<see cref="CEReactions"/>]
        /// </summary>
        /// <param name="element">Element to queue this reaction for</param>
        /// <param name="wrappedMethod">The specifications dictate that for any [CEReaction] attributed method, the original steps given for said method must be encompassed by the callback reaction steps, this method is those original steps</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Wrap_CEReaction(Element element, Action wrappedMethod)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#cereactions */
            /* 1) Push a new element queue onto this object's relevant agent's custom element reactions stack. */
            var window = element.ownerDocument.defaultView;
            window.Reactions.Stack.Push(new Queue<Element>());

            /* 2) Run the originally-specified steps for this construct, catching any exceptions. If the steps return a value, let value be the returned value. If they throw an exception, let exception be the thrown exception. */
            Exception exception = null;
            try
            {
                wrappedMethod.Invoke();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            /* 3) Let queue be the result of popping from this object's relevant agent's custom element reactions stack. */
            var queue = window.Reactions.Stack.Pop();
            /* 4) Invoke custom element reactions in queue. */
            window.Reactions.Invoke_Reactions(queue);
            /* 5) If an exception exception was thrown by the original steps, rethrow exception. */
            if (exception != null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Convenient wrapper for any Operations, attributes, setters, or deleters marked with [<see cref="CEReactions"/>]
        /// </summary>
        /// <param name="element">Element to queue this reaction for</param>
        /// <param name="wrappedMethod">The specifications dictate that for any [CEReaction] attributed method, the original steps given for said method must be encompassed by the callback reaction steps, this method is those original steps</param>
        /// <returns>The returned value of <paramref name="wrappedMethod"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static dynamic Wrap_CEReaction(Element element, Func<dynamic> wrappedMethod)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#cereactions */
            /* 1) Push a new element queue onto this object's relevant agent's custom element reactions stack. */
            var window = element.ownerDocument.defaultView;
            window.Reactions.Stack.Push(new Queue<Element>());

            /* 2) Run the originally-specified steps for this construct, catching any exceptions. If the steps return a value, let value be the returned value. If they throw an exception, let exception be the thrown exception. */
            Exception exception = null;
            dynamic retValue = null;
            try
            {
                retValue = wrappedMethod.Invoke();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            /* 3) Let queue be the result of popping from this object's relevant agent's custom element reactions stack. */
            var queue = window.Reactions.Stack.Pop();
            /* 4) Invoke custom element reactions in queue. */
            window.Reactions.Invoke_Reactions(queue);
            /* 5) If an exception exception was thrown by the original steps, rethrow exception. */
            if (exception != null)
            {
                throw exception;
            }

            /* 6) If a value value was returned from the original steps, return value. */
            return retValue;
        }
    }
}
