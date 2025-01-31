﻿using System;
using CssUI.CSS;

namespace CssUI.Rendering
{

    /*
     * XXX: TODO: Move away from this outdated state machine dependent model and instead create a new system where 
     * UI items can just tell the engine how they need to look, like what geometry and colors they have,
     * and then the engine gives the UI element some sort of RenderID handle
     * Then the RenderID can be used to tell the engine to render that thing or reference it in general!
     * 
     * This acheives true seperation of concerns.
     * UI elements shouldnt be involved PERIOD with their rasterization, all they need to be able to do is tell the engine
     * HEY, RENDER ME NOW K?
     * 
     * Further this allows a huge variety of engine implementations to be created.
     * Everything from OpenGL 2.1 state machines all the way to OpenGL 4 or Vulkan stateless rendering!
     */

    /// <summary>
    /// Provides an implementation of a basic render engine with a stack.
    /// </summary>
    public abstract class RenderEngineBase : IRenderEngine
    {
        RenderStack Stack = new RenderStack();

        public RenderEngineBase()
        {
            Stack.Stack_Changed += onStack_Value_Change;
        }

        #region Logic Functions
        /// <summary>
        /// Prepares the engine to begin rendering a new frame.
        /// <para>Ensure that:</para>
        /// <para>DepthTesting = OFF</para>
        /// <para>Blending = ON</para>
        /// </summary>
        public virtual void Begin()
        {
            Reset();
        }

        /// <summary>
        /// Releases the engine when it is done rendering the current frame.
        /// </summary>
        public abstract void End();

        /// <summary>
        /// Resets all states to their defaults
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Pushes a new copy of any stacking values onto their respective stacks, each UI element calls this at the start of it's rendering function
        /// EG: Blending color, clipping region, etc.
        /// </summary>
        public virtual void Push()
        {
            Stack.Push_All();
        }

        /// <summary>
        /// Pops the latest copy of any stacking values off of their respective stacks, each UI element calls this at the end of it's rendering function
        /// EG: Blending color, clipping region, etc.
        /// </summary>
        public virtual void Pop()
        {
            Stack.Pop_All();
        }

        /// <summary>
        /// Fired anytime a value on our stack changes so we can update that data within whatever methods we are using to render.
        /// </summary>
        private void onStack_Value_Change()
        {
            Finalize_Color();
            Finalize_Matrix();
        }
        #endregion

        #region Matrix

        public Matrix4 Matrix => Stack.Matrix;

        /// <summary>
        /// Sets the latest matrix value in the stack.
        /// </summary>
        /// <param name="Matrix"></param>
        public virtual void Set_Matrix(Matrix4 Matrix)
        {
            Stack.Set_Matrix(Matrix, false);
            Finalize_Matrix();
        }

        private void Finalize_Matrix()
        {
            Upload_Matrix(Matrix);
        }

        /// <summary>
        /// Uploads the current matrix value to whatever system is doing the rendering, be it DirectX, OpenGL, Vulkan, D3D, etc.
        /// </summary>
        public abstract void Upload_Matrix(Matrix4 Matrix);
        #endregion

        #region Blending
        public ReadOnlyColor Blending_Color => Stack.Blend_Color;

        /// <summary>
        /// Sets the value of the latest tint color value in the blending stack.
        /// The 'tint' color refers to the color which the base color multiplies against itsself to obtain the final color value to be used when rendering verticies.
        /// </summary>
        /// <param name="color">The tint color to multiply the base color by.</param>
        public virtual void Set_Blending_Color(ReadOnlyColor color)
        {
            Stack.Set_Blend(color);
            Finalize_Color();
        }
        #endregion

        #region Color
        public Color Color => Stack.Color;

        /// <summary>
        /// Sets the current color
        /// </summary>
        public virtual void Set_Color(ReadOnlyColor color)
        {
            Stack.Set_Color(new Color(color), false);
            Finalize_Color();
        }

        public virtual void Set_Color(float R, float G, float B, float A)
        {
            Stack.Set_Color(new Color(R, G, B, A), false);
            Finalize_Color();
        }

        /// <summary>
        /// Uploads the final, blended, color value to whatever system is doing the rendering, be it DirectX, OpenGL, Vulkan, D3D, etc.
        /// </summary>
        public abstract void Upload_Color(ReadOnlyColor color);

        /// <summary>
        /// Performs blending on the base color and then uploads it.
        /// </summary>
        private void Finalize_Color()
        {
            var color = Color * Stack.Blend_Color;
            Upload_Color(color);
        }
        #endregion

