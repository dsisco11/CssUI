using System;
using System.Threading;

namespace CssUI.Rendering
{
    /// <summary>
    /// Single frame of a <see cref="GpuTexture"/> instance.
    /// Effectively just serves as a convenient pass-through between RGBA pixel-data and the UI's current <see cref="IRenderEngine"/> implementation, which actually does all the handling/uploading of image data
    /// For example; an OpenGL rendering engine implementation would store a texture ID as an integer within a frame's <see cref="Handle"/> instance, while some other engine might store something else which it uses to draw the image.
    /// </summary>
    public class GpuTextureFrame : IDisposable
    {
        #region Accessors
        private ReadOnlyMemory<byte> Data;
        /// <summary>
        /// Instance object given to us by the <see cref="IRenderEngine"/> implementation, which it uses to actually handle the texture.
        /// </summary>
        public object Handle { get; private set; } = null;
        /// <summary>
        /// Time (in seconds) this frame should be onscreen for
        /// </summary>
        public readonly float Duration = 0f;
        public readonly EPixelFormat Format;
        /// <summary>
        /// Returns whether this frame is ready for the <see cref="IRenderEngine"/> instance to draw it, or if it still needs to be created.
        /// </summary>
        public bool IsReady => (null != Handle);
        #endregion

        #region Constructors
        public GpuTextureFrame(ReadOnlySpan<byte> Data, Rect2i Size, EPixelFormat Format, float Duration = 0f)
        {
            this.Format = Format;
            this.Duration = Duration;
#if ENABLE_HEADLESS == false
            this.Data = new ReadOnlyMemory<byte>(Data.ToArray());
#endif
        }
#endregion

        public void Upload(GpuTexture Owner, IRenderEngine Engine)
        {
            Handle = Engine.Create_Texture(Data.Span, Owner.Size, Format);
            if (!GpuTexture.PERSIST_IMAGE_DATA)
            {
                // Clear the pixel data from memory to minimize RAM usage
                Data = null;
            }
        }

#region Destructor
        public bool IsDisposed => (0 != Disposed);
        private int Disposed;

        ~GpuTextureFrame()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool userInitiated)
        {
            if (userInitiated)
            {
                if (Interlocked.Exchange(ref Disposed, 1) == 0)
                {
                    Handle = null;
                    Data = null;
                }
            }
        }
#endregion
    }
}