﻿
using System;
using CssUI.Fonts;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.Numerics;
using CssUI.Enums;
using CssUI.DOM;
using CssUI.CSS;

namespace CssUI
{

    /// <summary>
    /// Draws lines of text
    /// </summary>
    public class cssTextElement : cssElement, ITextElement
    {
        public static new readonly string CssTagName = "Text";

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
        public cssTextElement(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, className, ID)
        {
            // If the box-sizing mode isn't content then our borders & padding will warp our text because they change the content-area block size and it is no longer the same size as the rendered text!

            Style.ImplicitRules.BoxSizing.Set(EBoxSizingMode.ContentBox);
            Style.ImplicitRules.Display.Set(EDisplayMode.INLINE_BLOCK);

            Style.onProperty_Change += Style_Property_Change;
            this.Debug.Draw_Bounds = true;
        }

        private void Style_Property_Change(ICssProperty Sender, EPropertyDirtFlags Flags, System.Diagnostics.StackTrace Origin)
        {
            // If something that affects text has changed then we invalidate our text
            if (0 != (Flags & EPropertyDirtFlags.Text))
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
            if (0 != (Dirt & EElementDirtyFlags.Text))
                Update_Text();

            if (Texture == null)
                return;
            
            Root.Engine.Set_Color(Color);
            Root.Engine.Set_Texture(Texture);
            Root.Engine.Fill_Rect(Box.Content.Get_Pos(), Texture.Size);
            Root.Engine.Set_Texture(null);
        }
        #endregion

        #region Invalidation

        private void Invalidate_Text()
        {
            Flag_Dirty(EElementDirtyFlags.Text);
        }
        #endregion

        #region Text Handling

        /// <summary>
        /// Renders our text to a GL texture
        /// </summary>
        private async void Update_Text()
        {
            if (0 != (Style.Dirt & EPropertySystemDirtFlags.Font))
                Style.Resolve_Font();

            if (Style.Font == null) return;

            if (Texture != null)
            {
                Texture.Dispose();
                Texture = null;
            }

            if (string.IsNullOrEmpty(text)) return;

            Texture = From_Text_String(text, Style.Font);
            if (Texture != null)
            {
                Box.Set_Content_Size(Texture.Size.Width, Texture.Size.Height);
            }

            // unset the text flag
            Unflag_Dirty(EElementDirtyFlags.Text);
        }
       

        /// <summary>
        /// Returns a new texture with text rendered to it
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public cssTexture From_Text_String(string Text, Font font)
        {
            if (string.IsNullOrEmpty(Text)) return null;
            if (font == null) return null;


            GlyphBuilder glyphBuilder = new GlyphBuilder();
            TextRenderer renderer = new TextRenderer(glyphBuilder);

            RendererOptions style = new RendererOptions(font, (float)Style.DpiX, (float)Style.DpiY, PointF.Empty)
            {
                ApplyKerning = true,
                TabWidth = 5,
                WrappingWidth = 0,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Measure the text, make sure its a valid size
            SizeF sz = SizeF.Empty;
            sz = TextMeasurer.Measure(Text, style);
            style.Origin = new PointF( sz.Width*0.5f, sz.Height*0.5f);

            if (sz.Width <= 0 || sz.Height <= 0 || float.IsNaN(sz.Width) || float.IsNaN(sz.Height))
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