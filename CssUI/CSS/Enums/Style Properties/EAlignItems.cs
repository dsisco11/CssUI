using CssUI.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Values for the align-items and align-self properties.
    /// Spec: https://www.w3.org/TR/css-align-3/#propdef-align-items
    /// </summary>
    [MetaEnum]
    public enum EAlignItems
    {
        /// <summary>
        /// Behaves as 'stretch' for flex items, 'start' for grid items.
        /// For align-self, computes to the parent's align-items value.
        /// </summary>
        [MetaKeyword("auto")]
        Auto,

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
        /// Align to the item's own start edge.
        /// </summary>
        [MetaKeyword("self-start")]
        SelfStart,

        /// <summary>
        /// Align to the item's own end edge.
        /// </summary>
        [MetaKeyword("self-end")]
        SelfEnd,

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
        /// Stretch items to fill the container.
        /// </summary>
        [MetaKeyword("stretch")]
        Stretch,

        /// <summary>
        /// Align items to their baseline.
        /// </summary>
        [MetaKeyword("baseline")]
        Baseline
    }
}

