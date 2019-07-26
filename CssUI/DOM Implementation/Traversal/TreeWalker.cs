using CssUI.DOM.Nodes;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;

namespace CssUI.DOM
{
    public class TreeWalker
    {/* Docs: https://dom.spec.whatwg.org/#treewalker */
        #region Properties
        public readonly Node root = null;
        public readonly ENodeFilterMask whatToShow = 0x0;
        public readonly NodeFilter Filter = null;
        private Node currentNode;
        private bool isActive = false;
        #endregion

        #region Constructors
        public TreeWalker(Node root, ENodeFilterMask whatToShow)
        {
            this.root = root;
            this.whatToShow = whatToShow;
            this.currentNode = root;
            this.Filter = null;
        }
        public TreeWalker(Node root, ENodeFilterMask whatToShow, NodeFilter Filter)
        {
            this.root = root;
            this.whatToShow = whatToShow;
            this.currentNode = root;
            this.Filter = Filter;
        }
        #endregion


        private ENodeFilterResult filterNode(Node node)
        {
            /* To filter a node node within a NodeIterator or TreeWalker object traverser, run these steps: */
            /* 1) If traverser’s active flag is set, then throw an "InvalidStateError" DOMException. */
            if (isActive) throw new InvalidStateError();
            /* 2) Let n be node’s nodeType attribute value − 1. */
            int n = (int)node.nodeType;
            /* 3) If the nth bit (where 0 is the least significant bit) of traverser’s whatToShow is not set, then return FILTER_SKIP. */
            ulong mask = (1UL << n);
            if (0 == ((ulong)whatToShow & mask))
                return ENodeFilterResult.FILTER_SKIP;
            /* If traverser’s filter is null, then return FILTER_ACCEPT. */
            if (Filter == null)
                return ENodeFilterResult.FILTER_ACCEPT;

            /* Set traverser’s active flag. */
            isActive = true;
            /* Let result be the return value of call a user object’s operation with traverser’s filter, "acceptNode", and « node ». If this throws an exception, then unset traverser’s active flag and rethrow the exception. */
            ENodeFilterResult result;
            try
            {
                result = Filter.acceptNode(node);
            }
            catch (DOMException)
            {
                /* Unset traverser’s active flag. */
                isActive = false;
                throw;
            }
            /* Unset traverser’s active flag. */
            isActive = false;
            return result;
        }

        /// <summary>
        /// Returns the next node in a sequence containing all nodes in the parent chain(in reverse tree order)
        /// </summary>
        /// <returns></returns>
        public Node parentNode()
        {
            /* The parentNode() method, when invoked, must run these steps: */
            /* 1) Let node be the context object’s current. */
            Node node = this.currentNode;
            /* 2) While node is non-null and is not the context object’s root: */
            while (node != null && !ReferenceEquals(node, this.root))
            {
                /* 1) Set node to node’s parent. */
                node = node.parentNode;
                /* 2) If node is non-null and filtering node within the context object returns FILTER_ACCEPT, then set the context object’s current to node and return node. */
                if (!ReferenceEquals(node, null) && filterNode(node) == ENodeFilterResult.FILTER_ACCEPT)
                {
                    this.currentNode = node;
                    return node;
                }
            }
            /* 3) Return null. */
            return null;
        }

        /// <summary>
        /// Returns the next node in a sequence containing all first-child descendant nodes and the siblings of the bottom-most node(in tree order)
        /// </summary>
        public Node firstChild()
        {
            /* To traverse children, given a walker and type, run these steps: */
            /* 1) Let node be walker’s current. */
            Node node = this.currentNode;
            /* 2) Set node to node’s first child if type is first, and node’s last child if type is last. */
            node = node.firstChild;
            /* 3) While node is non-null: */
            while(node != null)
            {
                /* 1) Let result be the result of filtering node within walker. */
                var result = filterNode(node);
                /* 2) If result is FILTER_ACCEPT, then set walker’s current to node and return node. */
                if (result == ENodeFilterResult.FILTER_ACCEPT)
                {
                    this.currentNode = node;
                    return node;
                }
                /* 3) If result is FILTER_SKIP, then: */
                if(result == ENodeFilterResult.FILTER_SKIP)
                {
                    /* 1) Let child be node’s first child if type is first, and node’s last child if type is last. */
                    var child = node.firstChild;
                    /* 2) If child is non-null, then set node to child and continue. */
                    if (child != null)
                    {
                        node = child;
                        continue;
                    }
                }
                /* 4) While node is non-null: */
                while(!ReferenceEquals(node, null))
                {
                    /* 1) Let sibling be node’s next sibling if type is first, and node’s previous sibling if type is last. */
                    var sibling = node.nextSibling;
                    /* 2) If sibling is non-null, then set node to sibling and break. */
                    if (sibling != null) break;
                    /* 3) Let parent be node’s parent. */
                    var parent = node.parentNode;
                    /* 4) If parent is null, walker’s root, or walker’s current, then return null. */
                    if (parent == null || ReferenceEquals(parent, this.root) || ReferenceEquals(parent, this.currentNode))
                        return null;
                    /* 5) Set node to parent. */
                    node = parent;
                }
            }
            /* 4) Return null. */
            return null;
        }

