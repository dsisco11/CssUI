namespace CssUI;

/// <summary>
/// Holds metadata associated with an enum value at compile-time such that it can be queried at runtime.
/// This allows us to associate extra data with enum values without using reflection.
/// </summary>
public readonly record struct EnumData(string Keyword, params object[] Data)
{
    #region Properties
    public readonly object[] Data = Data;
    public readonly string Keyword = Keyword;
    #endregion

    public object this[int i]
    {
        get => Data[i];
    }

    public int Length => Data.Length;
}

