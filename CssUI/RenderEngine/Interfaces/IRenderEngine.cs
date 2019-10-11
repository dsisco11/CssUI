using System;
using CssUI.CSS;

namespace CssUI.Rendering
{
    /// <summary>
    /// Represents a Rendering engine for displaying UI elements
    /// </summary>
    public interface IRenderEngine
    {
        #region Logic Functions
        /// <summary>
        /// Prepares the engine to begin rendering a new set of elements
        /// <para>Ensure that:</para>
        /// <para>DepthTesting = OFF</para>
        /// <para>Blending = ON</para>
        /// </summary>
        void Begin();
        /// <summary>
        /// Releases the engine
        /// </summary>
        void End();
        /// <summary>
        /// Resets all states to their defaults
        /// </summary>
        void Reset();
        /// <summary>
        /// Pushes a new copy of any stacking values onto their respective stacks, each UI element calls this at the start of it's rendering function
        /// EG: Blending color, clipping region, etc.
        /// </summary>
        void Push();
        /// <summary>
        /// Pops the latest copy of any stacking values off of their respective stacks, each UI element calls this at the end of it's rendering function
        /// EG: Blending color, clipping region, etc.
        /// </summary>
        void Pop();
        #endregion

        #region Matrix
        /// <summary>
        /// Sets the current matrix value.
        /// </summary>
        /// <param name="Matrix">The matrix value to set</param>
        void Set_Matrix(Matrix4 Matrix);

        /// <summary>
        /// Uploads the current matrix value to whatever system is doing the rendering, be it DirectX, OpenGL, Vulkan, D3D, etc.
        /// </summary>
        void Upload_Matrix(Matrix4 Matrix);
        #endregion

        #region Blending
        /// <summary>
        /// Sets the value of the latest tint color value in the blending stack.
        /// The 'tint' color refers to the color which the base color multiplies against itsself to obtain the final color value to be used when rendering verticies.
        /// </summary>
        /// <param name="color">The tint color to multiply the base color by.</param>
        void Set_Blending_Color(ReadOnlyColor color);
        #endregion

        #region Color
        /// <summary>
        /// Sets the current color.
        /// </summary>
        void Set_Color(ReadOnlyColor color);
        /// <summary>
        /// Sets the current color.
        /// </summary>
        void Set_Color(float R, float G, float B, float A);

        /// <summary>
        /// Uploads the final, blended, color value to whatever system is doing the rendering, be it DirectX, OpenGL, Vulkan, D3D, etc.
        /// </summary>
        void Upload_Color(ReadOnlyColor ReadOnlyColor);
        #endregion

        #region Texturing
        /// <summary>
        /// Sets the current texture for whatever system is doing the rendering, be it DirectX, OpenGL, Vulkan, D3D, etc.
        /// </summary>
        void Set_Texture(GpuTexture tex);
        /// <summary>
        /// Creates a new texture object.
        /// </summary>
        /// <param name="Data">Pixel data for the texture</param>
        /// <param name="Size">Pixel dimensions of the texture</param>
        /// <param name="Format">Format for the pixels in Data</param>
        object Create_Texture(ReadOnlySpan<byte> Data, ReadOnlyRect2i Size, EPixelFormat Format);
        /// <summary>
        /// Destroy a texture, ensuring it cannot be used again unless recreated.
        /// </summary>
        bool Destroy_Texture(GpuTexture texture);
        #endregion

