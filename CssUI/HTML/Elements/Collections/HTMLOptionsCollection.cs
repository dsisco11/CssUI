﻿using CssUI.DOM;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.HTML
{
    public class HTMLOptionsCollection : HTMLCollection<HTMLOptionElement>
    {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#htmloptionscollection */
        #region Properties
        #endregion

        #region Constructor
        public HTMLOptionsCollection(HTMLSelectElement root) : base(root)
        {
        }
        #endregion

        #region Accessors
        #endregion

        /// <summary>
        /// Returns the number of elements in the collection.
        /// When set to a smaller number, truncates the number of option elements in the corresponding container.
        /// When set to a greater number, adds new blank option elements to that container.
        /// </summary>
        [CEReactions]
        public override int length// shadows inherited length
        {
            get => Collection.Count;
            set
            {
                CEReactions.Wrap_CEReaction(root.nodeDocument.defaultView, () =>
                {
                    var items = Collection;
                    int len = items.Count;

                    /* When set to a smaller number, truncates the number of option elements in the corresponding container. */
                    if (value < len)
                    {
                        for (int i = len - 1; i > value; i--)
                        {
                            var rmv = items[i];
                            root.removeChild(rmv);
                        }
                    }

                    /* When set to a greater number, adds new blank option elements to that container. */
                    if (value > len)
                    {
                        var add = value - len;
                        for (int i = 0; i < add; i++)
                        {
                            var opt = new HTMLOptionElement(root.nodeDocument);
                            root.appendChild(opt);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Returns the item with index index from the collection. The items are sorted in tree order.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [CEReactions]
        public override HTMLOptionElement this[int index]
        {
            get => Collection[index];
            set
            {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#dom-htmloptionscollection-setter */
                CEReactions.Wrap_CEReaction(root.nodeDocument.defaultView, () =>
                {
                    if (value == null)
                    {
                        remove(index);
                        return;
                    }

                    var n = index - length;
                    if (n > 0)
                    {
                        var frag = new DocumentFragment(root, root.nodeDocument);
                        for (int i = 0; i <= n - 1; i++)
                        {
                            var opt = new HTMLOptionElement(root.nodeDocument);
                            frag.append(opt);
                        }

                        root.appendChild(frag);
                    }

                    if (n >= 0)
                    {
                        root.appendChild(value);
                    }
                    else
                    {
                        root.replaceChild(value, Collection[index]);
                    }
                });
            }
        }


        /// <summary>
        /// Inserts element before the node given by before.
        /// The before argument can be a number, in which case element is inserted before the item with that number, or an element from the collection, in which case element is inserted before that element.
        /// If before is omitted, null, or a number out of range, then element will be added at the end of the list.
        /// This method will throw a "HierarchyRequestError" DOMException if element is an ancestor of the element into which it is to be inserted.
        /// </summary>
        /// <param name="element">Element to add</param>
        /// <param name="before">location to insert element at</param>
        [CEReactions]
        public void add(HTMLOptionElement element, long before)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#dom-htmloptionscollection-add */
            CEReactions.Wrap_CEReaction(root.nodeDocument.defaultView, () =>
            {
                if (DOMCommon.Is_Ancestor(element, root))
                {
                    throw new HierarchyRequestError("Cannot add ancestor of Select element to its descendants");
                }

                Node reference = null;
                var items = Collection;
                if (before < items.Count)
                {
                    reference = items[(int)before];
                }

                Node parent = null;
                if (reference != null)
                {
                    parent = reference.parentNode;
                }
                else
                {
                    parent = root;
                }

                Node.Dom_pre_insert_node(element, parent, reference);
            });
        }

        /// <summary>
        /// Inserts element before the node given by before.
        /// The before argument can be a number, in which case element is inserted before the item with that number, or an element from the collection, in which case element is inserted before that element.
        /// If before is omitted, null, or a number out of range, then element will be added at the end of the list.
        /// This method will throw a "HierarchyRequestError" DOMException if element is an ancestor of the element into which it is to be inserted.
        /// </summary>
        /// <param name="element">Element to add</param>
        /// <param name="before">location to insert element at</param>
        [CEReactions]
        public void add(HTMLOptionElement element, HTMLElement before = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#dom-htmloptionscollection-add */
            CEReactions.Wrap_CEReaction(root.nodeDocument.defaultView, () =>
            {
                if (DOMCommon.Is_Ancestor(element, root))
                {
                    throw new HierarchyRequestError("Cannot add ancestor of Select element to its descendants");
                }

                if (!DOMCommon.Is_Descendant(before, root))
                {
                    throw new HierarchyRequestError("the element specified for 'before' must be a descendant of the Select element");
                }

                if (ReferenceEquals(element, before))
                {
                    return;
                }

                Node reference = before;
                Node parent = null;
                if (reference != null)
                {
                    parent = reference.parentNode;
                }
                else
                {
                    parent = root;
                }

                Node.Dom_pre_insert_node(element, parent, reference);
            });
        }

        /// <summary>
        /// Inserts element before the node given by before.
        /// The before argument can be a number, in which case element is inserted before the item with that number, or an element from the collection, in which case element is inserted before that element.
        /// If before is omitted, null, or a number out of range, then element will be added at the end of the list.
        /// This method will throw a "HierarchyRequestError" DOMException if element is an ancestor of the element into which it is to be inserted.
        /// </summary>
        /// <param name="element">Element to add</param>
        /// <param name="before">location to insert element at</param>
        [CEReactions]
        public void add(HTMLOptGroupElement element, long before)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#dom-htmloptionscollection-add */
            CEReactions.Wrap_CEReaction(root.nodeDocument.defaultView, () =>
            {
                if (DOMCommon.Is_Ancestor(element, root))
                {
                    throw new HierarchyRequestError("Cannot add ancestor of Select element to its descendants");
                }

                Node reference = null;
                var items = Collection;
                if (before < items.Count)
                {
                    reference = items[(int)before];
                }

                Node parent = null;
                if (reference != null)
                {
                    parent = reference.parentNode;
                }
                else
                {
                    parent = root;
                }

                Node.Dom_pre_insert_node(element, parent, reference);
            });
        }

        /// <summary>
        /// Inserts element before the node given by before.
        /// The before argument can be a number, in which case element is inserted before the item with that number, or an element from the collection, in which case element is inserted before that element.
        /// If before is omitted, null, or a number out of range, then element will be added at the end of the list.
        /// This method will throw a "HierarchyRequestError" DOMException if element is an ancestor of the element into which it is to be inserted.
        /// </summary>
        /// <param name="element">Element to add</param>
        /// <param name="before">location to insert element at</param>
        [CEReactions]
        public void add(HTMLOptGroupElement element, HTMLElement before = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#dom-htmloptionscollection-add */
            CEReactions.Wrap_CEReaction(root.nodeDocument.defaultView, () =>
            {
                if (DOMCommon.Is_Ancestor(element, root))
                {
                    throw new HierarchyRequestError("Cannot add ancestor of Select element to its descendants");
                }

                if (!DOMCommon.Is_Descendant(before, root))
                {
                    throw new HierarchyRequestError("the element specified for 'before' must be a descendant of the Select element");
                }

                if (ReferenceEquals(element, before))
                {
                    return;
                }

                Node reference = before;
                Node parent = null;
                if (reference != null)
                {
                    parent = reference.parentNode;
                }
                else
                {
                    parent = root;
                }

                Node.Dom_pre_insert_node(element, parent, reference);
            });
        }


        /// <summary>
        /// Removes the item with index index from the collection.
        /// </summary>
        /// <param name="index"></param>
        [CEReactions]
        public void remove(long index)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#dom-htmloptionscollection-remove */
            var items = Collection;
            if (items.Count == 0)
            {
                return;
            }

            if (index < 0 || index >= items.Count)
            {
                return;
            }

            var element = items[(int)index];
            root.removeChild(element);
        }

        /// <summary>
        /// Returns the index of the first selected item, if any, or −1 if there is no selected item.
        /// Can be set, to change the selection.
        /// </summary>
        public int selectedIndex
        {/* Docs: https://html.spec.whatwg.org/multipage/common-dom-interfaces.html#dom-htmloptionscollection-selectedindex */
            get => ((HTMLSelectElement)root).selectedIndex;
            set => ((HTMLSelectElement)root).selectedIndex = value;
        }

    }
}