        #region Texturing
        /// <summary>
        /// Sets the current texture for whatever system is doing the rendering, be it DirectX, OpenGL, Vulkan, D3D, etc.
        /// </summary>
        public abstract void Set_Texture(GpuTexture tex);
        /// <summary>
        /// Creates a new texture object.
        /// </summary>
        /// <param name="Data">Pixel data for the texture</param>
        /// <param name="Size">Pixel dimensions of the texture</param>
        /// <param name="Format">Format for the pixels in Data</param>
        public abstract object Create_Texture(ReadOnlySpan<byte> Data, ReadOnlyRect2i Size, EPixelFormat Format);
        /// <summary>
        /// Destroy a texture, ensuring it cannot be used again unless recreated.
        /// </summary>
        public abstract bool Destroy_Texture(GpuTexture texture);
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
        public abstract void Draw_Line(int LineThickness, int x1, int y1, int x2, int y2);
        /// <summary>
        /// Draws a line between two given points
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="v1">First vertex</param>
        /// <param name="v2">Second vertex</param>
        public abstract void Draw_Line(int LineThickness, Vertex2i v1, Vertex2i v2);
        /// <summary>
        /// Outlines a rectangular area with the currently set color
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="Rect">A rectangular area</param>
        public abstract void Draw_Rect(int LineThickness, CssRect Rect);
        /// <summary>
        /// Outlines a rectangular area with the currently set color
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="pos">Origin location of the area</param>
        /// <param name="size">Size of the area</param>
        public abstract void Draw_Rect(int LineThickness, ReadOnlyPoint2i Pos, ReadOnlyRect2i Size);
        /// <summary>
        /// Outlines a rectangular area with the currently set color
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="X">X-Axis origin location of the area</param>
        /// <param name="Y">Y-Axis origin location of the area</param>
        /// <param name="W">Width of the area</param>
        /// <param name="H">Height of the area</param>
        public abstract void Draw_Rect(int LineThickness, int X, int Y, int W, int H);
        /// <summary>
        /// Outlines a rectangular area with the currently set color
        /// <para>Clockwise winding assumed for all verticies</para>
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="v1">Top-Left vertex</param>
        /// <param name="v2">Top-Right vertex</param>
        /// <param name="v3">Bottom-Right vertex</param>
        /// <param name="v4">Bottom-Left vertex</param>
        public abstract void Draw_Rect(int LineThickness, Vertex2i v1, Vertex2i v2, Vertex2i v3, Vertex2i v4);
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
        public abstract void Draw_Tri(int LineThickness, int x1, int y1, int x2, int y2, int x3, int y3);

        /// <summary>
        /// Outlines a triangular area with the currently set color
        /// <para>Clockwise winding assumed for all verticies</para>
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="v1">First vertex</param>
        /// <param name="v2">Second vertex</param>
        /// <param name="v3">Third vertex</param>
        public abstract void Draw_Tri(int LineThickness, Vertex2i v1, Vertex2i v2, Vertex2i v3);
        #endregion

        #region Filling
        /// <summary>
        /// Fills a rectangular area with the currently set color
        /// </summary>
        /// <param name="Rect">A  rectangular area</param>
        public abstract void Fill_Rect(CssRect Rect);
        /// <summary>
        /// Fills a rectangular area with the currently set color
        /// </summary>
        /// <param name="pos">Origin location of the area</param>
        /// <param name="size">Size of the area</param>
        public abstract void Fill_Rect(ReadOnlyPoint2i Pos, ReadOnlyRect2i Size);
        /// <summary>
        /// Fills a rectangular area with the currently set color
        /// </summary>
        /// <param name="X">X-Axis origin location of the area</param>
        /// <param name="Y">Y-Axis origin location of the area</param>
        /// <param name="W">Width of the area</param>
        /// <param name="H">Height of the area</param>
        public abstract void Fill_Rect(int X, int Y, int W, int H);
        /// <summary>
        /// Fills a rectangular area with the currently set color
        /// <para>Clockwise winding assumed for all verticies</para>
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="v1">Top-Left vertex</param>
        /// <param name="v2">Top-Right vertex</param>
        /// <param name="v3">Bottom-Right vertex</param>
        /// <param name="v4">Bottom-Left vertex</param>
        public abstract void Fill_Rect(Vertex2i v1, Vertex2i v2, Vertex2i v3, Vertex2i v4);
        /// <summary>
        /// Fills a triangular area with the currently set color
        /// </summary>
        /// <param name="x1">X-axis of the first point</param>
        /// <param name="y1">Y-axis of the first point</param>
        /// <param name="x2">X-axis of the second point</param>
        /// <param name="y2">Y-axis of the second point</param>
        /// <param name="x3">X-axis of the third point</param>
        /// <param name="y3">Y-axis of the third point</param>
        public abstract void Fill_Tri(int x1, int y1, int x2, int y2, int x3, int y3);
        /// <summary>
        /// Fills a triangular area with the currently set color
        /// <para>Clockwise winding assumed for all verticies</para>
        /// </summary>
        /// <param name="v1">First vertex</param>
        /// <param name="v2">Second vertex</param>
        /// <param name="v3">Third vertex</param>
        public abstract void Fill_Tri(Vertex2i v1, Vertex2i v2, Vertex2i v3);
        #endregion
    }
}
