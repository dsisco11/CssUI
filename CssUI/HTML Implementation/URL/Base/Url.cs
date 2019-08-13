using xLog;
using System;
using System.Collections.Generic;
using System.Text;
using static CssUI.UnicodeCommon;
using System.Net;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.HTML
{
    public class Url
    {
        #region Static
        static ILogger Log = LogFactory.GetLogger(typeof(Url));
        #endregion

        #region Backing Values
        private string _username = string.Empty;
        private string _password = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public AtomicName<EUrlScheme> Scheme = string.Empty;

        public UrlHost Host = null;
        public ushort? Port = null;

        public List<string> Path = new List<string>();
        public string Query = null;
        public string Fragment = null;
        public bool bCannotBeBaseURLFlag = false;
        public BlobURLEntry blobURLEntry = null;
        #endregion

        #region Constructors
        #endregion

        #region Accessors
        public string Username
        {
            get => _username;
            set
            {
                _username = string.Empty;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < value.Length; i++)
                {
                    var encoded = UTF8_Percent_Encode(value[i], Percent_Encode_Set_Userinfo);
                    sb.Append(encoded);
                }

                _username = sb.ToString();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = string.Empty;
                StringBuilder sb = new StringBuilder();
                for (int i=0; i<value.Length; i++)
                {
                    var encoded = UTF8_Percent_Encode(value[i], Percent_Encode_Set_Userinfo);
                    sb.Append(encoded);
                }

                _password = sb.ToString();
            }
        }

        public UrlOrigin Origin
        {/* Docs: https://url.spec.whatwg.org/#concept-url-origin */
            get
            {
                if (Scheme == "blob")
                {
                    if (blobURLEntry != null)
                    {
                        return UrlOrigin.Default;
                    }

                    if (Parse_Basic(Path[0].AsMemory(), null, out Url parsedUrl))
                    {
                        return parsedUrl.Origin;
                    }

                    return UrlOrigin.Opaque;
                }

                if (Scheme.EnumValue.HasValue)
                {
                    switch (Scheme.EnumValue.Value)
                    {
                        case EUrlScheme.Ftp:
                        case EUrlScheme.Gopher:
                        case EUrlScheme.Http:
                        case EUrlScheme.Https:
                        case EUrlScheme.Ws:
                        case EUrlScheme.Wss:
                            {
                                return new UrlOrigin(Scheme, Host, Port, null);
                            }
                        case EUrlScheme.File:
                            return UrlOrigin.Opaque;
                    }
                }

                return UrlOrigin.Opaque;
            }
        }

        /// <summary>
        /// Returns the default port(if any) for the urls scheme
        /// </summary>
        public int? DefaultPort
        {
            get
            {
                if (Scheme.IsCustom || !Scheme.EnumValue.HasValue) return null;

                if (Lookup.TryData<EUrlScheme>(Scheme.EnumValue.Value, out EnumData outData))
                {
                    return outData.Data[0];
                }

                return null;
            }
        }

        /// <summary>
        /// Returns <c>True</c> if this urls scheme is a special scheme
        /// </summary>
        public bool IsSpecial
        {/* Docs: https://url.spec.whatwg.org/#is-special */
            get => Scheme.EnumValue.HasValue;
        }

        /// <summary>
        /// Returns <c>True</c> if this urls username or password are a non-empty string
        /// </summary>
        public bool IncludesCredentials
        {/* Docs: https://url.spec.whatwg.org/#include-credentials */
            get => (Username?.Length > 0) || (Password?.Length > 0);
        }

        /// <summary>
        /// A URL cannot have a username/password/port if its host is null or the empty string, its cannot-be-a-base-URL flag is set, or its scheme is "file".
        /// </summary>
        public bool CannotHaveCredentials
        {/* Docs: https://url.spec.whatwg.org/#cannot-have-a-username-password-port */
            get
            {
                return (Host == null || (Host.Value is string hostString && hostString.Length <= 0) || bCannotBeBaseURLFlag || Scheme == EUrlScheme.File);
            }
        }
        #endregion

        #region Parsing
        public static Url Parse(ReadOnlyMemory<char> input, Url urlBase = null, Encoding encodingOverride = null)
        {
            if (TryParse(input, out Url outUrl, urlBase, encodingOverride))
            {
                return outUrl;
            }

            return null;
        }
        public static bool TryParse(ReadOnlyMemory<char> input, out Url outUrl, Url urlBase = null, Encoding encodingOverride = null)
        {/* Docs: https://url.spec.whatwg.org/#concept-url-parser */
            if (Parse_Basic(input, urlBase, out Url url, encodingOverride))
            {
                outUrl = null;
                return false;
            }

            if (url.Scheme != (AtomicName<EUrlScheme>)"blob")
            {
                outUrl = url;
                return true;
            }

            if (!BlobURLEntry.Resolve(url, out BlobURLEntry outBlob))
            {
                outUrl = null;
                return false;
            }

            url.blobURLEntry = outBlob;
            outUrl = url;
            return true;
        }
        public static bool Parse_Basic(ReadOnlyMemory<char> input, Url Base, out Url outUrl, Encoding encodingOverride = null, Url url = null, ESchemeState? stateOverride = null)
        {/* Docs: https://url.spec.whatwg.org/#concept-basic-url-parser */
            if (url == null)
            {
                url = new Url();
                if (Is_Ascii_Control_Or_Space(input.Span[0]) || Is_Ascii_Control_Or_Space(input.Span[input.Length-1]))
                {
                    Log.Warn($"Encountered leading or trailing C0 control or space character in url string: \"{input.ToString()}\"");
                    input = StringCommon.Trim(input, Is_Ascii_Control_Or_Space);
                }
            }

            if (StringCommon.Contains(input.Span, Is_Ascii_Tab_Or_Newline))
            {
                Log.Warn($"Encountered tab or newline character in url string: \"{input.ToString()}\"");
                input = StringCommon.Trim(input, Is_Ascii_Tab_Or_Newline);
            }

            /* 4) Let state be state override if given, or scheme start state otherwise. */
            var state = stateOverride ?? ESchemeState.SchemeStart;
            var encoding = Encoding.UTF8;
            if (encodingOverride != null) encoding = HTMLCommon.Get_Output_Encoding(encodingOverride);

            StringBuilder buffer = new StringBuilder();
            bool bAtFlag = false, bSquareBracketFlag = false, bPasswordTokenSeenFlag = false;
            DataStream<char> Stream = new DataStream<char>(input, EOF);

            /* Keep running the following state machine by switching on state. If after a run pointer points to the EOF code point, go to the next step. 
             * Otherwise, increase pointer by one and continue with the state machine. */
            while (!Stream.atEOF)
            {
                switch (state)
                {
                    case ESchemeState.SchemeStart:
                        {
                            if (Is_Ascii_Alpha(Stream.Next))
                            {
                                buffer.Append(To_ASCII_Lower_Alpha(Stream.Next));
                                state = ESchemeState.Scheme;
                            }
                            else if (!stateOverride.HasValue)
                            {
                                state = ESchemeState.NoScheme;
                                continue;/* Decrease pointer by one (skip the Stream.Consume call) */
                            }
                            else
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                            }
                        }
                        break;
                    case ESchemeState.Scheme:
                        {
                            if (Is_Ascii_Alpha(Stream.Next) || Stream.Next == CHAR_PLUS_SIGN || Stream.Next == CHAR_HYPHEN_MINUS || Stream.Next == CHAR_FULL_STOP)
                            {
                                buffer.Append(To_ASCII_Lower_Alpha(Stream.Next));
                            }
                            else if (Stream.Next == CHAR_COLON)
                            {
                                /* 1) If state override is given, then: */
                                if (stateOverride.HasValue)
                                {
                                    /* 1) If url’s scheme is a special scheme and buffer is not a special scheme, then return. */
                                    if (url.Scheme.EnumValue.HasValue && !Lookup.TryEnum<EUrlScheme>(buffer.ToString(), out _))
                                    {
                                        outUrl = url;
                                        return true;
                                    }
                                    /* 2) If url’s scheme is not a special scheme and buffer is a special scheme, then return. */
                                    if (!url.Scheme.EnumValue.HasValue && Lookup.TryEnum<EUrlScheme>(buffer.ToString(), out _))
                                    {
                                        outUrl = url;
                                        return true;
                                    }
                                    /* 3) If url includes credentials or has a non-null port, and buffer is "file", then return. */
                                    if (url.Username.Length > 0 || url.Password.Length > 0 || url.Port.HasValue)
                                    {
                                        outUrl = url;
                                        return true;
                                    }
                                    /* 4) If url’s scheme is "file" and its host is an empty host or null, then return. */
                                    if (url.Scheme == EUrlScheme.File && string.IsNullOrEmpty(url.Host))
                                    {
                                        outUrl = url;
                                        return true;
                                    }
                                }
                                /* 2) Set url’s scheme to buffer. */
                                url.Scheme = buffer.ToString();
                                /* 3) If state override is given, then: */
                                if (stateOverride.HasValue)
                                {
                                    if (url.Port == url.DefaultPort)
                                    {
                                        url.Port = null;
                                    }

                                    outUrl = url;
                                    return true;
                                }
                                buffer.Clear();
                                /* 5) If url’s scheme is "file", then: */
                                if (url.Scheme == EUrlScheme.File)
                                {
                                    var slash = new char[2] { CHAR_REVERSE_SOLIDUS, CHAR_REVERSE_SOLIDUS };
                                    if (!Stream.Slice(1, 2).Equals(slash))
                                    {
                                        Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                    }
                                    state = ESchemeState.File;
                                }
                                /* 6) Otherwise, if url is special, base is non-null, and base’s scheme is equal to url’s scheme, set state to special relative or authority state. */
                                else if (url.IsSpecial && !ReferenceEquals(null, Base) && Base.Scheme == url.Scheme)
                                {
                                    state = ESchemeState.SpecialRelativeOrAuthority;
                                }
                                /* 7) Otherwise, if url is special, set state to special authority slashes state. */
                                else if (url.IsSpecial)
                                {
                                    state = ESchemeState.SpecialAuthoritySlashes;
                                }
                                /* 8) Otherwise, if remaining starts with an U+002F (/), set state to path or authority state and increase pointer by one. */
                                else if (Stream.NextNext == CHAR_SOLIDUS)
                                {
                                    state = ESchemeState.PathOrAuthority;
                                    Stream.Consume();
                                }
                                /* 9) Otherwise, set url’s cannot-be-a-base-URL flag, append an empty string to url’s path, and set state to cannot-be-a-base-URL path state. */
                                else
                                {
                                    url.bCannotBeBaseURLFlag = true;
                                    url.Path.Add(string.Empty);
                                    state = ESchemeState.CannotBeBaseURLPath;
                                }
                            }
                            else if (!stateOverride.HasValue)
                            {
                                buffer.Clear();
                                state = ESchemeState.NoScheme;
                                Stream.Seek(0);
                                continue;
                            }
                            else
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                            }
                        }
                        break;
                    case ESchemeState.NoScheme:
                        {
                            if ((Base == null || Base.bCannotBeBaseURLFlag) && Stream.Next != CHAR_HASH)
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                outUrl = null;
                                return false;
                            }
                            else if (Base.bCannotBeBaseURLFlag && Stream.Next == CHAR_HASH)
                            {
                                url.Scheme = Base.Scheme;
                                url.Path = new List<string>(Base.Path);
                                url.Query = Base.Query;
                                url.Fragment = string.Empty;
                                url.bCannotBeBaseURLFlag = true;
                                state = ESchemeState.Fragment;
                            }
                            else if (Base.Scheme != EUrlScheme.File)
                            {
                                state = ESchemeState.Relative;
                                continue;/* Decrease pointer by one */
                            }
                            else
                            {
                                state = ESchemeState.File;
                                continue; /* Decrease pointer by one */
                            }
                        }
                        break;
                    case ESchemeState.SpecialRelativeOrAuthority:
                        {
                            if (Stream.Next == CHAR_SOLIDUS && Stream.NextNext == CHAR_SOLIDUS)
                            {
                                state = ESchemeState.SpecialAuthorityIgnoreSlashes;
                                Stream.Consume();
                            }
                            else
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                state = ESchemeState.Relative;
                                continue;/* Decreae pointer by one */
                            }
                        }
                        break;
                    case ESchemeState.PathOrAuthority:
                        {
                            if (Stream.Next == CHAR_SOLIDUS)
                            {
                                state = ESchemeState.Authority;
                            }
                            else
                            {
                                state = ESchemeState.Path;
                                continue; /* Decrease pointer by one */
                            }
                        }
                        break;
                    case ESchemeState.Relative:
                        {
                            url.Scheme = Base.Scheme;
                            switch (Stream.Next)
                            {
                                case EOF:
                                    {
                                        url.Username = Base.Username;
                                        url.Password = Base.Password;
                                        url.Host = Base.Host;
                                        url.Port = Base.Port;
                                        url.Path = new List<string>(Base.Path);
                                        url.Query = Base.Query;
                                    }
                                    break;
                                case CHAR_SOLIDUS:
                                    {
                                        state = ESchemeState.RelativeSlash;
                                    }
                                    break;
                                case CHAR_QUESTION_MARK:
                                    {
                                        url.Username = Base.Username;
                                        url.Password = Base.Password;
                                        url.Host = Base.Host;
                                        url.Port = Base.Port;
                                        url.Path = new List<string>(Base.Path);
                                        url.Query = string.Empty;
                                        state = ESchemeState.Query;
                                    }
                                    break;
                                case CHAR_HASH:
                                    {
                                        url.Username = Base.Username;
                                        url.Password = Base.Password;
                                        url.Host = Base.Host;
                                        url.Port = Base.Port;
                                        url.Path = new List<string>(Base.Path);
                                        url.Query = Base.Query;
                                        url.Fragment = string.Empty;
                                        state = ESchemeState.Fragment;
                                    }
                                    break;
                                default:
                                    {
                                        if (url.IsSpecial && Stream.Next == CHAR_REVERSE_SOLIDUS)
                                        {
                                            Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                            state = ESchemeState.RelativeSlash;
                                        }
                                        else
                                        {
                                            url.Username = Base.Username;
                                            url.Password = Base.Password;
                                            url.Host = Base.Host;
                                            url.Port = Base.Port;
                                            url.Path = new List<string>(Base.Path);
                                            if (url.Path.Count > 0)
                                            {
                                                url.Path.RemoveAt(url.Path.Count - 1);
                                            }
                                            state = ESchemeState.Path;
                                            continue;/* Decrease pointer by one */
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case ESchemeState.RelativeSlash:
                        {
                            if (url.IsSpecial && (Stream.Next == CHAR_SOLIDUS || Stream.Next == CHAR_REVERSE_SOLIDUS))
                            {
                                if (Stream.Next == CHAR_REVERSE_SOLIDUS)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }
                                state = ESchemeState.SpecialAuthorityIgnoreSlashes;
                            }
                            else if (Stream.Next == CHAR_SOLIDUS)
                            {
                                state = ESchemeState.Authority;
                            }
                            else
                            {
                                url.Username = Base.Username;
                                url.Password = Base.Password;
                                url.Host = Base.Host;
                                url.Port = Base.Port;
                                state = ESchemeState.Path;
                                continue;/* Decrease pointer by one */
                            }
                        }
                        break;
                    case ESchemeState.SpecialAuthoritySlashes:
                        {
                            if (Stream.Next == CHAR_SOLIDUS && Stream.NextNext == CHAR_SOLIDUS)
                            {
                                state = ESchemeState.SpecialAuthorityIgnoreSlashes;
                                Stream.Consume();
                            }
                            else
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                state = ESchemeState.SpecialAuthorityIgnoreSlashes;
                                continue;/* Decrease pointer by one */
                            }
                        }
                        break;
                    case ESchemeState.SpecialAuthorityIgnoreSlashes:
                        {
                            if (Stream.Next != CHAR_SOLIDUS && Stream.Next != CHAR_REVERSE_SOLIDUS)
                            {
                                state = ESchemeState.Authority;
                                continue;/* Decrease pointer by one */
                            }
                            else
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                            }
                        }
                        break;
                    case ESchemeState.Authority:
                        {
                            if (Stream.Next == CHAR_AT_SIGN)
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                if (bAtFlag) buffer.Insert(0, "%40");
                                bAtFlag = true;

                                string bufferStr = buffer.ToString();
                                var buff = bufferStr.AsSpan();
                                /* 4) For each codePoint in buffer: */
                                for (int i=0; i<buff.Length; i++)
                                {
                                    var codePoint = buff[i];
                                    if (codePoint == CHAR_COLON && !bPasswordTokenSeenFlag)
                                    {
                                        bPasswordTokenSeenFlag = true;
                                        continue;
                                    }

                                    var encodedCodePoints = UTF8_Percent_Encode(codePoint, Percent_Encode_Set_Userinfo);
                                    if (bPasswordTokenSeenFlag)
                                    {
                                        url.Password += encodedCodePoints;
                                    }
                                    else
                                    {
                                        url.Username += encodedCodePoints;
                                    }
                                }
                                /* 5) Set buffer to the empty string. */
                                buffer.Clear();
                            }
                            else if ((url.IsSpecial && Stream.Next == CHAR_REVERSE_SOLIDUS) || Stream.atEOF || Stream.Next == CHAR_SOLIDUS || Stream.Next == CHAR_QUESTION_MARK || Stream.Next == CHAR_HASH)
                            {
                                /* 1) If @ flag is set and buffer is the empty string, validation error, return failure. */
                                if (bAtFlag && buffer.Length <= 0)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                    outUrl = null;
                                    return false;
                                }
                                /* 2) Decrease pointer by the number of code points in buffer plus one, set buffer to the empty string, and set state to host state. */
                                Stream.Reconsume(buffer.Length);
                                buffer.Clear();
                                state = ESchemeState.Hostname;
                            }
                            else
                            {
                                buffer.Append(Stream.Next);
                            }
                        }
                        break;
                    case ESchemeState.Hostname:
                        {
                            if (stateOverride.HasValue && url.Scheme == EUrlScheme.File)
                            {
                                state = ESchemeState.FileHost;
                                continue;/* Decrease pointer by one */
                            }
                            else if (Stream.Next == CHAR_COLON && !bSquareBracketFlag)
                            {
                                /* 1) If buffer is the empty string, validation error, return failure. */
                                if (buffer.Length <= 0)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                    return null;
                                }
                                /* 2) Let host be the result of host parsing buffer with url is not special. */
                                /* 3) If host is failure, then return failure. */
                                var buffStr = buffer.ToString();
                                if (!Parse_Host(buffStr.AsMemory(), out string parsedHost, !url.IsSpecial))
                                {
                                    return null;
                                }

                                url.Host = parsedHost;
                                buffer.Clear();
                                state = ESchemeState.Port;
                                /* 5) If state override is given and state override is hostname state, then return. */
                                if (stateOverride.HasValue && stateOverride == ESchemeState.Hostname)
                                {
                                    return url;
                                }
                            }
                            else if ((Stream.atEOF || Stream.Next == CHAR_SOLIDUS || Stream.Next == CHAR_QUESTION_MARK || Stream.Next == CHAR_HASH) || (url.IsSpecial && Stream.Next == CHAR_REVERSE_SOLIDUS))
                            {
                                /* x) then decrease pointer by one, and then: */
                                Stream.Reconsume(1);
                                if (url.IsSpecial && buffer.Length <= 0)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                    return null;
                                }
                                else if (stateOverride.HasValue && buffer.Length <= 0 && (url.IncludesCredentials || url.Port.HasValue))
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                    return null;
                                }

                                if (!Parse_Host(buffer.ToString().AsMemory(), out string parsedHost, !url.IsSpecial))
                                {
                                    return null;
                                }

                                url.Host = parsedHost;
                                buffer.Clear();
                                state = ESchemeState.PathStart;
                                if (stateOverride.HasValue)
                                {
                                    return url;
                                }
                            }
                            else
                            {
                                if (Stream.Next == CHAR_LEFT_SQUARE_BRACKET) bSquareBracketFlag = true;
                                else if (Stream.Next == CHAR_RIGHT_SQUARE_BRACKET) bSquareBracketFlag = false;

                                buffer.Append(Stream.Next);
                            }
                        }
                        break;
                    case ESchemeState.Port:
                        {
                            if (Is_Ascii_Digit(Stream.Next))
                            {
                                buffer.Append(Stream.Next);
                            }
                            else if ((Stream.atEnd || Stream.Next == CHAR_SOLIDUS || Stream.Next == CHAR_QUESTION_MARK || Stream.Next == CHAR_HASH) || (url.IsSpecial && Stream.Next == CHAR_REVERSE_SOLIDUS) || stateOverride.HasValue)
                            {
                                if (buffer.Length > 0)
                                {
                                    var port = Int32.Parse(buffer.ToString());
                                    /* 2) If port is greater than 2^16 − 1, validation error, return failure. */
                                    if (port > 65535)
                                    {
                                        Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                        outUrl = null;
                                        return false;
                                    }

                                    if (port == url.DefaultPort)
                                        url.Port = null;
                                    else
                                        url.Port = (ushort)port;

                                    buffer.Clear();
                                }

                                if (stateOverride.HasValue)
                                {
                                    outUrl = url;
                                    return true;
                                }
                                state = ESchemeState.PathStart;
                                Stream.Reconsume(1);
                            }
                            else
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                outUrl = null;
                                return false;
                            }
                        }
                        break;
                    case ESchemeState.File:
                        {
                            url.Scheme = EUrlScheme.File;
                            if (Stream.Next == CHAR_SOLIDUS || Stream.Next == CHAR_REVERSE_SOLIDUS)
                            {
                                if (Stream.Next == CHAR_REVERSE_SOLIDUS)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }
                                state = ESchemeState.FileSlash;
                            }
                            else if (Base != null && Base.Scheme == EUrlScheme.File)
                            {
                                switch (Stream.Next)
                                {
                                    case EOF:
                                        {
                                            url.Host = Base.Host;
                                            url.Path = new List<string>(Base.Path);
                                            url.Query = Base.Query;
                                        }
                                        break;
                                    case CHAR_QUESTION_MARK:
                                        {
                                            url.Host = Base.Host;
                                            url.Path = new List<string>(Base.Path);
                                            url.Query = string.Empty;
                                            state = ESchemeState.Query;
                                        }
                                        break;
                                    case CHAR_HASH:
                                        {
                                            url.Host = Base.Host;
                                            url.Path = new List<string>(Base.Path);
                                            url.Query = Base.Query;
                                            url.Fragment = string.Empty;
                                            state = ESchemeState.Fragment;
                                        }
                                        break;
                                    default:
                                        {
                                            if (!Starts_With_Windows_Drive_Letter(Stream.Slice(0)))
                                            {
                                                url.Host = Base.Host;
                                                url.Path = new List<string>(Base.Path);
                                                Shorten_Url_Path(ref url);
                                            }
                                            else
                                            {
                                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                            }

                                            state = ESchemeState.Path;
                                            continue;/* Decrease pointer by one */
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                state = ESchemeState.Path;
                                Stream.Reconsume(1);
                            }
                        }
                        break;
                    case ESchemeState.FileSlash:
                        {
                            if (Stream.Next == CHAR_SOLIDUS || Stream.Next == CHAR_REVERSE_SOLIDUS)
                            {
                                if (Stream.Next == CHAR_REVERSE_SOLIDUS)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }
                                state = ESchemeState.FileHost;
                            }
                            else
                            {
                                if (Base != null && Base.Scheme == EUrlScheme.File && !Starts_With_Windows_Drive_Letter(Stream.Slice()))
                                {
                                    if (Is_Normalized_Windows_Drive_Letter(Base.Path[0].AsMemory()))
                                    {
                                        url.Path.Add(Base.Path[0]);
                                    }
                                    else
                                    {
                                        url.Host = Base.Host;
                                    }
                                }
                                else
                                {
                                    state = ESchemeState.Path;
                                    Stream.Reconsume(1);
                                }
                            }
                        }
                        break;
                    case ESchemeState.FileHost:
                        {
                            if (Stream.atEOF || Stream.Next == CHAR_SOLIDUS || Stream.Next == CHAR_REVERSE_SOLIDUS || Stream.Next == CHAR_QUESTION_MARK || Stream.Next == CHAR_HASH)
                            {
                                Stream.Reconsume(1);
                                if (!stateOverride.HasValue && Is_Windows_Drive_Letter(buffer.ToString().AsMemory()))
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                    state = ESchemeState.Path;
                                }
                                else if (buffer.Length <= 0)
                                {
                                    url.Host = string.Empty;
                                    if (stateOverride.HasValue)
                                    {
                                        outUrl = url;
                                        return true;
                                    }
                                    state = ESchemeState.PathStart;
                                }
                                else
                                {
                                    if (!Parse_Host(buffer.ToString().AsMemory(), out string parsedHost, !url.IsSpecial))
                                    {
                                        outUrl = null;
                                        return false;
                                    }

                                    if (parsedHost.AsSpan().Equals("localhost".AsSpan(), StringComparison.Ordinal))
                                    {
                                        parsedHost = string.Empty;
                                    }

                                    url.Host = parsedHost;
                                    if (stateOverride.HasValue)
                                    {
                                        outUrl = url;
                                        return true;
                                    }

                                    buffer.Clear();
                                    state = ESchemeState.PathStart;
                                }
                            }
                            else
                            {
                                buffer.Append(Stream.Next);
                            }
                        }
                        break;
                    case ESchemeState.PathStart:
                        {
                            if (url.IsSpecial)
                            {
                                if (Stream.Next == CHAR_REVERSE_SOLIDUS)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }

                                state = ESchemeState.Path;
                                if (Stream.Next != CHAR_SOLIDUS && Stream.Next != CHAR_REVERSE_SOLIDUS)
                                {
                                    Stream.Reconsume(1);
                                }
                            }
                            else if (!stateOverride.HasValue && Stream.Next == CHAR_QUESTION_MARK)
                            {
                                url.Query = string.Empty;
                                state = ESchemeState.Query;
                            }
                            else if (!stateOverride.HasValue && Stream.Next == CHAR_HASH)
                            {
                                url.Fragment = string.Empty;
                                state = ESchemeState.Fragment;
                            }
                            else if (!Stream.atEOF)
                            {
                                state = ESchemeState.Path;
                                if (Stream.Next != CHAR_SOLIDUS)
                                {
                                    Stream.Reconsume(1);
                                }
                            }
                        }
                        break;
                    case ESchemeState.Path:
                        {
                            if ((Stream.atEOF || Stream.Next == CHAR_SOLIDUS) || (url.IsSpecial && Stream.Next == CHAR_REVERSE_SOLIDUS) || (!stateOverride.HasValue && (Stream.Next == CHAR_QUESTION_MARK || Stream.Next == CHAR_HASH)))
                            {
                                if (url.IsSpecial && Stream.Next == CHAR_REVERSE_SOLIDUS)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }

                                /* 2) If buffer is a double-dot path segment, shorten url’s path, and then if neither c is U+002F (/), nor url is special and c is U+005C (\), append the empty string to url’s path. */
                                if (Is_Double_Dot_Path_Segment(buffer.ToString().AsMemory()))
                                {
                                    Shorten_Url_Path(ref url);
                                    if (!url.IsSpecial && Stream.Next == CHAR_REVERSE_SOLIDUS)
                                    {
                                        url.Path.Add(string.Empty);
                                    }
                                }
                                else if (Is_Single_Dot_Path_Segment(buffer.ToString().AsMemory()) && !url.IsSpecial && Stream.Next == CHAR_REVERSE_SOLIDUS)
                                {
                                    url.Path.Add(string.Empty);
                                }
                                else if (!Is_Single_Dot_Path_Segment(buffer.ToString().AsMemory()))
                                {
                                    if (url.Scheme == EUrlScheme.File && url.Path.Count <= 0 && Is_Windows_Drive_Letter(buffer.ToString().AsMemory()))
                                    {
                                        if (!string.IsNullOrEmpty(url.Host))
                                        {
                                            Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                            url.Host = string.Empty;
                                        }

                                        buffer.Replace(buffer[1], CHAR_COLON, 1, 1);
                                    }

                                    url.Path.Add(buffer.ToString());
                                }

                                buffer.Clear();

                                if (url.Scheme == EUrlScheme.File && (Stream.atEOF || Stream.Next == CHAR_QUESTION_MARK || Stream.Next == CHAR_HASH))
                                {
                                    while (url.Path.Count > 1 && url.Path[0].Length <= 0)
                                    {
                                        Log.Warn($"Validation error url path contains an empty segment");
                                        url.Path.RemoveAt(0);
                                    }
                                }

                                if (Stream.Next == CHAR_QUESTION_MARK)
                                {
                                    url.Query = string.Empty;
                                    state = ESchemeState.Query;
                                }
                                else if (Stream.Next == CHAR_HASH)
                                {
                                    url.Fragment = string.Empty;
                                    state = ESchemeState.Fragment;
                                }
                            }
                            else
                            {
                                if (!Is_URL_Code_Point(Stream.Next) && Stream.Next != CHAR_PERCENT)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }
                                if (Stream.Next == CHAR_PERCENT && !Is_Ascii_Hex_Digit(Stream.NextNext) && !Is_Ascii_Hex_Digit(Stream.NextNextNext))
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }

                                var encoded = UTF8_Percent_Encode(Stream.Next, Percent_Encode_Set_Path);
                                buffer.Append(encoded);
                            }
                        }
                        break;
                    case ESchemeState.CannotBeBaseURLPath:
                        {
                            if (Stream.Next == CHAR_QUESTION_MARK)
                            {
                                url.Query = string.Empty;
                                state = ESchemeState.Query;
                            }
                            else if (Stream.Next == CHAR_HASH)
                            {
                                url.Fragment = string.Empty;
                                state = ESchemeState.Fragment;
                            }
                            else
                            {
                                if (!Stream.atEOF && !Is_URL_Code_Point(Stream.Next) && Stream.Next != CHAR_PERCENT)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }
                                if (Stream.Next == CHAR_PERCENT && !Is_Ascii_Hex_Digit(Stream.NextNext) && !Is_Ascii_Hex_Digit(Stream.NextNextNext))
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }
                                if (!Stream.atEOF)
                                {
                                    var encoded = UTF8_Percent_Encode(Stream.Next, Percent_Encode_Set_C0_Control);
                                    url.Path.Add(encoded);
                                }
                            }
                        }
                        break;
                    case ESchemeState.Query:
                        {
                            if (encoding != Encoding.UTF8 && (!url.IsSpecial || url.Scheme == EUrlScheme.Ws || url.Scheme == EUrlScheme.Wss))
                            {
                                encoding = Encoding.UTF8;
                            }

                            if (!stateOverride.HasValue && Stream.Next == CHAR_HASH)
                            {
                                url.Fragment = string.Empty;
                                state = ESchemeState.Fragment;
                            }
                            else if (!Stream.atEOF)
                            {
                                if (!Is_URL_Code_Point(Stream.Next) && Stream.Next != CHAR_PERCENT)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }

                                if (Stream.Next == CHAR_PERCENT && (!Is_Ascii_Hex_Digit(Stream.NextNext) || !Is_Ascii_Hex_Digit(Stream.NextNextNext)))
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }

                                var bytes = encoding.GetBytes(new char[1] { Stream.Next }).ToList();
                                if (bytes[0] == CHAR_AMPERSAND && bytes[1] == CHAR_HASH && bytes[bytes.Count-1] == CHAR_SEMICOLON)
                                {
                                    bytes.RemoveAt(0);
                                    bytes.InsertRange(0, new byte[] { (byte)CHAR_PERCENT, (byte)CHAR_DIGIT_2, (byte)CHAR_DIGIT_6, (byte)CHAR_PERCENT, (byte)CHAR_DIGIT_2, (byte)CHAR_DIGIT_3 });

                                    bytes.RemoveAt(bytes.Count - 1);
                                    bytes.InsertRange(bytes.Count - 1, new byte[] { (byte)CHAR_PERCENT, (byte)CHAR_DIGIT_3, (byte)CHAR_B_UPPER });

                                    var chars = bytes.Cast<char>().ToArray();
                                    url.Query += new string(chars);
                                }
                                else
                                {
                                    foreach (byte b in bytes)
                                    {
                                        if (b < CHAR_EXCLAMATION_POINT || b > CHAR_TILDE || b == CHAR_QUOTATION_MARK || b == CHAR_HASH || b == CHAR_LEFT_CHEVRON || b == CHAR_RIGHT_CHEVRON || (b == CHAR_APOSTRAPHE && url.IsSpecial))
                                        {
                                            var chars = Percent_Encode(b);
                                            url.Query += chars;
                                        }
                                        else
                                        {
                                            url.Query += (char)b;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case ESchemeState.Fragment:
                        {
                            if (Stream.atEnd)
                            {
                                /* Do Nothing */
                            }
                            else if (Stream.Next == 0x0)
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                            }
                            else
                            {
                                if (!Is_URL_Code_Point(Stream.Next) && Stream.Next != CHAR_PERCENT)
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }
                                else if (Stream.Next == CHAR_PERCENT && !Is_Ascii_Hex_Digit(Stream.NextNext) && !Is_Ascii_Hex_Digit(Stream.NextNextNext))
                                {
                                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                }

                                var encoded = UTF8_Percent_Encode(Stream.Next, Percent_Encode_Set_Fragment);
                                url.Fragment += encoded;
                            }
                        }
                        break;
                }

                Stream.Consume();
            }

            outUrl = url;
            return true;
        }

        #endregion

        #region Serialization
        public string Serialize(bool bExcludeFragmentFlag = false)
        {/* Docs: https://url.spec.whatwg.org/#concept-url-serializer */
            StringBuilder output = new StringBuilder();
            output.Append(Scheme.NameLower);
            output.Append(CHAR_COLON);

            if (Host != null)
            {
                output.Append(CHAR_SOLIDUS);
                output.Append(CHAR_SOLIDUS);

                if (IncludesCredentials)
                {
                    output.Append(Username);

                    if (Password.Length > 0)
                    {
                        output.Append(CHAR_COLON);
                        output.Append(Password);
                    }

                    output.Append(CHAR_AT_SIGN);
                }

                output.Append(Host.Serialize());
                if (Port.HasValue)
                {
                    output.Append(CHAR_COLON);
                    output.Append(Port.Value);
                }
            }
            else if (Host == null && Scheme == EUrlScheme.File)
            {
                output.Append(CHAR_SOLIDUS);
                output.Append(CHAR_SOLIDUS);
            }

            if (bCannotBeBaseURLFlag)
            {
                output.Append(Path[0]);
            }
            else
            {
                foreach (string path in Path)
                {
                    output.Append(CHAR_SOLIDUS);
                    output.Append(path);
                }
            }

            if (!ReferenceEquals(null, Query))
            {
                output.Append(CHAR_QUESTION_MARK);
                output.Append(Query);
            }

            if (!bExcludeFragmentFlag && !ReferenceEquals(null, Fragment))
            {
                output.Append(CHAR_HASH);
                output.Append(Fragment);
            }

            return output.ToString();
        }
        #endregion


        #region Host Parsing

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Is_Forbidden_Host_Code_Point(char c)
        {/* Docs: https://url.spec.whatwg.org/#forbidden-host-code-points */
            switch (c)
            {
                case CHAR_NULL:
                case CHAR_TAB:
                case CHAR_LINE_FEED:
                case CHAR_CARRIAGE_RETURN:
                case CHAR_SPACE:
                case CHAR_HASH:
                case CHAR_PERCENT:
                case CHAR_SOLIDUS:
                case CHAR_COLON:
                case CHAR_QUESTION_MARK:
                case CHAR_AT_SIGN:
                case CHAR_LEFT_SQUARE_BRACKET:
                case CHAR_REVERSE_SOLIDUS:
                case CHAR_RIGHT_SQUARE_BRACKET:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Parse_Host(ReadOnlyMemory<char> input, out string outHost, bool bIsNotSpecial = false)
        {/* Docs: https://url.spec.whatwg.org/#concept-host-parser */
            DataStream<char> Stream = new DataStream<char>(input, EOF);
            if (Stream.Next == CHAR_LEFT_SQUARE_BRACKET)
            {
                if (Stream.Peek(Stream.LongLength - 1) != CHAR_RIGHT_SQUARE_BRACKET)
                {
                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                    outHost = null;
                    return false;
                }

                var result = Parse_IPV6(Stream.Slice(1, Stream.Remaining - 2), out IPAddress IPV6);
                outHost = IPV6.ToString();
                return result;
            }
            /* 3) If isNotSpecial is true, then return the result of opaque-host parsing input. */
            if (bIsNotSpecial)
            {
                var result = Parse_Opaque_Host(Stream.Slice(), out string parsedHost);
                outHost = parsedHost;
                return result;
            }
            /* 4) Let domain be the result of running UTF-8 decode without BOM on the string percent decoding of input. */
            var decoder = Encoding.UTF8.GetDecoder();
            var domainDecode = String_Percent_Decode(input);
            var domainLength = decoder.GetCharCount(domainDecode, 0, domainDecode.Length);
            char[] domain = new char[domainLength];
            decoder.GetChars(domainDecode, 0, domainDecode.Length, domain, domainLength);
            /* 5) Let asciiDomain be the result of running domain to ASCII on domain. */
            var asciiDomain = Domain_To_ASCII(domain);
            /* 6) If asciiDomain is failure, validation error, return failure. */
            if (ReferenceEquals(null, asciiDomain))
            {
                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                outHost = null;
                return false;
            }

            if (StringCommon.Contains(asciiDomain.AsSpan(), Is_Forbidden_Host_Code_Point))
            {
                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                outHost = null;
                return false;
            }

            /* 8) Let ipv4Host be the result of IPv4 parsing asciiDomain. */
            /* 9) If ipv4Host is an IPv4 address or failure, return ipv4Host. */
            if (!Parse_IPV4(asciiDomain.AsMemory(), out IPAddress ipv4Host) || ipv4Host != null)
            {
                outHost = ipv4Host?.ToString();
                return true;
            }

            outHost = asciiDomain;
            return true;
        }
        public static bool Parse_Opaque_Host(ReadOnlyMemory<char> input, out string outHost)
        {/* Docs: https://url.spec.whatwg.org/#concept-opaque-host-parser */
            DataStream<char> Stream = new DataStream<char>(input, EOF);
            if (StringCommon.Contains(input.Span, c => { return c != CHAR_PERCENT && Is_Forbidden_Host_Code_Point(c); }))
            {
                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                outHost = null;
                return false;
            }

            StringBuilder output = new StringBuilder();
            while (!Stream.atEnd)
            {
                var codePoint = Stream.Consume();
                var encoded = UTF8_Percent_Encode(codePoint, Percent_Encode_Set_C0_Control);
                output.Append(encoded);
            }

            outHost = output.ToString();
            return true;
        }


        public static bool Parse_IPV4_Number(ReadOnlyMemory<char> input, ref bool bValidationErrorFlag, out long outValue)
        {/* Docs: https://url.spec.whatwg.org/#ipv4-number-parser */
            var Stream = new DataStream<char>(input, EOF);
            int R = 10;
            if (Stream.Remaining >= 2 && Stream.Next == CHAR_DIGIT_0 && (Stream.NextNext == CHAR_X_LOWER || Stream.NextNext == CHAR_X_UPPER))
            {
                bValidationErrorFlag = true;
                Stream.Consume(2);
                R = 16;
            }
            else if (Stream.Remaining >= 2 && Stream.Next == CHAR_DIGIT_0)
            {
                bValidationErrorFlag = true;
                Stream.Consume();
                R = 8;
            }

            if (Stream.atEnd)
            {
                outValue = 0;
                return false;
            }

            var remainMem = Stream.Slice(0);
            var remaining = remainMem.Span;
            long value = 0;
            int b = 1;

            foreach (var c in remaining)
            {
                var num = Ascii_Hex_To_Value(c);
                if (num >= R)
                {
                    outValue = 0;
                    return false;
                }

                value += (num * b);
                b *= R;
            }

            outValue = value;
            return true;
        }
        static long[] IPV4_POW_LUT = new long[4] { 256, MathExt.Pow(256, 2), MathExt.Pow(256, 3), MathExt.Pow(256, 4) };
        public static bool Parse_IPV4(ReadOnlyMemory<char> input, out IPAddress outAddress)
        {/* Docs: https://url.spec.whatwg.org/#concept-ipv4-parser */
            var Stream = new DataStream<char>(input, EOF);
            bool bValidationErrorFlag = false;
            IReadOnlyList<ReadOnlyMemory<char>> Parts = StringCommon.Strtok(Stream.AsMemory(), CHAR_FULL_STOP);

            if (Parts[Parts.Count -1].Length <= 0)
            {
                bValidationErrorFlag = true;
                if (Parts.Count > 1)
                {
                    var list = new List<ReadOnlyMemory<char>>(Parts);
                    list.RemoveAt(list.Count - 1);
                    Parts = list;
                }
            }

            if (Parts.Count > 4)
            {
                outAddress = null;
                return false;
            }

            var numbers = new List<long>(4);
            foreach (ReadOnlyMemory<char> Part in Parts)
            {
                if (Part.Length <= 0)
                {
                    outAddress = null;
                    return false;
                }

                if (!Parse_IPV4_Number(Part, ref bValidationErrorFlag, out long outNumber))
                {
                    outAddress = null;
                    return false;
                }

                if (bValidationErrorFlag)
                {
                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                }
                else if (outNumber > 255)
                {
                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                }

                numbers.Add(outNumber);
            }

            /* 9) If any but the last item in numbers is greater than 255, return failure. */
            for (int i=0; i<numbers.Count; i++)
            {
                if (numbers[i] > 255)
                {
                    outAddress = null;
                    return false;
                }
            }

            if (numbers[numbers.Count-1] >= IPV4_POW_LUT[5-numbers.Count])
            {
                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                outAddress = null;
                return false;
            }

            long ipv4 = numbers[numbers.Count - 1];
            numbers.RemoveAt(numbers.Count - 1);
            int counter = 0;
            foreach (var n in numbers)
            {
                ipv4 += (n * IPV4_POW_LUT[3-counter]);
                counter++;
            }

            outAddress = new IPAddress(ipv4);
            return true;
        }
        public static bool Parse_IPV6(ReadOnlyMemory<char> input, out IPAddress outAddress)
        {/* Docs: https://url.spec.whatwg.org/#concept-ipv6-parser */
            var Stream = new DataStream<char>(input, EOF);
            var address = new ushort[8];
            int pieceIndex = 0;
            int? compress = null;

            if (Stream.Next == CHAR_COLON)
            {
                if (Stream.NextNext != CHAR_COLON)
                {
                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                    outAddress = null;
                    return false;
                }
                Stream.Consume(2);
                pieceIndex++;
                compress = pieceIndex;
            }

            while (!Stream.atEOF)
            {
                if (pieceIndex == 8)
                {
                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                    outAddress = null;
                    return false;
                }

                /* 2)  */
                if (Stream.Next == CHAR_COLON)
                {
                    if (compress.HasValue)
                    {
                        Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                        outAddress = null;
                        return false;
                    }

                    Stream.Consume();
                    pieceIndex++;
                    compress = pieceIndex;
                    continue;
                }
                /* 3)  */
                int value = 0, length = 0;
                Stream.Consume_While(Is_Ascii_Hex_Digit, out ReadOnlySpan<char> outHex, 4);
                length += outHex.Length;
                for (int i = 0; i <= outHex.Length; i++) { value = (value * 0x10) + Ascii_Hex_To_Value(outHex[i]); }
                /* 5)  */
                if (Stream.Next == CHAR_FULL_STOP)
                {
                    if (length == 0)
                    {
                        Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                        return null;
                    }

                    Stream.Reconsume(length);

                    if (pieceIndex > 6)
                    {
                        Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                        return null;
                    }

                    int numbersSeen = 0;
                    while (!Stream.atEOF)
                    {
                        int? ipv4Piece = null;
                        if (numbersSeen > 0)
                        {
                            if (Stream.Next == CHAR_FULL_STOP && numbersSeen < 4)
                            {
                                Stream.Consume();
                            }
                            else
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                return null;
                            }
                        }

                        if (!Is_Ascii_Digit(Stream.Next))
                        {
                            Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                            return null;
                        }
                        /* 4) While c is an ASCII digit: */
                        while (Is_Ascii_Digit(Stream.Next))
                        {
                            int number = Ascii_Digit_To_Value(Stream.Next);
                            /* 2) If ipv4Piece is null, then set ipv4Piece to number. */
                            if (ipv4Piece == null)
                            {
                                ipv4Piece = number;
                            }
                            else if (ipv4Piece == 0)
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                return null;
                            }
                            else
                            {
                                ipv4Piece = (ipv4Piece * 10) + number;
                            }
                            /* 3) If ipv4Piece is greater than 255, validation error, return failure. */
                            if (ipv4Piece > 255)
                            {
                                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                                return null;
                            }
                            Stream.Consume();
                        }
                        /* 5) Set address[pieceIndex] to address[pieceIndex] × 0x100 + ipv4Piece. */
                        address[pieceIndex] = (ushort)((address[pieceIndex] * 0x100) + ipv4Piece.Value);
                        numbersSeen++;
                        /* 7) If numbersSeen is 2 or 4, then increase pieceIndex by 1. */
                        if (numbersSeen == 2 || numbersSeen == 4)
                        {
                            pieceIndex++;
                        }
                    }
                    /* 6) If numbersSeen is not 4, validation error, return failure. */
                    if (numbersSeen != 4)
                    {
                        Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                        return null;
                    }
                    break;
                }
                else if (Stream.Next == CHAR_COLON)
                {
                    Stream.Consume();
                    if (Stream.atEOF)
                    {
                        Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                        outAddress = null;
                        return false;
                    }
                }
                else if (!Stream.atEOF)
                {
                    Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                    outAddress = null;
                    return false;
                }

                /* 8) Set address[pieceIndex] to value. */
                address[pieceIndex] = (ushort)value;
                pieceIndex++;
            }
            /* 7) If compress is non-null, then: */
            if (compress.HasValue)
            {
                var swaps = pieceIndex - compress.Value;
                pieceIndex = 7;
                /* 3) While pieceIndex is not 0 and swaps is greater than 0, swap address[pieceIndex] with address[compress + swaps − 1], and then decrease both pieceIndex and swaps by 1. */
                while (pieceIndex != 0 && swaps > 0)
                {
                    var A = address[pieceIndex];
                    var B = address[pieceIndex + swaps - 1];
                    address[pieceIndex] = B;
                    address[pieceIndex + swaps - 1] = A;
                }
            }
            else if (!compress.HasValue && pieceIndex != 8)
            {
                Log.Warn($"Validation error @ \"{ParsingCommon.Get_Location(Stream)}\"");
                outAddress = null;
                return false;
            }

            var ipv6 = new IPV6Address(address);
            outAddress = new IPAddress(ipv6.GetAddressBytes());
            return true;
        }

        public static string Domain_To_ASCII(ReadOnlyMemory<char> domain, bool bBeStrict = false)
        {/* Docs: https://url.spec.whatwg.org/#concept-domain-to-ascii */
            var idnMapping = new System.Globalization.IdnMapping();
            idnMapping.UseStd3AsciiRules = bBeStrict;
            return idnMapping.GetAscii(domain.ToString());
        }
        #endregion


        #region Checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_URL_Code_Point(char c)
        {/* Docs: https://url.spec.whatwg.org/#url-code-points */

            if (Is_Ascii_Alphanumeric(c)) return true;
            if (c >= 0x00A0 && c <= 0x10FFFD)
            {
                return !Is_Surrogate_Code_Point(c) && !Is_NonCharacter_Code_Point(c);
            }

            switch (c)
            {
                case CHAR_EXCLAMATION_POINT:
                case CHAR_DOLLAR_SIGN:
                case CHAR_AMPERSAND:
                case CHAR_APOSTRAPHE:
                case CHAR_LEFT_PARENTHESES:
                case CHAR_RIGHT_PARENTHESES:
                case CHAR_ASTERISK:
                case CHAR_PLUS_SIGN:
                case CHAR_COMMA:
                case CHAR_HYPHEN_MINUS:
                case CHAR_FULL_STOP:
                case CHAR_SOLIDUS:
                case CHAR_COLON:
                case CHAR_SEMICOLON:
                case CHAR_EQUALS:
                case CHAR_QUESTION_MARK:
                case CHAR_AT_SIGN:
                case CHAR_UNDERSCORE:
                case CHAR_TILDE:
                default:
                    return false;
            }
        }

        /// <summary>
        /// A percent-encoded byte is U+0025 (%), followed by two ASCII hex digits.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Percent_Encoded_Byte(char A, char B, char C)
        {/* Docs: https://url.spec.whatwg.org/#percent-encoded-byte */
            return A == CHAR_PERCENT && Is_Ascii_Hex_Digit(B) && Is_Ascii_Hex_Digit(C);
        }

        /// <summary>
        /// A percent-encoded byte is U+0025 (%), followed by two ASCII hex digits.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Percent_Encoded_Byte(ReadOnlyMemory<char> input)
        {/* Docs: https://url.spec.whatwg.org/#percent-encoded-byte */
            if (input.Length < 3) return false;
            return input.Span[0] == CHAR_PERCENT && Is_Ascii_Hex_Digit(input.Span[1]) && Is_Ascii_Hex_Digit(input.Span[2]);
        }

        /// <summary>
        /// The URL units are URL code points and percent-encoded bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_URL_Unit(char A, char B, char C)
        {/* Docs: https://url.spec.whatwg.org/#url-units */
            return Is_URL_Code_Point(A) || Is_Percent_Encoded_Byte(A, B, C);
        }

        /// <summary>
        /// The URL units are URL code points and percent-encoded bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_URL_Unit(ReadOnlyMemory<char> input)
        {/* Docs: https://url.spec.whatwg.org/#url-units */
            return Is_URL_Code_Point(input.Span[0]) || Is_Percent_Encoded_Byte(input.Span[0], input.Span[1], input.Span[2]);
        }

        /// <summary>
        /// Returns <c>True</c> if <paramref name="input"/> is a string containing only URL Units
        /// </summary>
        public static bool Is_URL_Unit_String(ReadOnlyMemory<char> input)
        {/* Docs: https://url.spec.whatwg.org/#url-query-string */
            DataStream<char> Stream = new DataStream<char>(input, EOF);
            while (!Stream.atEnd)
            {
                if (!Is_URL_Code_Point(Stream.Next))
                {
                    if (!Is_Percent_Encoded_Byte(Stream.Next, Stream.NextNext, Stream.NextNextNext))
                    {
                        return false;
                    }
                    else
                    {
                        Stream.Consume(3);
                    }
                }
                else
                {
                    Stream.Consume();
                }
            }

            return true;
        }

        /// <summary>
        /// A single-dot path segment must be "." or an ASCII case-insensitive match for "%2e".
        /// </summary>
        public static bool Is_Single_Dot_Path_Segment(ReadOnlyMemory<char> input)
        {/* Docs: https://url.spec.whatwg.org/#single-dot-path-segment */
            if (input.Length == 1 && input.Span[0] == CHAR_FULL_STOP) return true;
            if (input.Length == 3 && input.Span[0] == CHAR_PERCENT && input.Span[1] == CHAR_DIGIT_2 && input.Span[2] == CHAR_E_LOWER) return true;

            return false;
        }

        /// <summary>
        /// A double-dot path segment must be ".." or an ASCII case-insensitive match for ".%2e", "%2e.", or "%2e%2e".
        /// </summary>
        public static bool Is_Double_Dot_Path_Segment(ReadOnlyMemory<char> input)
        {/* Docs: https://url.spec.whatwg.org/#double-dot-path-segment */
            if (input.Span.Equals("..".AsSpan(), StringComparison.Ordinal)) return true;
            if (input.Span.Equals(".%2e".AsSpan(), StringComparison.Ordinal)) return true;
            if (input.Span.Equals("%2e.".AsSpan(), StringComparison.Ordinal)) return true;
            if (input.Span.Equals("%2e%2e".AsSpan(), StringComparison.Ordinal)) return true;

            return false;
        }



        /// <summary>
        /// A URL-query string must be zero or more URL units.
        /// </summary>
        public static bool Is_URL_Query_String(ReadOnlyMemory<char> input)
        {/* Docs: https://url.spec.whatwg.org/#url-query-string */
            return input.Length == 0 || Is_URL_Unit_String(input);
        }

        /// <summary>
        /// A URL-fragment string must be zero or more URL units.
        /// </summary>
        public static bool Is_URL_Fragment_String(ReadOnlyMemory<char> input)
        {/* Docs: https://url.spec.whatwg.org/#url-fragment-string */
            return input.Length == 0 || Is_URL_Unit_String(input);
        }

        /// <summary>
        /// A Windows drive letter is two code points, of which the first is an ASCII alpha and the second is either U+003A (:) or U+007C (|).
        /// </summary>
        public static bool Is_Windows_Drive_Letter(ReadOnlyMemory<char> input)
        {/* Docs: https://url.spec.whatwg.org/#windows-drive-letter */
            return input.Length == 2 && Is_Ascii_Alpha(input.Span[0]) && (input.Span[1] == CHAR_COLON || input.Span[1] == CHAR_PIPE);
        }

        /// <summary>
        /// A normalized Windows drive letter is two code points, of which the first is an ASCII alpha and the second is U+003A (:).
        /// </summary>
        public static bool Is_Normalized_Windows_Drive_Letter(ReadOnlyMemory<char> input)
        {/* Docs: https://url.spec.whatwg.org/#normalized-windows-drive-letter */
            return input.Length == 2 && Is_Ascii_Alpha(input.Span[0]) && input.Span[1] == CHAR_COLON;
        }

        public static bool Starts_With_Windows_Drive_Letter(ReadOnlyMemory<char> input)
        {/* Docs: https://url.spec.whatwg.org/#start-with-a-windows-drive-letter */
            /* A string starts with a Windows drive letter if all of the following are true: */
            /* its length is greater than or equal to 2 */
            if (input.Length < 2) return false;
            /* its first two code points are a Windows drive letter */
            if (!Is_Windows_Drive_Letter(input.Slice(0, 2))) return false;
            /* its length is 2 or its third code point is U+002F (/), U+005C (\), U+003F (?), or U+0023 (#). */
            if (input.Length == 2) return true;
            switch (input.Span[2])
            {
                case CHAR_SOLIDUS:
                case CHAR_REVERSE_SOLIDUS:
                case CHAR_QUESTION_MARK:
                case CHAR_HASH:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region URL Common
        public static void Shorten_Url_Path(ref Url url)
        {/* Docs: https://url.spec.whatwg.org/#shorten-a-urls-path */
            if (url.Path.Count <= 0) return;
            if (url.Scheme == EUrlScheme.File && url.Path.Count == 1 && Is_Normalized_Windows_Drive_Letter(url.Path[0].AsMemory())) return;
            url.Path.RemoveAt(url.Path.Count - 1);
        }
        #endregion

    }
}
