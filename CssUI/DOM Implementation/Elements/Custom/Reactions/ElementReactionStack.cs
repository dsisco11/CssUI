using System.Collections.Generic;
using System.Threading.Tasks;

namespace CssUI.DOM.CustomElements
{
    /// <summary>
    /// A Custom element reactions stack
    /// </summary>
    internal sealed class ElementReactionStack
    {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#concept-custom-element-reaction */
        #region Properties
        internal bool bProcessing_Backup_Element_Queue = false;
        /// <summary>
        /// The backup element queue
        /// </summary>
        internal Queue<Element> Backup_Queue = new Queue<Element>();
        /// <summary>
        /// The custom element reactions stack
        /// </summary>
        internal Stack<Queue<Element>> Stack = new Stack<Queue<Element>>();

        private readonly Window window;
        private Task processingTask = null;
        #endregion

        #region Accessors
        /// <summary>
        /// The current element queue is the element queue at the top of the custom element reactions stack
        /// </summary>
        internal Queue<Element> Current_Queue => Stack.Peek();
        #endregion

        #region Constructor
        public ElementReactionStack(Window window)
        {
            this.window = window;
        }
        #endregion

        /// <summary>
        /// Pushes a new elements queue onto the reactions stack.
        /// </summary>
        internal void Push_New_Element_Queue()
        {
            Stack.Push(new Queue<Element>());
        }

        /// <summary>
        /// Enqueues an element on a windows custom element queue so its' custom reaction events may be processed
        /// </summary>
        /// <param name="window"></param>
        /// <param name="element"></param>
        internal void Enqueue_Element(Element element)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#enqueue-an-element-on-the-appropriate-element-queue */
            var reactionsStack = Stack;
            if (reactionsStack.Count <= 0)
            {
                Backup_Queue.Enqueue(element);

                if (bProcessing_Backup_Element_Queue)
                    return;

                bProcessing_Backup_Element_Queue = true;
                processingTask = Task.Factory.StartNew(_Process_Backup_Element_Queue);
            }
            else
            {
                Current_Queue.Enqueue(element);
            }
        }

        internal void Invoke_Reactions(Queue<Element> queue)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#invoke-custom-element-reactions */
            foreach (Element element in queue)
            {
                var reactions = element.Custom_Element_Reaction_Queue;
                while (reactions.Count > 0)
                {
                    IElementReaction reaction = reactions.Dequeue();
                    reaction.Handle(element);
                }
            }
        }


        #region Tasks

        /// <summary>
        /// Task logic for processing a windows backup element queue
        /// </summary>
        private void _Process_Backup_Element_Queue()
        {
            /* 1) Invoke custom element reactions in reactionsStack's backup element queue. */
            Invoke_Reactions(Backup_Queue);
            /* 2) Unset reactionsStack's processing the backup element queue flag. */
            bProcessing_Backup_Element_Queue = false;
        }
        #endregion
    }
}
