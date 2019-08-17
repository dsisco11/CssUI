using CssUI.DOM;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Internal;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CssUI.HTML
{
    /* Docs: https://html.spec.whatwg.org/multipage/images.html */

    /// <summary>
    /// An img element represents an image.
    /// </summary>
    [MetaElement("img")]
    public class HTMLImageElement : FormAssociatedElement
    {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content.html#htmlimageelement */

        #region Definition
        public override EContentCategories Categories
        {
            get
            {
                var flags = EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Embedded | EContentCategories.Palpable;
                if (hasAttribute(EAttributeName.UseMap))
                {
                    flags |= EContentCategories.Interactive;
                }

                return flags;
            }
        }
        #endregion

        #region Properties
        protected double? currentPixelDensity = null;
        protected DataRequest currentRequest;
        protected DataRequest pendingRequest;
        internal EmbeddedImage Content = null;
        #endregion

        #region Constructors
        public HTMLImageElement(Document document) : this(document, "img")
        {
        }
        public HTMLImageElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Returns the image's absolute URL.
        /// </summary>
        public string currentSrc => currentRequest.currentURL;

        /// <summary>
        /// Return the intrinsic width of the image, or zero if not known.
        /// </summary>
        public uint naturalWidth
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content.html#dom-img-naturalwidth */
            get
            {
                if (Content == null) return 0;

                if (!currentPixelDensity.HasValue)
                {
                    return (uint?)Content?.Intrinsic_Width ?? 0;
                }

                return (uint)(Content.Intrinsic_Width * currentPixelDensity.Value);
            }
        }
        /// <summary>
        /// Return the intrinsic height of the image, or zero if not known.
        /// </summary>
        public uint naturalHeight
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content.html#dom-img-naturalheight */
            get
            {
                if (Content == null) return 0;

                if (!currentPixelDensity.HasValue)
                {
                    return (uint?)Content?.Intrinsic_Height ?? 0;
                }

                return (uint)(Content.Intrinsic_Height * currentPixelDensity.Value);
            }
        }

        /// <summary>
        /// Returns true if the image has been completely downloaded or if no image is specified; otherwise, returns false.
        /// </summary>
        public bool complete
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content.html#dom-img-complete */
            get
            {
                /* The IDL attribute complete must return true if any of the following conditions is true: */
                bool srcsetOmitted = !hasAttribute(EAttributeName.SrcSet);
                if (!hasAttribute(EAttributeName.Src) && srcsetOmitted)
                    return true;

                if (srcsetOmitted && string.IsNullOrEmpty(src))
                    return true;

                /* The final task that is queued by the networking task source once the resource has been fetched has been queued. */
                if (currentRequest.State == EDataRequestState.Broken || currentRequest.State == EDataRequestState.CompletelyAvailable)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// X-coordinate offset of this element relative to its root element
        /// </summary>
        public long x
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#extensions-to-the-htmlimageelement-interface */
            get
            {
                return (long)(Box.Border.Left - ownerDocument.Initial_Containing_Block.left);
            }
        }
        /// <summary>
        /// Y-coordinate offset of this element relative to its root element
        /// </summary>
        public long y
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#extensions-to-the-htmlimageelement-interface */
            get
            {
                return (long)(Box.Border.Top - ownerDocument.Initial_Containing_Block.top);
            }
        }
        #endregion

        #region Content Attributes

        [CEReactions]
        public string alt
        {
            get => getAttribute(EAttributeName.Alt)?.Get_String();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Alt, AttributeValue.From_String(value)));
        }
        [CEReactions]
        public string src
        {
            get => getAttribute(EAttributeName.Src)?.Get_String();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Src, AttributeValue.From_String(value)));
        }
        [CEReactions]
        public string srcset
        {
            get => getAttribute(EAttributeName.SrcSet)?.Get_String();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.SrcSet, AttributeValue.From_String(value)));
        }
        [CEReactions]
        public string sizes
        {
            get => getAttribute(EAttributeName.Sizes)?.Get_String();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Sizes, AttributeValue.From_String(value)));
        }
        [CEReactions]
        public string crossOrigin
        {
            get => getAttribute(EAttributeName.CrossOrigin)?.Get_String();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.CrossOrigin, AttributeValue.From_String(value)));
        }

        /// <summary>
        /// The usemap attribute, if present, can indicate that the image has an associated image map.
        /// </summary>
        [CEReactions]
        public string useMap
        {
            get => getAttribute(EAttributeName.UseMap)?.Get_String();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.UseMap, AttributeValue.From_String(value)));
        }

        /// <summary>
        /// The ismap attribute, when used on an element that is a descendant of an a element with an href attribute, indicates by its presence that the element provides access to a server-side image map. 
        /// This affects how events are handled on the corresponding a element.
        /// </summary>
        [CEReactions]
        public bool isMap
        {
            get => hasAttribute(EAttributeName.IsMap);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => toggleAttribute(EAttributeName.IsMap, value));
        }

        /* =====================================================================
         * XXX: WIDTH & HEIGHT FOR IMAGES IS THE IMAGE BOUNDS, SEE DOCUMENTATION
         * =====================================================================
         */
        [CEReactions]
        public uint width
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content.html#dom-img-width */
            get => getAttribute(EAttributeName.Width)?.Get_UInt() ?? naturalWidth;
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Width, AttributeValue.From_Integer(value)));
        }
        [CEReactions]
        public uint height
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content.html#dom-img-height */
            get => getAttribute(EAttributeName.Height)?.Get_UInt() ?? naturalHeight;
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Height, AttributeValue.From_Integer(value)));
        }

        [CEReactions]
        public string referrerPolicy
        {
            get => getAttribute(EAttributeName.ReferrerPolicy).Get_String();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.ReferrerPolicy, AttributeValue.From_String(value)));
        }
        [CEReactions]
        public string decoding
        {
            get => getAttribute(EAttributeName.Decoding).Get_String();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Decoding, AttributeValue.From_String(value)));
        }

        #endregion

        /// <summary>
        /// This function returns an appropriate <see cref="DataRequest"/> object that can properly retrieve the data format of the requested object.
        /// </summary>
        /// <returns></returns>
        protected virtual DataRequest get_data_request(string location)
        {
            return new DataRequest(location);
        }


        /// <summary>
        /// Decodes the provided data stream into a useable raw RGBA image object which can be offloaded to the graphics API
        /// </summary>
        /// <param name="Stream"></param>
        protected virtual void run_decoder(DataRequest dataRequest)
        {/* Docs: https://html.spec.whatwg.org/multipage/images.html#img-decoding-process */


        }

        /// <summary>
        /// This method causes the user agent to decode the image in parallel, returning a promise that fulfills when decoding is complete.
        /// The promise will be rejected with an "EncodingError" DOMException if the image cannot be decoded.
        /// </summary>
        /// <returns></returns>
        public TaskCompletionSource<object> decode()
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content.html#dom-img-decode */
            var tcs = new TaskCompletionSource<object>();

            Task.Factory.StartNew(() =>
            {
                /* If any of the following conditions are true about this img element:
                 *      its node document is not an active document;
                 *      its current request's state is broken,
                 *      then reject promise with an "EncodingError" DOMException.
                 */
                if (!nodeDocument.Is_FullyActive)
                {
                    tcs.TrySetException(new EncodingError());
                    return;
                }
                /* XXX: IF the current requests state is broken then throw an EncodingError on tcs */
                else if (currentRequest.State == EDataRequestState.Broken)
                {
                    tcs.TrySetException(new EncodingError());
                }
                else
                {
                    /* Otherwise, in parallel, wait for one of the following cases to occur, and perform the corresponding actions: */
                    /* This img element's node document stops being an active document
                     * This img element's current request changes or is mutated
                     * This img element's current request's state becomes broken
                     *      Reject promise with an "EncodingError" DOMException. 
                     */
                    while (true)
                    {
                        WaitHandle.WaitAny(new WaitHandle[] { currentRequest.State_Change_Signal, nodeDocument.Active_State_Change_Signal });
                        if (!nodeDocument.Is_FullyActive || currentRequest.State == EDataRequestState.Broken)
                        {
                            tcs.TrySetException(new EncodingError());
                            return;
                        }
                        else if (currentRequest.State == EDataRequestState.CompletelyAvailable)
                        {
                            run_decoder(currentRequest);
                            /* If the decoding process completes successfully, resolve promise with undefined. */
                            tcs.TrySetResult(null);
                            return;
                        }
                    }
                }

            }).ConfigureAwait(continueOnCapturedContext: false);

            return tcs;
        }
    }
}
