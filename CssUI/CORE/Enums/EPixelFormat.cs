namespace CssUI;

/// <summary>
/// Specifys the format of pixel data including bit-depth and component order
/// </summary>
public enum EPixelFormat
{
    /// <summary> Each pixel is comprised of 4 color components using 8-bits, making a pixel 32-bits. </summary>
    RGBA8,
    /// <summary> Each pixel is comprised of 3 color components using 8-bits, making a pixel 24-bits. </summary>
    RGB8,
    /// <summary> Each pixel is comprised of 4 color components using 8-bits, making a pixel 32-bits. </summary>
    BGRA8,
    /// <summary> Each pixel is comprised of 3 color components using 8-bits, making a pixel 24-bits. </summary>
    BGR8,
    /// <summary> Each pixel is comprised of 1 color component using 8-bits, making a pixel 8-bits. </summary>
    GRAY8,
}