        #region Outlining
        /// <summary>
        /// Draws a line between two given points
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="x1">X-axis of the first point</param>
        /// <param name="y1">Y-axis of the first point</param>
        /// <param name="x2">X-axis of the second point</param>
        /// <param name="y2">Y-axis of the second point</param>
        void Draw_Line(int LineThickness, int x1, int y1, int x2, int y2);
        /// <summary>
        /// Draws a line between two given points
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="v1">First vertex</param>
        /// <param name="v2">Second vertex</param>
        void Draw_Line(int LineThickness, Vertex2i v1, Vertex2i v2);
        /// <summary>
        /// Outlines a rectangular area with the currently set color
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="block">An element block which describes the rectangular area</param>
        void Draw_Rect(int LineThickness, CssRect block);
        /// <summary>
        /// Outlines a rectangular area with the currently set color
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="pos">Origin location of the area</param>
        /// <param name="size">Size of the area</param>
        void Draw_Rect(int LineThickness, ReadOnlyPoint2i Pos, ReadOnlyRect2i Size);
        /// <summary>
        /// Outlines a rectangular area with the currently set color
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="X">X-Axis origin location of the area</param>
        /// <param name="Y">Y-Axis origin location of the area</param>
        /// <param name="W">Width of the area</param>
        /// <param name="H">Height of the area</param>
        void Draw_Rect(int LineThickness, int X, int Y, int W, int H);
        /// <summary>
        /// Outlines a rectangular area with the currently set color
        /// <para>Clockwise winding assumed for all verticies</para>
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="v1">Top-Left vertex</param>
        /// <param name="v2">Top-Right vertex</param>
        /// <param name="v3">Bottom-Right vertex</param>
        /// <param name="v4">Bottom-Left vertex</param>
        void Draw_Rect(int LineThickness, Vertex2i v1, Vertex2i v2, Vertex2i v3, Vertex2i v4);
        /// <summary>
        /// Outlines a triangular area with the currently set color
        /// <para>Clockwise winding assumed for all verticies</para>
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="x1">X-axis of the first point</param>
        /// <param name="y1">Y-axis of the first point</param>
        /// <param name="x2">X-axis of the second point</param>
        /// <param name="y2">Y-axis of the second point</param>
        /// <param name="x3">X-axis of the third point</param>
        /// <param name="y3">Y-axis of the third point</param>
        void Draw_Tri(int LineThickness, int x1, int y1, int x2, int y2, int x3, int y3);

        /// <summary>
        /// Outlines a triangular area with the currently set color
        /// <para>Clockwise winding assumed for all verticies</para>
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="v1">First vertex</param>
        /// <param name="v2">Second vertex</param>
        /// <param name="v3">Third vertex</param>
        void Draw_Tri(int LineThickness, Vertex2i v1, Vertex2i v2, Vertex2i v3);
        #endregion

        #region Filling
        /// <summary>
        /// Fills a rectangular area with the currently set color
        /// </summary>
        /// <param name="Rect">An element block which describes the rectangular area</param>
        void Fill_Rect(CssRect Rect);
        /// <summary>
        /// Fills a rectangular area with the currently set color
        /// </summary>
        /// <param name="pos">Origin location of the area</param>
        /// <param name="size">Size of the area</param>
        void Fill_Rect(ReadOnlyPoint2i Pos, ReadOnlyRect2i Size);
        /// <summary>
        /// Fills a rectangular area with the currently set color
        /// </summary>
        /// <param name="X">X-Axis origin location of the area</param>
        /// <param name="Y">Y-Axis origin location of the area</param>
        /// <param name="W">Width of the area</param>
        /// <param name="H">Height of the area</param>
        void Fill_Rect(int X, int Y, int W, int H);
        /// <summary>
        /// Fills a rectangular area with the currently set color
        /// <para>Clockwise winding assumed for all verticies</para>
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="v1">Top-Left vertex</param>
        /// <param name="v2">Top-Right vertex</param>
        /// <param name="v3">Bottom-Right vertex</param>
        /// <param name="v4">Bottom-Left vertex</param>
        void Fill_Rect(Vertex2i v1, Vertex2i v2, Vertex2i v3, Vertex2i v4);
        /// <summary>
        /// Fills a triangular area with the currently set color
        /// </summary>
        /// <param name="x1">X-axis of the first point</param>
        /// <param name="y1">Y-axis of the first point</param>
        /// <param name="x2">X-axis of the second point</param>
        /// <param name="y2">Y-axis of the second point</param>
        /// <param name="x3">X-axis of the third point</param>
        /// <param name="y3">Y-axis of the third point</param>
        void Fill_Tri(int x1, int y1, int x2, int y2, int x3, int y3);
        /// <summary>
        /// Fills a triangular area with the currently set color
        /// <para>Clockwise winding assumed for all verticies</para>
        /// </summary>
        /// <param name="v1">First vertex</param>
        /// <param name="v2">Second vertex</param>
        /// <param name="v3">Third vertex</param>
        void Fill_Tri(Vertex2i v1, Vertex2i v2, Vertex2i v3);
        #endregion
    }
}