        /// <summary>
        /// Returns the next node in a sequence containing all descendant nodes (in reverse tree order)
        /// </summary>
        public Node lastChild()
        {
            /* To traverse children, given a walker and type, run these steps: */
            /* 1) Let node be walker’s current. */
            Node node = this.currentNode;
            /* 2) Set node to node’s first child if type is first, and node’s last child if type is last. */
            node = node.lastChild;
            /* 3) While node is non-null: */
            while (node != null)
            {
                /* 1) Let result be the result of filtering node within walker. */
                var result = filterNode(node);
                /* 2) If result is FILTER_ACCEPT, then set walker’s current to node and return node. */
                if (result == ENodeFilterResult.FILTER_ACCEPT)
                {
                    this.currentNode = node;
                    return node;
                }
                /* 3) If result is FILTER_SKIP, then: */
                if (result == ENodeFilterResult.FILTER_SKIP)
                {
                    /* 1) Let child be node’s first child if type is first, and node’s last child if type is last. */
                    var child = node.lastChild;
                    /* 2) If child is non-null, then set node to child and continue. */
                    if (child != null)
                    {
                        node = child;
                        continue;
                    }
                }
                /* 4) While node is non-null: */
                while (node != null)
                {
                    /* 1) Let sibling be node’s next sibling if type is first, and node’s previous sibling if type is last. */
                    var sibling = node.previousSibling;
                    /* 2) If sibling is non-null, then set node to sibling and break. */
                    if (sibling != null) break;
                    /* 3) Let parent be node’s parent. */
                    var parent = node.parentNode;
                    /* 4) If parent is null, walker’s root, or walker’s current, then return null. */
                    if (parent == null || ReferenceEquals(parent, this.root) || ReferenceEquals(parent, this.currentNode))
                        return null;
                    /* 5) Set node to parent. */
                    node = parent;
                }
            }
            /* 4) Return null. */
            return null;
        }

        /// <summary>
        /// Returns the next node in a sequence containing all sibling nodes of the root and their children (in tree order)
        /// </summary>
        /// <returns></returns>
        public Node nextSibling()
        {
            /* To traverse siblings, given a walker and type, run these steps: */
            /* 1) Let node be walker’s current. */
            Node node = this.currentNode;
            /* 2) If node is root, then return null. */
            if (ReferenceEquals(node, this.root)) return null;
            /* 3) While true: */
            while (true)
            {
                /* 1) Let sibling be node’s next sibling if type is next, and node’s previous sibling if type is previous. */
                var sibling = node.nextSibling;
                /* 2) While sibling is non-null: */
                while (sibling != null)
                {
                    /* 1) Set node to sibling. */
                    node = sibling;
                    /* 2) Let result be the result of filtering node within walker. */
                    var result = filterNode(node);
                    /* 3) If result is FILTER_ACCEPT, then set walker’s current to node and return node. */
                    if (result == ENodeFilterResult.FILTER_ACCEPT)
                    {
                        this.currentNode = node;
                        return node;
                    }
                    /* 4) Set sibling to node’s first child if type is next, and node’s last child if type is previous. */
                    sibling = node.firstChild;
                    /* 5) If result is FILTER_REJECT or sibling is null, then set sibling to node’s next sibling if type is next, and node’s previous sibling if type is previous. */
                    if (sibling == null || result == ENodeFilterResult.FILTER_REJECT)
                        sibling = node.nextSibling;
                }
                /* 3) Set node to node’s parent. */
                node = node.parentNode;
                /* 4) If node is null or walker’s root, then return null. */
                if (node == null || ReferenceEquals(node, this.root))
                    return null;
                /* 5) If the return value of filtering node within walker is FILTER_ACCEPT, then return null. */
                if (filterNode(node) == ENodeFilterResult.FILTER_ACCEPT)
                    return null;
            }
        }

        /// <summary>
        /// Returns the next node in a sequence containing all sibling nodes of the root and their children (in reverse tree order)
        /// </summary>
        /// <returns></returns>
        public Node previousSibling()
        {
            /* To traverse siblings, given a walker and type, run these steps: */
            /* 1) Let node be walker’s current. */
            Node node = this.currentNode;
            /* 2) If node is root, then return null. */
            if (ReferenceEquals(node, this.root)) return null;
            /* 3) While true: */
            while(true)
            {
                /* 1) Let sibling be node’s next sibling if type is next, and node’s previous sibling if type is previous. */
                var sibling = node.previousSibling;
                /* 2) While sibling is non-null: */
                while(sibling != null)
                {
                    /* 1) Set node to sibling. */
                    node = sibling;
                    /* 2) Let result be the result of filtering node within walker. */
                    var result = filterNode(node);
                    /* 3) If result is FILTER_ACCEPT, then set walker’s current to node and return node. */
                    if (result == ENodeFilterResult.FILTER_ACCEPT)
                    {
                        this.currentNode = node;
                        return node;
                    }
                    /* 4) Set sibling to node’s first child if type is next, and node’s last child if type is previous. */
                    sibling = node.lastChild;
                    /* 5) If result is FILTER_REJECT or sibling is null, then set sibling to node’s next sibling if type is next, and node’s previous sibling if type is previous. */
                    if (sibling == null || result == ENodeFilterResult.FILTER_REJECT)
                        sibling = node.previousSibling;
                }
                /* 3) Set node to node’s parent. */
                node = node.parentNode;
                /* 4) If node is null or walker’s root, then return null. */
                if (node == null || ReferenceEquals(node, this.root))
                    return null;
                /* 5) If the return value of filtering node within walker is FILTER_ACCEPT, then return null. */
                if (filterNode(node) == ENodeFilterResult.FILTER_ACCEPT)
                    return null;
            }
        }

