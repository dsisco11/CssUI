
namespace CssUI.CSS
{
    /// <summary>
    /// Represents a value that the Css Parser algorithm spits out
    /// </summary>
    public abstract class CssComponent : CssToken
    {
        //public readonly ECssComponent Type;

        #region Constructors
        public CssComponent(ECssTokenType Type) : base(Type)
        {
            //this.Type = Type;
        }
        #endregion
    }
}
