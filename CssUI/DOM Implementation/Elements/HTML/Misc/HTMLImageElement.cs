using CssUI.DOM.Exceptions;
using CssUI.DOM.Internal;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CssUI.DOM
{
    /* Docs: https://html.spec.whatwg.org/multipage/images.html */

    public class HTMLImageElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content.html#htmlimageelement */
        #region Properties
        [CEReactions] public string alt
        {
            get => getAttribute(EAttributeName.Alt);
            set => ReactionsCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Alt, value));
        }
        [CEReactions] public string src
        {
            get => getAttribute(EAttributeName.Src);
            set => ReactionsCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Src, value));
        }
        [CEReactions] public string srcset
        {
            get => getAttribute(EAttributeName.SrcSet);
            set => ReactionsCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.SrcSet, value));
        }
        [CEReactions] public string sizes
        {
            get => getAttribute(EAttributeName.Sizes);
            set => ReactionsCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Sizes, value));
        }
        [CEReactions] public string crossOrigin
        {
            get => getAttribute(EAttributeName.CrossOrigin);
            set => ReactionsCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.CrossOrigin, value));
        }

        /// <summary>
        /// The usemap attribute, if present, can indicate that the image has an associated image map.
        /// </summary>
        [CEReactions] public string useMap
        {
            get => getAttribute(EAttributeName.UseMap);
            set => ReactionsCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.UseMap, value));
        }

        /// <summary>
        /// The ismap attribute, when used on an element that is a descendant of an a element with an href attribute, indicates by its presence that the element provides access to a server-side image map. 
        /// This affects how events are handled on the corresponding a element.
        /// </summary>
        [CEReactions] public bool isMap
        {
            get => hasAttribute(EAttributeName.IsMap);
            set => ReactionsCommon.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.IsMap, value));
        }

        [CEReactions] public uint width
        {
            get => (uint)MathExt.Max(0, getAttribute_Numeric(EAttributeName.Width));
            set => ReactionsCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Width, value.ToString()));
        }
        [CEReactions] public uint height
        {
            get => (uint)MathExt.Max(0, getAttribute_Numeric(EAttributeName.Height));
            set => ReactionsCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Height, value.ToString()));
        }

        [CEReactions] public string referrerPolicy
        {
            get => getAttribute(EAttributeName.ReferrerPolicy);
            set => ReactionsCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.ReferrerPolicy, value));
        }
        [CEReactions] public string decoding
        {
            get => getAttribute(EAttributeName.Decoding);
            set => ReactionsCommon.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Decoding, value));
        }

        protected DataRequest currentRequest;
        protected DataRequest pendingRequest;
        #endregion

        #region Accessors
        /// <summary>
        /// Returns the image's absolute URL.
        /// </summary>
        public string currentSrc => currentRequest.currentURL;
        /// <summary>
        /// Return the intrinsic width of the image, or zero if not known.
        /// </summary>
        public ulong naturalWidth { get; }
        /// <summary>
        /// Return the intrinsic height of the image, or zero if not known.
        /// </summary>
        public ulong naturalHeight { get; }
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

                if (srcsetOmitted && string.IsNullOrEmpty(this.src))
                    return true;

                /* The final task that is queued by the networking task source once the resource has been fetched has been queued. */
                if (currentRequest.State == Enums.EDataRequestState.Broken || currentRequest.State == Enums.EDataRequestState.CompletelyAvailable)
                    return true;

                return false;
            }
        }
        #endregion

        #region Accessors
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

        #region Constructors
        public HTMLImageElement(Document document, string localName, string prefix, string Namespace) : base(document, localName)
        {
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
                if (!nodeDocument.is_fully_active)
                {
                    tcs.TrySetException(new EncodingError());
                    return;
                }
                /* XXX: IF the current requests state is broken then throw an EncodingError on tcs */
                else if (currentRequest.State == Enums.EDataRequestState.Broken)
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
                        if (!nodeDocument.is_fully_active || currentRequest.State == Enums.EDataRequestState.Broken)
                        {
                            tcs.TrySetException(new EncodingError());
                            return;
                        }
                        else if (currentRequest.State == Enums.EDataRequestState.CompletelyAvailable)
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
