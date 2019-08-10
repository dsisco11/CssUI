using CssUI.DOM.Enums;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CssUI.Filters;
using CssUI.DOM;

namespace CssUI.HTML
{
    public class Blob
    {/* Docs: https://w3c.github.io/FileAPI/#dfn-Blob */
        #region Properties
        public readonly ulong size;
        public readonly string type;
        protected byte[] data;
        #endregion

        #region Constructor
        public Blob()
        {
            data = new byte[0];
            size = 0;
            type = string.Empty;
        }

        public Blob(IReadOnlyCollection<BlobPart> blobParts, BlobPropertyBag options = null)
        {
            data = process_blob_parts(blobParts, options);
            size = (uint)data.Length;
            type = StringCommon.Transform(options.type.AsMemory(), UnicodeCommon.To_ASCII_Lower_Alpha);
        }
        #endregion

        #region Internal Utility
        private static byte[] process_blob_parts(IReadOnlyCollection<BlobPart> blobParts, BlobPropertyBag options)
        {/* Docs: https://w3c.github.io/FileAPI/#process-blob-parts */
            using (MemoryStream Stream = new MemoryStream())
            {
                foreach (var element in blobParts)
                {
                    if (element.data is string)
                    {
                        string s = element.data;
                        if (options.endings == EBlobEnding.Native)
                        {
                            string native_line_ending = "";
                            if (Environment.NewLine.Equals("\r\n"))
                            {
                                native_line_ending = StringCommon.Replace(s.AsMemory(), FilterCRLF.Instance, FilterCRLF.CRLF.AsSpan());
                            }
                            else
                            {
                                native_line_ending = StringCommon.Replace(s.AsMemory(), FilterCRLF.Instance, FilterCRLF.LF.AsSpan());
                            }

                            var encodedBytes = System.Text.Encoding.UTF8.GetBytes(native_line_ending);
                            Stream.Write(encodedBytes, (int)Stream.Position, encodedBytes.Length);
                        }
                    }
                    else if (element.data is ReadOnlyMemory<byte> bytes)
                    {
                        Stream.Write(bytes.ToArray(), (int)Stream.Position, bytes.Length);
                    }
                    else if (element.data is Blob blob)
                    {
                        Stream.Write(blob.data, (int)Stream.Position, (int)blob.size);
                    }
                }

                return Stream.ToArray();
            }
        }
        #endregion


        #region Memory
        public ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>(data);

        public ReadOnlyMemory<byte> AsMemory() => new ReadOnlyMemory<byte>(data);
        #endregion

        // slice Blob into byte-ranged chunks
        public Blob slice(long? start, long? end, string contentType = null)
        {/* Docs: https://w3c.github.io/FileAPI/#dfn-slice */
            ulong relativeStart = 0;
            if (start.HasValue)
            {
                if (start < 0) relativeStart = MathExt.Max(0, (ulong)((long)size + start.Value));
                else relativeStart = MathExt.Min((ulong)start.Value, size);
            }

            ulong relativeEnd = size;
            if (end.HasValue)
            {
                if (end < 0) relativeEnd = MathExt.Max(0, (ulong)((long)size + end.Value));
                else relativeEnd = MathExt.Min((ulong)end.Value, size);
            }

            string relativeContentType = string.Empty;
            if (!string.IsNullOrEmpty(contentType))
            {
                relativeContentType = StringCommon.Transform(contentType.AsMemory(), UnicodeCommon.To_ASCII_Lower_Alpha);
            }

            ulong span = MathExt.Max(0, relativeEnd - relativeStart);
            return new Blob(new BlobPart[] { AsMemory().Slice((int)relativeStart, (int)span) }, new BlobPropertyBag(relativeContentType));
        }

        // read from the Blob.
        public Stream stream()
        {
            return new MemoryStream(data, false);
        }

        public TaskCompletionSource<string> text()
        {
            var tcs = new TaskCompletionSource<string>();
            Task.Factory.StartNew(() =>
            {
                Stream stream = this.stream();
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);

                string decoded = System.Text.Encoding.UTF8.GetString(bytes);
                tcs.SetResult(decoded);
            }).ConfigureAwait(continueOnCapturedContext: false);
            return tcs;
        }

        public TaskCompletionSource<ReadOnlyMemory<byte>> arrayBuffer()
        {
            var tcs = new TaskCompletionSource<ReadOnlyMemory<byte>>();
            Task.Factory.StartNew(() =>
            {
                Stream stream = this.stream();
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);

                tcs.SetResult(new ReadOnlyMemory<byte>(bytes));
            }).ConfigureAwait(continueOnCapturedContext: false);

            return tcs;
        }


    }

}
