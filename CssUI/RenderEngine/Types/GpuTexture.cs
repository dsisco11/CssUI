﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

#if ENABLE_HEADLESS == false
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
#endif

namespace CssUI.Rendering
{
    /// <summary>
    /// Abstracted representation of a multi-frame animated texture used by UI elements.
    /// </summary>
    public class GpuTexture : IDisposable
    {
        public static bool PERSIST_IMAGE_DATA = false;
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
        private float[] Timeline = Array.Empty<float>();
        /// <summary>
        /// Total sum duration of all frames for this texture
        /// </summary>
        private float Duration = 0f;
        /// <summary>
        /// All frames for this (possibly animated) image texture...
        /// </summary>
        public readonly List<GpuTextureFrame> FrameAtlas = new List<GpuTextureFrame>(1);
        #endregion

        #region Accessors
        /// <summary>
        /// Returns the frame object which should be currently displayed for this texture.
        /// </summary>
        public GpuTextureFrame Frame => FrameAtlas[CurrentFrame];
        /// <summary>
        /// Size of the current texture
        /// </summary>
        public readonly Rect2i Size;
        #endregion

        #region Constructors
        private GpuTexture(ReadOnlyRect2i Size)
        {
            this.Size = new Rect2i(Size);
        }

        public GpuTexture(ReadOnlySpan<byte> Data, ReadOnlyRect2i Size, EPixelFormat Format)
        {
            this.Size = new Rect2i(Size);
#if ENABLE_HEADLESS == false
            Push_Frame(Data, Size, Format);
#endif
        }

#endregion

#region Image Loading

        public static async Task<GpuTexture> fromStream(MemoryStream Stream)
        {
            return await fromData(Stream.ToArray());
        }
        /// <summary>
        /// Creates a new GPU texture by decoding the given image data
        /// </summary>
        public static async Task<GpuTexture> fromData(ReadOnlyMemory<byte> imgData)
        {
#if ENABLE_HEADLESS
            return new GpuTexture(Rect2i.Zero);
#else
            var image = Image.Load<Rgba32>(imgData.ToArray());
            var Size = new Rect2i(image.Width, image.Height);
            var Texture = new GpuTexture(Size);

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
                        Span<byte> rgbaBytes = MemoryMarshal.AsBytes(frame.GetPixelSpan()).ToArray();
                        var size = new Rect2i(frame.Width, frame.Height);
                        var duration = delay / 1000.0f;
                        Texture.Push_Frame(rgbaBytes, size, EPixelFormat.RGBA, duration);
                    }
                }
            }
            else
            {
                Span<byte> rgbaBytes = MemoryMarshal.AsBytes(image.GetPixelSpan()).ToArray();
                var size = new Rect2i(image.Width, image.Height);
                Texture.Push_Frame(rgbaBytes, size, EPixelFormat.RGBA);
            }
            return Texture;
#endif
        }

        public static async Task<GpuTexture> fromFile(string path)
        {
#if ENABLE_HEADLESS
            return new GpuTexture(Rect2i.Zero);
#else
            return await Task.Factory.StartNew(() =>
            {
                var img = Image.Load<Rgba32>(path);
                return new GpuTexture(img);
            });
#endif
        }
#endregion

#region Destructor
        public void Dispose()
        {
            foreach (var frame in FrameAtlas)
            {
                frame.Dispose();
            }
        }
#endregion

#region Frame Pushing
        public void Push_Frame(ReadOnlySpan<byte> Data, ReadOnlyRect2i Size, EPixelFormat Format, float Time = 0f)
        {
            FrameAtlas.Add(new GpuTextureFrame(Data, new Rect2i(Size), Format, Time));
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

            Time = Time % Duration;// Loop the animation
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
            IsAnimated = FrameAtlas.Count > 1;
            List<float> timeline = new List<float>(FrameAtlas.Count);
            Duration = 0f;
            for (int i = 0; i < FrameAtlas.Count; i++)
            {
                var Frame = FrameAtlas[i];
                timeline.Add(Duration);
                Duration += Frame.Duration;
            }

            Timeline = timeline.ToArray();
        }
#endregion
    }
}
