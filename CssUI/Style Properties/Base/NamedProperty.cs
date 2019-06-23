using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a named property within a UI element which can be tracked down, meaning property inheritence can be resolved
    /// </summary>
    public abstract class NamedProperty
    {
        #region Accessors
        /// <summary>
        /// The UI element which contains this property
        /// </summary>
        public uiElement Owner
        {
            set { owner = value; }
            get
            {
                if (owner == null)
                    return Parent?.Owner;
                return owner;
            }
        }
        uiElement owner = null;
        public Func<NamedProperty, CSSValue> Inheritance_Resolver;

        /// <summary>
        /// If non-null then this named property is a sub-property Eg: the property "Left" within "Margins"
        /// </summary>
        public readonly NamedProperty Parent = null;
        /// <summary>
        /// The propertys field-name in whatever class is holding it.
        /// <para>If FullName were "Margins.Left" then this would be "Left"</para>
        /// </summary>
        public AtomicString FieldName = null;
        /// <summary>
        /// The propertys identifier token in stylesheets.
        /// <para>EG; "box-sizing", "margin-left", "margin-top", etc </para>
        /// </summary>
        public AtomicString CssName = null;
        /// <summary>
        /// The full specifier name for this property
        /// </summary>
        public string FullName
        {
            get
            {
                if (Parent != null) return string.Concat(Parent.FullName, ".", FieldName.ToString());
                return FieldName.ToString();
            }
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return FullName;
        }
        #endregion

        #region Constructors
        public NamedProperty()
        {
        }
        public NamedProperty(string Name)
        {
            this.FieldName = new AtomicString(Name);
        }
        public NamedProperty(NamedProperty Parent, string Name)
        {
            this.Parent = Parent;
            this.FieldName = new AtomicString(Name);
        }
        public NamedProperty(NamedProperty Parent, string Name, string CssToken)
        {
            this.Parent = Parent;
            this.FieldName = new AtomicString(Name);
            this.CssName = new AtomicString(CssToken);
        }
        #endregion

        /// <summary>
        /// Returns the parent elements property of the same name.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="FieldName"></param>
        protected CSSValue Get_Inherited()
        {
            return Inheritance_Resolver?.Invoke(this);
        }
    }
}
