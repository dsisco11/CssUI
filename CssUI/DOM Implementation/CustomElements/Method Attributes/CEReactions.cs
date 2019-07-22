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



        public static void Upgrade_Element(Element element)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#concept-try-upgrade */
        }


        public static void Enqueue_Reaction(Element element, AtomicName<EReactionName> Reaction, params object[] Args)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#enqueue-a-custom-element-callback-reaction */
            if (!element.Is_Custom)
                return;
            /* 1) Let definition be element's custom element definition. */
            CustomElementDefinition def = element.ownerDocument.defaultView.customElements.Lookup(element.tagName);
            if (ReferenceEquals(null, def))
                return;

            /* 2) Let callback be the value of the entry in definition's lifecycle callbacks with key callbackName. */
            var callback = def.lifecycleCallbacks[Reaction];
            /* 3) If callback is null, then return */
            if (ReferenceEquals(null, callback))
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
            if (!ReferenceEquals(null, exception))
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
            if (!ReferenceEquals(null, exception))
            {
                throw exception;
            }

            /* 6) If a value value was returned from the original steps, return value. */
            return retValue;
        }
    }
}
