using CssUI.DOM.Enums;

namespace CssUI.DOM.Exceptions
{
    public class DOMException : System.Exception
    {
        public readonly string Name;
        public readonly string Message;
        public EDomExceptionCode Code {
            get
            {
                switch (this.Name)
                {
                    case "HierarchyRequestError":
                        return EDomExceptionCode.HIERARCHY_REQUEST_ERR;
                    case "WrongDocumentError":
                        return EDomExceptionCode.WRONG_DOCUMENT_ERR;
                    case "InvalidCharacterError":
                        return EDomExceptionCode.INVALID_CHARACTER_ERR;
                    case "NoModificationAllowedError":
                        return EDomExceptionCode.NO_MODIFICATION_ALLOWED_ERR;
                    case "NotFoundError":
                        return EDomExceptionCode.NOT_FOUND_ERR;
                    case "NotSupportedError":
                        return EDomExceptionCode.NOT_SUPPORTED_ERR;
                    case "InUseAttributeError":
                        return EDomExceptionCode.INUSE_ATTRIBUTE_ERR;
                    case "InvalidStateError":
                        return EDomExceptionCode.INVALID_STATE_ERR;
                    case "SyntaxError":
                        return EDomExceptionCode.SYNTAX_ERR;
                    case "InvalidModificationError":
                        return EDomExceptionCode.INVALID_MODIFICATION_ERR;
                    case "NamespaceError":
                        return EDomExceptionCode.NAMESPACE_ERR;
                    case "SecurityError":
                        return EDomExceptionCode.SECURITY_ERR;
                    case "NetworkError":
                        return EDomExceptionCode.NETWORK_ERR;
                    case "AbortError":
                        return EDomExceptionCode.ABORT_ERR;
                    case "URLMismatchError":
                        return EDomExceptionCode.URL_MISMATCH_ERR;
                    case "QuotaExceededError":
                        return EDomExceptionCode.QUOTA_EXCEEDED_ERR;
                    case "TimeoutError":
                        return EDomExceptionCode.TIMEOUT_ERR;
                    case "InvalidNodeTypeError":
                        return EDomExceptionCode.INVALID_NODE_TYPE_ERR;
                    case "DataCloneError":
                        return EDomExceptionCode.DATA_CLONE_ERR;
                    case "":
                        return EDomExceptionCode.DATA_CLONE_ERR;
                    default:
                        return 0x0;
                }
            }
        }


        public DOMException(string message = "", string name = "Error")
        {
            this.Name = name;
            this.Message = message;
        }
    }
}
