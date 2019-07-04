using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Abstracted representation of a multi-frame animated texture used by UI elements.
    /// </summary>
    public class cssTexture : IDisposable
    {
        #region Properties
        public bool IsAnimated { get; private set; } = false;
        /// <summary>
        /// The current frame number of the animation
        /// </summary>
        private int CurrentFrame = 0;
        /// <summary>
        /// Current progress (in seconds) of the animation.
        /// </summary>
        private float Time = 0f;
        /// <summary>
        /// Stores the number of seconds each frame starts at.
        /// </summary>
        private float[] Timeline = new float[0];
        /// <summary>
        /// Total sum duration of all frames for this texture
        /// </summary>
        private float Duration = 0f;
        /// <summary>
        /// All frames for this (possibly animated) image texture...
        /// </summary>
        public readonly List<cssTextureFrame> FrameAtlas = new List<cssTextureFrame>(1);
        #endregion

        #region Accessors
        /// <summary>
        /// Returns the frame object which should be currently displayed for this texture.
        /// </summary>
        public cssTextureFrame Frame { get { return FrameAtlas[CurrentFrame]; } }
        /// <summary>
        /// Size of the current texture
        /// </summary>
        public readonly Size2D Size = Size2D.Zero;
        #endregion

        #region Constructors
        public cssTexture(byte[] imgData) : this(SixLabors.ImageSharp.Image.Load<Rgba32>(imgData))
        {
        }

        public cssTexture(MemoryStream imgData) : this(SixLabors.ImageSharp.Image.Load<Rgba32>(imgData.ToArray()))
        {
        }

        public cssTexture(byte[] Data, Size2D Size, EPixelFormat Format)
        {
            this.Size = Size;
            Push_Frame(Data, Size, Format);
        }

        internal cssTexture(Image<Rgba32> image)
        {
            this.Size = new Size2D(image.Width, image.Height);

            if (image.Frames.Count > 1)
            {
                var frames = image.Frames;
                int frameCount = frames.Count;

                for (int f = 0; f < frameCount; f++)
                {
                    var frame = frames[f];
                    // Get the delay for this frame from the gif
                    int delay = 0;
                    try
                    {
                        GifFrameMetaData meta = frame.MetaData.GetFormatMetaData(GifFormat.Instance);
                        delay = meta.FrameDelay;// Time is in 1/100th of a second
                    }
                    finally
                    {
                        byte[] rgbaBytes = MemoryMarshal.AsBytes(frame.GetPixelSpan()).ToArray();
                        Push_Frame(rgbaBytes, new Size2D(frame.Width, frame.Height), EPixelFormat.RGBA, (delay / 1000.0f));
                    }
                }
            }
            else
            {
                byte[] rgbaBytes = MemoryMarshal.AsBytes(image.GetPixelSpan()).ToArray();
                Push_Frame(rgbaBytes, new Size2D(image.Width, image.Height), EPixelFormat.RGBA);
            }
        }
        #endregion

        #region Image Loading
        public static async Task<cssTexture> fromFile(string path)
        {
            return await Task.Factory.StartNew(() =>
            {
                var img = SixLabors.ImageSharp.Image.Load<Rgba32>(path);
                return new cssTexture(img);
            });
        }
        #endregion

        #region Destructor
        public void Dispose()
        {
            foreach(var frame in FrameAtlas)
            {
                frame.Dispose();
            }
        }
        #endregion

        #region Frame Pushing
        public void Push_Frame(byte[] Data, Size2D Size, EPixelFormat Format, float Time = 0f)
        {
            FrameAtlas.Add(new cssTextureFrame(Data, Size, Format) { Duration = Time });
            Build_Timeline();
        }
        #endregion

        #region Uploading
        public void Upload(IRenderEngine Engine)
        {
            Frame.Upload(this, Engine);
        }
        #endregion

        #region Frame Progression
        int? lastUpdate = null;
        /// <summary>
        /// Progresses the current frame number based on the number of seconds which have passed since it was last updated
        /// </summary>
        public void Update()
        {
            if (!IsAnimated) return;
            if (!lastUpdate.HasValue) lastUpdate = Environment.TickCount;
            
            int delta = (Environment.TickCount - lastUpdate.Value);
            lastUpdate = Environment.TickCount;
            float dT = ((float)delta / 1000.0f);

            Update(dT);
        }

        /// <summary>
        /// Progresses the current frame number based on the number of seconds which have passed since it was last updated, given by deltaTime
        /// </summary>
        /// <param name="deltaTime">Time (in seconds) to progress the animation by</param>
        public void Update(float deltaTime)
        {
            if (!IsAnimated) return;
            Time += deltaTime;// Progress the animation time

            // Handle some exceptional cases for the start index of our timeline search
            if (deltaTime < 0)
                CurrentFrame = 0;// Start frame searching from the beginning again because we are progressing backwards, a rare case.
            else if (Time >= Duration)
                CurrentFrame = 0;// Start frame searching back from the beginning as we probably looped back around here
            
            Time = (Time % Duration);// Loop the animation
            if (Time < 0) Time += Duration;// Loop back to the beginning 

            // Find the current frame number using our timeline, but start searching from the last frame we were at.
            for (int fnum = CurrentFrame; fnum < Timeline.Length; fnum++)
            {
                if (Timeline[fnum] > Time)// We arent at this frame yet
                    break;

                CurrentFrame = fnum;
            }
        }
        #endregion

        #region Timeline
        /// <summary>
        /// Rebuilds the timeline of all our frames.
        /// </summary>
        void Build_Timeline()
        {
            IsAnimated = (FrameAtlas.Count > 1);
            List<float> timeline = new List<float>(FrameAtlas.Count);
            Duration = 0f;
            for(int i=0; i<FrameAtlas.Count; i++)
            {
                var Frame = FrameAtlas[i];
                timeline.Add(Duration);
                Duration += Frame.Duration;
            }

            Timeline = timeline.ToArray();
        }
        #endregion
    }

    /// <summary>
    /// Single frame of a <see cref="cssTexture"/> instance.
    /// Effectively just serves as a convenient pass-through between RGBA pixel-data and the UI's current <see cref="IRenderEngine"/> implementation, which actually does all the handling/uploading of image data
    /// For example; an OpenGL rendering engine implementation would store a texture ID as an integer within a frame's <see cref="Instance"/> instance, while some other engine might store something else which it uses to draw the image.
    /// </summary>
    public class cssTextureFrame : IDisposable
    {
        #region Accessors
        /// <summary>
        /// Instance object given to us by the <see cref="IRenderEngine"/> implementation, which it uses to actually handle the texture.
        /// </summary>
        public object Instance { get; private set; } = null;
        /// <summary>
        /// Time (in seconds) this frame should be onscreen for
        /// </summary>
        public float Duration = 0f;
        public readonly EPixelFormat Format;
        private byte[] Data = null;
        /// <summary>
        /// Returns whether this frame is ready for the <see cref="IRenderEngine"/> instance to draw it, or if it still needs to be created.
        /// </summary>
        public bool IsReady { get { return !object.ReferenceEquals(null, Instance); } }
        #endregion

        #region Constructors
        public cssTextureFrame(byte[] Data, Size2D Size, EPixelFormat Format)
        {
            this.Format = Format;
            this.Data = Data;
        }
        #endregion

        public void Upload(cssTexture Owner, IRenderEngine Engine)
        {
            Instance = Engine.Create_Texture(Data, Owner.Size, Format);
        }

        #region Destructor
        public void Dispose()
        {
            Instance = null;
        }
        #endregion
    }
}
