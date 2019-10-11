namespace CssUI.Difference
{
    public enum EDiffAction
    {
        /// <summary>
        /// Data was unmodified
        /// </summary>
        None,
        /// <summary>
        /// New data was added, total data length might increase
        /// </summary>
        Insertion,
        /// <summary>
        /// Existing data was removed, total data length might decrease
        /// </summary>
        Removal,
        /// <summary>
        /// Existing data was altered in place, total data length is unchanged
        /// </summary>
        Modify
    };
}