        /// <summary>
        /// Returns the next node in a sequence containing all ancestor nodes of the root (in reverse tree order)
        /// </summary>
        public Node previousNode()
        {
            /* The previousNode() method, when invoked, must run these steps: */
            /* 1) Let node be the context object’s current. */
            Node node = this.currentNode;
            /* 2) While node is not the context object’s root: */
            while (!ReferenceEquals(node, this.root))
            {
                /* 1) Let sibling be node’s previous sibling. */
                var sibling = node.previousSibling;
                /* 2) While sibling is non-null: */
                while(sibling != null)
                {
                    /* 1) Set node to sibling. */
                    node = sibling;
                    /* 2) Let result be the result of filtering node within the context object. */
                    var result = filterNode(node);
                    /* 3) While result is not FILTER_REJECT and node has a child: */
                    while( result != ENodeFilterResult.FILTER_REJECT && node.hasChildNodes())
                    {
                        /* 1) Set node to node’s last child. */
                        node = node.lastChild;
                        /* 2) Set result to the result of filtering node within the context object. */
                        result = filterNode(node);
                    }
                    /* 4) If result is FILTER_ACCEPT, then set the context object’s current to node and return node. */
                    if (result == ENodeFilterResult.FILTER_ACCEPT)
                    {
                        this.currentNode = node;
                        return node;
                    }
                    /* 5) Set sibling to node’s previous sibling. */
                    sibling = node.previousSibling;
                }
                /* 3) If node is the context object’s root or node’s parent is null, then return null. */
                if (ReferenceEquals(node, this.root) || ReferenceEquals(node.parentNode, null))
                    return null;
                /* 4) Set node to node’s parent. */
                node = node.parentNode;
                /* 5) If the return value of filtering node within the context object is FILTER_ACCEPT, then set the context object’s current to node and return node. */
                if (filterNode(node) == ENodeFilterResult.FILTER_ACCEPT)
                {
                    this.currentNode = node;
                    return node;
                }
            }
            /* 3) Return null. */
            return null;
        }

        /// <summary>
        /// Returns the next node in a sequence containing the complete tree of all descendant nodes from the root (in tree order)
        /// <para>Basically itterates through the first child of every descendent node until it hits one it doesnt accept, then starts returning the rest of the nodes within the roots tree in order from left-right and from bottom-top</para>
        /// </summary>
        public Node nextNode()
        {
            /* The nextNode() method, when invoked, must run these steps: */
            /* 1) Let node be the context object’s current. */
            Node node = this.currentNode;
            /* 2) Let result be FILTER_ACCEPT. */
            var result = ENodeFilterResult.FILTER_ACCEPT;
            /* 3) While true: */
            while (true)
            {
                /* 1) While result is not FILTER_REJECT and node has a child: */
                while (result != ENodeFilterResult.FILTER_REJECT && node.hasChildNodes())
                {
                    /* 1) Set node to its first child. */
                    node = node.firstChild;
                    /* 2) Set result to the result of filtering node within the context object. */
                    result = filterNode(node);
                    /* 3) If result is FILTER_ACCEPT, then set the context object’s current to node and return node. */
                    if (result == ENodeFilterResult.FILTER_ACCEPT)
                    {
                        this.currentNode = node;
                        return node;
                    }
                }
                /* 2) Let sibling be null. */
                Node sibling = null;
                /* 3) Let temporary be node. */
                var temporary = node;
                /* 4) While temporary is non-null: */
                while (temporary != null)
                {
                    /* 1) If temporary is the context object’s root, then return null. */
                    if (ReferenceEquals(temporary, this.root))
                        return null;
                    /* 2) Set sibling to temporary’s next sibling. */
                    sibling = temporary.nextSibling;
                    /* 3) If sibling is non-null, then break. */
                    if (sibling != null) break;
                    /* 4) Set temporary to temporary’s parent. */
                    temporary = temporary.parentNode;
                }
                /* 5) Set result to the result of filtering node within the context object. */
                result = filterNode(node);
                /* 6) If result is FILTER_ACCEPT, then set the context object’s current to node and return node. */
                if (result == ENodeFilterResult.FILTER_ACCEPT)
                {
                    this.currentNode = node;
                    return node;
                }
            }
        }
    }
}

