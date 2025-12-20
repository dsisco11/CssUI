namespace CssUI;

/// <summary>
/// A rectangle with position and size for rendering operations.
/// </summary>
public readonly record struct RenderRect(float X, float Y, float Width, float Height)
{
    public float Left => X;
    public float Top => Y;
    public float Right => X + Width;
    public float Bottom => Y + Height;

    public static RenderRect FromLTRB(float left, float top, float right, float bottom)
        => new(left, top, right - left, bottom - top);
}

