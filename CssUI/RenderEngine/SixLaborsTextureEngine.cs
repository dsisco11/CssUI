using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

#if !ENABLE_HEADLESS
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
#endif

namespace CssUI.Rendering;

/// <summary>
/// ITextureEngine implementation using SixLabors.ImageSharp.
/// </summary>
public sealed class SixLaborsTextureEngine : ITextureEngine
{
    private int _nextId = 1;
    private readonly ConcurrentDictionary<int, TextureData> _textures = new();
    
    private sealed class TextureData
    {
        public int Width { get; init; }
        public int Height { get; init; }
        public byte[] Pixels { get; set; } = Array.Empty<byte>();
    }

#if ENABLE_HEADLESS
    
    // Stub implementation for headless mode
    private static readonly string[] _supportedFormats = { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };
    
    public ReadOnlySpan<string> SupportedFormats => _supportedFormats;
    
    public ImageData DecodeImage(ReadOnlySpan<byte> data)
    {
        // Return empty image data in headless mode
        return new ImageData
        {
            Width = 1,
            Height = 1,
            Pixels = new byte[4], // Single transparent pixel
            FrameCount = 1,
            FrameDelaysMs = Array.Empty<int>()
        };
    }
    
    public ImageData DecodeImage(Stream stream)
    {
        return DecodeImage(ReadOnlySpan<byte>.Empty);
    }
    
    public TextureHandle CreateTexture(int width, int height, ReadOnlySpan<byte> rgbaPixels)
    {
        var id = _nextId++;
        _textures[id] = new TextureData
        {
            Width = width,
            Height = height,
            Pixels = rgbaPixels.ToArray()
        };
        return new TextureHandle(id);
    }
    
    public TextureHandle CreateTextureFromImage(ReadOnlySpan<byte> imageData)
    {
        var decoded = DecodeImage(imageData);
        return CreateTexture(decoded.Width, decoded.Height, decoded.Pixels);
    }
    
    public TextureHandle CreateTextureFromImage(Stream stream)
    {
        var decoded = DecodeImage(stream);
        return CreateTexture(decoded.Width, decoded.Height, decoded.Pixels);
    }
    
    public void UpdateTexture(TextureHandle handle, int x, int y, int width, int height, ReadOnlySpan<byte> rgbaPixels)
    {
        // No-op in headless mode
    }
    
    public (int Width, int Height) GetTextureSize(TextureHandle handle)
    {
        if (_textures.TryGetValue(handle.Id, out var tex))
            return (tex.Width, tex.Height);
        return (0, 0);
    }
    
    public bool IsValid(TextureHandle handle)
    {
        return !handle.IsNull && _textures.ContainsKey(handle.Id);
    }
    
    public void DestroyTexture(TextureHandle handle)
    {
        _textures.TryRemove(handle.Id, out _);
    }

#else
    
    private static readonly string[] _supportedFormats = { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp", ".tga" };
    
    public ReadOnlySpan<string> SupportedFormats => _supportedFormats;
    
    public ImageData DecodeImage(ReadOnlySpan<byte> data)
    {
        try
        {
            using var image = Image.Load<Rgba32>(data);
            return DecodeImageInternal(image);
        }
        catch
        {
            return new ImageData
            {
                Width = 0,
                Height = 0,
                Pixels = Array.Empty<byte>(),
                FrameCount = 0,
                FrameDelaysMs = Array.Empty<int>()
            };
        }
    }
    
    public ImageData DecodeImage(Stream stream)
    {
        try
        {
            using var image = Image.Load<Rgba32>(stream);
            return DecodeImageInternal(image);
        }
        catch
        {
            return new ImageData
            {
                Width = 0,
                Height = 0,
                Pixels = Array.Empty<byte>(),
                FrameCount = 0,
                FrameDelaysMs = Array.Empty<int>()
            };
        }
    }
    
    private ImageData DecodeImageInternal(Image<Rgba32> image)
    {
        int frameCount = image.Frames.Count;
        
        if (frameCount > 1)
        {
            // Animated image (GIF)
            var delays = new int[frameCount];
            var allPixels = new List<byte>();
            
            for (int f = 0; f < frameCount; f++)
            {
                var frame = image.Frames[f];
                
                // Get frame delay
                try
                {
                    var meta = frame.MetaData.GetFormatMetaData(GifFormat.Instance);
                    delays[f] = meta.FrameDelay * 10; // Convert from 1/100s to ms
                }
                catch
                {
                    delays[f] = 100; // Default 100ms
                }
                
                // Get pixel data
                var pixelSpan = MemoryMarshal.AsBytes(frame.GetPixelSpan());
                allPixels.AddRange(pixelSpan.ToArray());
            }
            
            return new ImageData
            {
                Width = image.Width,
                Height = image.Height,
                Pixels = allPixels.ToArray(),
                FrameCount = frameCount,
                FrameDelaysMs = delays
            };
        }
        else
        {
            // Single frame image
            var pixelSpan = MemoryMarshal.AsBytes(image.GetPixelSpan());
            
            return new ImageData
            {
                Width = image.Width,
                Height = image.Height,
                Pixels = pixelSpan.ToArray(),
                FrameCount = 1,
                FrameDelaysMs = Array.Empty<int>()
            };
        }
    }
    
    public TextureHandle CreateTexture(int width, int height, ReadOnlySpan<byte> rgbaPixels)
    {
        var id = _nextId++;
        _textures[id] = new TextureData
        {
            Width = width,
            Height = height,
            Pixels = rgbaPixels.ToArray()
        };
        return new TextureHandle(id);
    }
    
    public TextureHandle CreateTextureFromImage(ReadOnlySpan<byte> imageData)
    {
        var decoded = DecodeImage(imageData);
        if (decoded.Width == 0 || decoded.Height == 0)
            return TextureHandle.Null;
        
        return CreateTexture(decoded.Width, decoded.Height, decoded.Pixels);
    }
    
    public TextureHandle CreateTextureFromImage(Stream stream)
    {
        var decoded = DecodeImage(stream);
        if (decoded.Width == 0 || decoded.Height == 0)
            return TextureHandle.Null;
        
        return CreateTexture(decoded.Width, decoded.Height, decoded.Pixels);
    }
    
    public void UpdateTexture(TextureHandle handle, int x, int y, int width, int height, ReadOnlySpan<byte> rgbaPixels)
    {
        if (!_textures.TryGetValue(handle.Id, out var tex))
            return;
        
        // Update the pixel region
        int srcStride = width * 4;
        int dstStride = tex.Width * 4;
        
        for (int row = 0; row < height; row++)
        {
            int srcOffset = row * srcStride;
            int dstOffset = ((y + row) * tex.Width + x) * 4;
            
            if (dstOffset + srcStride <= tex.Pixels.Length)
            {
                rgbaPixels.Slice(srcOffset, srcStride).CopyTo(tex.Pixels.AsSpan(dstOffset));
            }
        }
    }
    
    public (int Width, int Height) GetTextureSize(TextureHandle handle)
    {
        if (_textures.TryGetValue(handle.Id, out var tex))
            return (tex.Width, tex.Height);
        return (0, 0);
    }
    
    public bool IsValid(TextureHandle handle)
    {
        return !handle.IsNull && _textures.ContainsKey(handle.Id);
    }
    
    public void DestroyTexture(TextureHandle handle)
    {
        _textures.TryRemove(handle.Id, out _);
    }
#endif
}
