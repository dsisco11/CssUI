using System;
using System.Collections.Generic;
using CssUI.DOM;
using CssUI.HTTP;

namespace CssUI.HTML
{
    public abstract class HyperlinkElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/links.html#htmlhyperlinkelementutils */

        #region Backing Values
        #endregion

        #region Properties
        public Url url { get; private set; } = null;
        #endregion

        #region Constructors
        public HyperlinkElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Accessors
        #endregion

        #region Content Attributes
        /// <summary>
        /// Returns the hyperlink's URL.
        /// Can be set, to change the URL.
        /// </summary>
        [CEReactions] public string href
        {
            get
            {
                reinitialize_url();
                if (!hasAttribute(EAttributeName.Href, out Attr outAttr) && url == null) return string.Empty;
                if (url == null) return outAttr.Value?.AsString();
                return url.Serialize();
            }
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Href, AttributeValue.From(value)));
        }

        /// <summary>
        /// Returns the hyperlink's URL's origin.
        /// </summary>
        public string origin
        {/* Docs: https://html.spec.whatwg.org/multipage/links.html#dom-hyperlink-origin */
            get
            {
                reinitialize_url();
                if (url == null) return string.Empty;
                return url?.Origin?.Serialize() ?? string.Empty;
            }
        }

        /// <summary>
        /// Returns the hyperlink's URL's scheme.
        /// Can be set, to change the URL's scheme.
        /// </summary>
        [CEReactions] public string protocol
        {
            get
            {
                reinitialize_url();
                if (url == null) return ":";
                return string.Concat(url.Scheme.NameLower, ":");
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
                {
                    reinitialize_url();
                    if (url == null) return;
                    string protocolString = string.Concat(value, UnicodeCommon.CHAR_COLON);
                    if (Url.Parse_Basic(protocolString.AsMemory(), null, out Url parsedUrl, null, url, ESchemeState.SchemeStart))
                    {
                        url = parsedUrl;
                    }

                    update_href();
                });
            }
        }


        /// <summary>
        /// Returns the hyperlink's URL's username.
        /// Can be set, to change the URL's username.
        /// </summary>
        [CEReactions] public string username
        {
            get
            {
                reinitialize_url();
                return url?.Username ?? string.Empty;
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
                {
                    reinitialize_url();
                    if (url == null || url.CannotHaveCredentials) return;
                    url.Username = value;
                    update_href();
                });
            }
        }

        /// <summary>
        /// Returns the hyperlink's URL's password.
        /// Can be set, to change the URL's password.
        /// </summary>
        [CEReactions] public string password
        {
            get
            {
                reinitialize_url();
                return url?.Password ?? string.Empty;
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
                {
                    reinitialize_url();
                    if (url == null || url.CannotHaveCredentials) return;
                    url.Password = value;
                    update_href();
                });
            }
        }

        /// <summary>
        /// Returns the hyperlink's URL's host and port (if different from the default port for the scheme).
        /// Can be set, to change the URL's host and port.
        /// </summary>
        [CEReactions] public string host
        {
            get
            {
                reinitialize_url();
                if (url?.Host == null) return string.Empty;
                if (url.Port == null) return url.Host.Serialize();
                return string.Concat(url.Host.Serialize(), UnicodeCommon.CHAR_COLON, url.Port.Value);
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
                {
                    reinitialize_url();
                    if (url == null || url.bCannotBeBaseURLFlag) return;
                    if (Url.Parse_Basic(value.AsMemory(), null, out Url parsedUrl, null, url, ESchemeState.Hostname))
                    {
                        url = parsedUrl;
                        update_href();
                    }
                });
            }
        }

        /// <summary>
        /// Returns the hyperlink's URL's host.
        /// Can be set, to change the URL's host.
        /// </summary>
        [CEReactions] public string hostname
        {
            get
            {
                reinitialize_url();
                return url?.Host?.Serialize() ?? string.Empty;
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
                {
                    reinitialize_url();
                    if (url == null || url.bCannotBeBaseURLFlag) return;
                    if (Url.Parse_Basic(value.AsMemory(), null, out Url parsedUrl, null, url, ESchemeState.Hostname))
                    {
                        url = parsedUrl;
                        update_href();
                    }
                });
            }
        }

        /// <summary>
        /// Returns the hyperlink's URL's port.
        /// Can be set, to change the URL's port.
        /// </summary>
        [CEReactions] public string port
        {
            get
            {
                reinitialize_url();
                if (url == null || !url.Port.HasValue) return string.Empty;
                return url.Port.Value.ToString();
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
                {
                    reinitialize_url();
                    if (url == null || url.CannotHaveCredentials) return;
                    if (value.Length <= 0)
                    {
                        url.Port = null;
                    }
                    else
                    {
                        if (Url.Parse_Basic(value.AsMemory(), null, out Url parsedUrl, null, url, ESchemeState.Port))
                        {
                            url = parsedUrl;
                            update_href();
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Returns the hyperlink's URL's path.
        /// Can be set, to change the URL's path.
        /// </summary>
        [CEReactions] public string pathname
        {
            get
            {
                reinitialize_url();
                if (url == null) return string.Empty;
                if (url.bCannotBeBaseURLFlag) return url.Path[0];
                if (url.Path.Count <= 0) return string.Empty;

                return string.Concat(UnicodeCommon.CHAR_SOLIDUS, String.Join(UnicodeCommon.CHAR_SOLIDUS.ToString(), url.Path));
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
                {
                    reinitialize_url();
                    if (url == null || url.bCannotBeBaseURLFlag) return;
                    url.Path = new List<string>();
                    if (Url.Parse_Basic(value.AsMemory(), null, out Url parsedUrl, null, url, ESchemeState.PathStart))
                    {
                        url = parsedUrl;
                    }
                    update_href();
                });
            }
        }

        /// <summary>
        /// Returns the hyperlink's URL's query (includes leading "?" if non-empty).
        /// Can be set, to change the URL's query (ignores leading "?").
        /// </summary>
        [CEReactions] public string search
        {
            get
            {
                reinitialize_url();
                if (url == null || url.Query == null || url.Query.Length <= 0) return string.Empty;
                return string.Concat(UnicodeCommon.CHAR_QUESTION_MARK, url.Query);
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
                {
                    reinitialize_url();
                    if (url == null) return;
                    if (value == null || value.Length <= 0)
                    {
                        url.Query = null;
                    }
                    else
                    {
                        var input = (value[0] == UnicodeCommon.CHAR_QUESTION_MARK) ? value.Substring(1) : value;
                        url.Query = string.Empty;
                        if (Url.Parse_Basic(input.AsMemory(), null, out Url parsedUrl, nodeDocument.characterEncoding, url, ESchemeState.Query))
                        {
                            url = parsedUrl;
                        }
                    }
                    update_href();
                });
            }
        }

        /// <summary>
        /// Returns the hyperlink's URL's fragment (includes leading "#" if non-empty).
        /// Can be set, to change the URL's fragment (ignores leading "#").
        /// </summary>
        [CEReactions] public string hash
        {
            get
            {
                reinitialize_url();
                if (url == null || string.IsNullOrEmpty(url.Fragment)) return string.Empty;
                return string.Concat(UnicodeCommon.CHAR_HASH, url.Fragment);
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
                {
                    reinitialize_url();
                    if (url == null) return;
                    if (value.Length <= 0)
                    {
                        url.Fragment = null;
                    }
                    else
                    {
                        var input = (value[0] == UnicodeCommon.CHAR_HASH) ? value.Substring(1) : value;
                        url.Fragment = null;
                        if (Url.Parse_Basic(value.AsMemory(), null, out Url parsedUrl, null, url, ESchemeState.Fragment))
                        {
                            url = parsedUrl;
                        }
                    }
                    update_href();
                });
            }
        }
        #endregion

        #region Internal Overrides
        internal override void run_attribute_change_steps(Element element, AtomicName<EAttributeName> localName, AttributeValue oldValue, AttributeValue newValue, ReadOnlyMemory<char> Namespace)
        {
            base.run_attribute_change_steps(element, localName, oldValue, newValue, Namespace);

            if (localName == EAttributeName.Href)
            {
                set_url();
            }
        }
        #endregion

        private void update_href()
        {/* Docs: https://html.spec.whatwg.org/multipage/links.html#update-href */
            string urlString = url.ToString();
            setAttribute(EAttributeName.Href, AttributeValue.From(urlString));
        }

        private void set_url()
        {/* Docs: https://html.spec.whatwg.org/multipage/links.html#concept-hyperlink-url-set */
            if (!hasAttribute(EAttributeName.Href, out Attr outAttr))
            {
                url = null;
                return;
            }

            string href = outAttr.Value?.AsString();
            if (HTTPCommon.Parse_URL(href.AsMemory(), nodeDocument, out Url outUrl, out _))
            {
                url = outUrl;
            }
            else
            {
                url = null;
            }
        }

        private void reinitialize_url()
        {/* Docs: https://html.spec.whatwg.org/multipage/links.html#reinitialise-url */
            if (url != null && url.Scheme == "blob" && url.bCannotBeBaseURLFlag)
                return;

            set_url();
        }



    }
}
