using CssUI.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Values for the justify-content property.
    /// Spec: https://www.w3.org/TR/css-align-3/#propdef-justify-content
    /// </summary>
    [MetaEnum]
    public enum EJustifyContent
    {
        /// <summary>
        /// Default alignment for the layout mode.
        /// </summary>
        [MetaKeyword("normal")]
        Normal,

        /// <summary>
        /// Pack items toward the start of the alignment container.
        /// </summary>
        [MetaKeyword("start")]
        Start,

        /// <summary>
        /// Pack items toward the end of the alignment container.
        /// </summary>
        [MetaKeyword("end")]
        End,

        /// <summary>
        /// Pack items toward the start of the flex container (flex-specific).
        /// </summary>
        [MetaKeyword("flex-start")]
        FlexStart,

        /// <summary>
        /// Pack items toward the end of the flex container (flex-specific).
        /// </summary>
        [MetaKeyword("flex-end")]
        FlexEnd,

        /// <summary>
        /// Pack items around the center.
        /// </summary>
        [MetaKeyword("center")]
        Center,

        /// <summary>
        /// Align to the left edge.
        /// </summary>
        [MetaKeyword("left")]
        Left,

        /// <summary>
        /// Align to the right edge.
        /// </summary>
        [MetaKeyword("right")]
        Right,

        /// <summary>
        /// Distribute items evenly, first item at start, last at end.
        /// </summary>
        [MetaKeyword("space-between")]
        SpaceBetween,

        /// <summary>
        /// Distribute items evenly with equal space around them.
        /// </summary>
        [MetaKeyword("space-around")]
        SpaceAround,

        /// <summary>
        /// Distribute items evenly with equal space between them.
        /// </summary>
        [MetaKeyword("space-evenly")]
        SpaceEvenly,

        /// <summary>
        /// Stretch items to fill the container.
        /// </summary>
        [MetaKeyword("stretch")]
        Stretch
    }
}
