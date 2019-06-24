
using System;
using CssUI.Fonts;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors;
using SixLabors.Primitives;
using System.Numerics;

namespace CssUI
{

    /// <summary>
    /// Draws lines of text
    /// </summary>
    public class cssTextElement : cssElement, ITextElement
    {
        public override string Default_CSS_TypeName { get { return "Text"; } }

        public override bool IsEmpty { get { return string.IsNullOrEmpty(text); } }

        #region Text
        /*
        Font textFont;
        public Font TextFont { get { return textFont; } set {
                textFont = value;
                Dirty_Text = true;
            }
        }
        */

        private cssTexture Texture;// The texture which stores our rendered text...
        private string text = "";
        public string Text { get { return text; } set {
                if (0 == string.Compare(value, text)) return;
                text = value;
                Invalidate_Text();
            }
        }
        #endregion

        #region Constructors
        public cssTextElement(string ID = null) : base(ID)
        {
            Style.User.BoxSizing.Set(EBoxSizingMode.CONTENT);// If the box-sizing mode isn't content then our borders & padding will warp our text because they change the content-area block size and it is no longer the same size as the rendered text!
            Style.User.Display.Set(EDisplayMode.INLINE_BLOCK);
            Style.Property_Changed += Style_Property_Change;
        }

        private void Style_Property_Change(IStyleProperty Sender, EPropertyFlags Flags, System.Diagnostics.StackTrace Origin)
        {
            if ((Flags & EPropertyFlags.Font) != 0)
                Invalidate_Text();
        }
        #endregion

        #region Dispose
        public override void Dispose()
        {
            if (Texture != null) { Texture.Dispose(); Texture = null; }
            base.Dispose();
        }
        #endregion

        #region Drawing
        protected override void Draw()
        {
            if (Dirt > 0)
            {
                if ((Dirt & EElementDirtyBit.Text) > 0)
                    Update_Text();

                Dirt = EElementDirtyBit.None;
            }

            if (Texture == null) return;
            
            Root.Engine.Set_Color(Color);
            Root.Engine.Set_Texture(Texture);
            Root.Engine.Fill_Rect(Block_Content.Get_Pos(), Texture.Size);
            Root.Engine.Set_Texture(null);
        }
        #endregion

        #region Invalidation

        private void Invalidate_Text()
        {
            Dirt &= EElementDirtyBit.Text;
        }
        #endregion

        #region Text Handling

        /// <summary>
        /// Renders our text to a GL texture
        /// </summary>
        private void Update_Text()
        {
            if (Texture != null)
            {
                Texture.Dispose();
                Texture = null;
            }
            if (Font == null) return;


            Texture = From_Text_String(text, Font);
            if (Texture != null)
            {
                Style.Set_Content_Width(Texture.Size.Width);
                Style.Set_Content_Height(Texture.Size.Height);

                //Style.Default.Width.Set(Texture.Size.Width);
                //Style.Default.Height.Set(Texture.Size.Height);
            }

            // unset the text flag
            Dirt &= ~EElementDirtyBit.Text;
        }
        

        /*internal static void Setup_Text_Options(ref Graphics g)
        {
            g.SmoothingMode = SmoothingMode.None;// SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.None;// PixelOffsetMode.HighQuality
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;

            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;// Use if outlining
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;// only use if not outlining
        }*/

        /// <summary>
        /// Returns a new texture with text rendered to it
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="textFont"></param>
        /// <returns></returns>
        public cssTexture From_Text_String(string Text, Font textFont)
        {
            if (string.IsNullOrEmpty(Text)) return null;

            GlyphBuilder glyphBuilder = new GlyphBuilder();
            TextRenderer renderer = new TextRenderer(glyphBuilder);
            RendererOptions style = new RendererOptions(textFont, Style.DpiX, Style.DpiY, PointF.Empty)
            {
                ApplyKerning = true,
                TabWidth = 5,
                WrappingWidth = 0,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            // Measure the text, make sure its a valid size
            SizeF sz = SizeF.Empty;
            sz = TextMeasurer.Measure(Text, style);

            if (sz.Width <= 0 || sz.Height <= 0)
                return null;

            cssTexture retVal = null;
            // Create a bitmap to render the text to and grab a graphics object
            using (Image<Rgba32> img = new Image<Rgba32>(Configuration.Default, 1 +(int)sz.Width, 1+(int)sz.Height, Rgba32.Transparent))
            {
                // Form the glyph path list
                renderer.RenderText(Text, style);
                // Render the text
                img.Mutate(x => x.Fill(Rgba32.White, glyphBuilder.Paths));
                // Upload bitmap to our texture
                retVal = new cssTexture(img);
            }
            return retVal;
        }

        /// <summary>
        /// Returns a new texture with text rendered to it
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="textFont"></param>
        /// <returns></returns>
        public static cssTexture Render_Text_String(string Text, string fontFamily, float fontSize, FontStyle fontStyle = FontStyle.Regular)
        {
            if (string.IsNullOrEmpty(Text)) return null;

            Font textFont = SystemFonts.CreateFont(fontFamily, fontSize, fontStyle);
            IntPtr window = Platform.Factory.SystemWindows.Get_Window();
            Vector2 Dpi = Platform.Factory.SystemMetrics.Get_DPI(window);

            GlyphBuilder glyphBuilder = new GlyphBuilder();
            TextRenderer renderer = new TextRenderer(glyphBuilder);
            RendererOptions style = new RendererOptions(textFont, Dpi.X, Dpi.Y, PointF.Empty)
            {
                ApplyKerning = true,
                TabWidth = 5,
                WrappingWidth = 0,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            // Measure the text, make sure its a valid size
            SizeF sz = SizeF.Empty;
            sz = TextMeasurer.Measure(Text, style);

            if (sz.Width <= 0 || sz.Height <= 0)
                return null;

            cssTexture retVal = null;
            // Create a bitmap to render the text to and grab a graphics object
            using (Image<Rgba32> img = new Image<Rgba32>(Configuration.Default, 1 + (int)sz.Width, 1 + (int)sz.Height, Rgba32.Transparent))
            {
                // Form the glyph path list
                renderer.RenderText(Text, style);
                // Render the text
                img.Mutate(x => x.Fill(Rgba32.White, glyphBuilder.Paths));
                // Upload bitmap to our texture
                retVal = new cssTexture(img);
            }
            return retVal;
        }
        #endregion
    }
}
