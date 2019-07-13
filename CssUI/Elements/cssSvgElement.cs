#if ENABLE_SVG
using Svg;
#endif
using System.IO;
using CssUI.CSS;
using CssUI.DOM;
using CssUI.Enums;
using CssUI.Extensions;

namespace CssUI
{
    /// <summary>
    /// Represents an SVG image element
    /// </summary>
    public class cssSvgElement : cssReplacedElement
    {
        public static readonly new string CssTagName = "Svg";

        #region Accessors
    #if ENABLE_SVG
        SvgDocument Svg = null;
    #endif

        /// <summary>
        /// Sets the SVG by parsing Svg markup from a string
        /// </summary>
        public void Set_SVG(string svg_markup)
        {
            if (string.IsNullOrEmpty(svg_markup))
            {
                Clear_SVG();
            }
            else
            {
                using (var ms = new MemoryStream(svg_markup.ToByteArray()))
                {
                    Set_SVG(ms);
                }
            }
        }

        /// <summary>
        /// Sets the SVG by parsing Svg markup from a string
        /// </summary>
        public void Set_SVG(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 0)
            {
                Clear_SVG();
            }
            else
            {
                using (var ms = new MemoryStream(bytes))
                {
                    Set_SVG(ms);
                }
            }
        }

        /// <summary>
        /// Sets the SVG by parsing Svg markup from a stream
        /// </summary>
        public void Set_SVG(Stream stream)
        {
            if (stream == null)
            {
                Clear_SVG();
            }
            else
            {
#if ENABLE_SVG
                Svg = SvgDocument.Open<SvgDocument>(stream);
                Dirty_Texture = true;
                Box.Set_Intrinsic_Size((int)Svg.Bounds.Width, (int)Svg.Bounds.Height);
                Box.Set_Content_Size((int)Svg.Bounds.Width, (int)Svg.Bounds.Height);
#endif
            }
        }

        public void Clear_SVG()
        {
#if ENABLE_SVG
            Svg = null;
#endif
            Texture = null;
            Box.Set_Intrinsic_Size(null, null);
            Box.Set_Content_Size(null, null);
        }
#endregion

        #region Constructors
        public cssSvgElement(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, EReplacedElementType.SVG, className, ID)
        {
            Style.Cascaded.ObjectFit.Set(EObjectFit.Contain);
        }
        #endregion


        /// <summary>
        /// Occurs after <see cref="Update_Cached_Blocks"/>
        /// </summary>
        protected override void Handle_Resized(eSize oldSize, eSize newSize)
        {
            base.Handle_Resized(oldSize, newSize);
            Update_Texture();
        }

        #region Texture Updating
        protected override void Update_Texture()
        {
#if ENABLE_SVG
            if (Svg != null)
            {
                if (Block_ReplacedObjectArea.Width <= 0 || Block_ReplacedObjectArea.Height <= 0)
                    return;

                int Width = Block_ReplacedObjectArea.Width;
                int Height = Block_ReplacedObjectArea.Height;
                Svg.Width = (float)Width;
                Svg.Height = (float)Height;

                using (Bitmap bmp = new Bitmap(Width, Height))
                {
                    Graphics g = Graphics.FromImage(bmp);

                    ISvgRenderer SVG_Renderer;
                    SVG_Renderer = SvgRenderer.FromGraphics(g);
                    
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;// PixelOffsetMode.HighQuality
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    g.Clear(System.Drawing.Color.Transparent);

                    Svg.ShapeRendering = SvgShapeRendering.Auto;
                    Svg.Fill = new SvgColourServer(System.Drawing.Color.FromArgb(Color.iA, Color.iR, Color.iG, Color.iB));
                    Svg.Draw(SVG_Renderer);
                    Texture = new uiTexture(bmp);
                }
            }
#endif

            Dirty_Texture = false;
        }
#endregion
    }
}