namespace CssUI.CSS.Internal
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="oldHash">Hash for the previous value</param>
    /// <param name="newHash">Hash for the new value</param>
    /// <param name="changes">Number of times the value has changed in total</param>
    /// <param name="hadValue"><c>True</c> if the tracker previously had a value set</param>
    public delegate void ValueTrackerEventHandler(int oldHash, int newHash, int changes, bool hadValue);
}
