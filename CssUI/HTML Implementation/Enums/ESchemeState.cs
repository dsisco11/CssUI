namespace CssUI.HTML
{
    public enum ESchemeState : int
    {/* Docs: https://url.spec.whatwg.org/#scheme-start-state */

        None = 0,
        SchemeStart = 1,
        Scheme,
        NoScheme,
        SpecialRelativeOrAuthority,
        PathOrAuthority,
        Relative,
        RelativeSlash,
        SpecialAuthoritySlashes,
        SpecialAuthorityIgnoreSlashes,
        Authority,
        Hostname,
        Port,
        File,
        FileSlash,
        FileHost,
        PathStart,
        Path,
        CannotBeBaseURLPath,
        Query,
        Fragment,

    }
}
