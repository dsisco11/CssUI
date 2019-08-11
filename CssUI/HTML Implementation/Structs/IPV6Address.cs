namespace CssUI.HTML
{
    public struct IPV6Address
    {/* Docs: https://url.spec.whatwg.org/#concept-ipv6 */

        #region Properties
        public readonly ushort[] Parts;
        #endregion

        #region Constructors

        public IPV6Address(ushort A, ushort B, ushort C, ushort D, ushort E, ushort F, ushort G, ushort H)
        {
            Parts = new ushort[8] { A, B, C, D, E, F, G, H };
        }
        #endregion
    }
}
