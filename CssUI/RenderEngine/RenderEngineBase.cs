
namespace CssUI
{

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

        public eMatrix Matrix { get { return Stack.Matrix; } }
        
        /// <summary>
        /// Sets the latest matrix value in the stack.
        /// </summary>
        /// <param name="Matrix"></param>
        public virtual void Set_Matrix(eMatrix Matrix)
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
        public abstract void Upload_Matrix(eMatrix Matrix);
        #endregion
        
        #region Blending
        public cssColor Blending_Color { get { return Stack.Blend_Color; } }

        /// <summary>
        /// Sets the value of the latest tint color value in the blending stack.
        /// The 'tint' color refers to the color which the base color multiplies against itsself to obtain the final color value to be used when rendering verticies.
        /// </summary>
        /// <param name="color">The tint color to multiply the base color by.</param>
        public virtual void Set_Blending_Color(cssColor color)
        {
            Stack.Set_Blend(color);
            Finalize_Color();
        }
        #endregion

        #region Color
        public cssColor Color { get { return Stack.Color; } }

        /// <summary>
        /// Sets the current color
        /// </summary>
        public virtual void Set_Color(cssColor Color)
        {
            Stack.Set_Color(Color, false);
            Finalize_Color();
        }

        public virtual void Set_Color(float R, float G, float B, float A)
        {
            Stack.Set_Color(new cssColor(R, G, B, A), false);
            Finalize_Color();
        }

        /// <summary>
        /// Uploads the final, blended, color value to whatever system is doing the rendering, be it DirectX, OpenGL, Vulkan, D3D, etc.
        /// </summary>
        public abstract void Upload_Color(cssColor Color);

        /// <summary>
        /// Performs blending on the base color and then uploads it.
        /// </summary>
        private void Finalize_Color()
        {
            double R = (Color.R * Stack.Blend_Color.R);
            double G = (Color.G * Stack.Blend_Color.G);
            double B = (Color.B * Stack.Blend_Color.B);
            double A = (Color.A * Stack.Blend_Color.A);

            Upload_Color(new cssColor(R, G, B, A));
        }
        #endregion

        #region Texturing
        /// <summary>
        /// Sets the current texture for whatever system is doing the rendering, be it DirectX, OpenGL, Vulkan, D3D, etc.
        /// </summary>
        public abstract void Set_Texture(cssTexture tex);
        /// <summary>
        /// Creates a new texture object.
        /// </summary>
        /// <param name="Data">Pixel data for the texture</param>
        /// <param name="Size">Pixel dimensions of the texture</param>
        /// <param name="Format">Format for the pixels in Data</param>
        public abstract object Create_Texture(byte[] Data, eSize Size, EPixelFormat Format);
        /// <summary>
        /// Destroy a texture, ensuring it cannot be used again unless recreated.
        /// </summary>
        public abstract bool Destroy_Texture(cssTexture texture);
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
        public abstract void Draw_Line(int LineThickness, cssVertex v1, cssVertex v2);
        /// <summary>
        /// Outlines a rectangular area with the currently set color
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="block">An element block which describes the rectangular area</param>
        public abstract void Draw_Rect(int LineThickness, eBlock block);
        /// <summary>
        /// Outlines a rectangular area with the currently set color
        /// </summary>
        /// <param name="LineThickness">Thickness of the line in pixels</param>
        /// <param name="pos">Origin location of the area</param>
        /// <param name="size">Size of the area</param>
        public abstract void Draw_Rect(int LineThickness, ePos Pos, eSize Size);
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
        public abstract void Draw_Rect(int LineThickness, cssVertex v1, cssVertex v2, cssVertex v3, cssVertex v4);
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
        public abstract void Draw_Tri(int LineThickness, cssVertex v1, cssVertex v2, cssVertex v3);
        #endregion

        #region Filling
        /// <summary>
        /// Fills a rectangular area with the currently set color
        /// </summary>
        /// <param name="block">An element block which describes the rectangular area</param>
        public abstract void Fill_Rect(eBlock block);
        /// <summary>
        /// Fills a rectangular area with the currently set color
        /// </summary>
        /// <param name="pos">Origin location of the area</param>
        /// <param name="size">Size of the area</param>
        public abstract void Fill_Rect(ePos Pos, eSize Size);
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
        public abstract void Fill_Rect(cssVertex v1, cssVertex v2, cssVertex v3, cssVertex v4);
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
        public abstract void Fill_Tri(cssVertex v1, cssVertex v2, cssVertex v3);
        #endregion
    }
}
