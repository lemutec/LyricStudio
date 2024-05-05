﻿using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using FFME.Avalonia.Common;

namespace FFME.Rendering
{
    using Common;
    using Container;
    using Diagnostics;
    using Engine;
    using FFmpeg.AutoGen;
    using Platform;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    /// <summary>
    /// Provides Video Image Rendering via a WPF Writable Bitmap.
    /// </summary>
    /// <seealso cref="IMediaRenderer" />
    internal sealed class VideoRenderer : VideoRendererBase
    {
        #region Private State

        /// <summary>
        /// Contains an equivalence lookup of FFmpeg pixel format and WPF pixel formats.
        /// </summary>
        private static readonly Dictionary<AVPixelFormat, PixelFormat> MediaPixelFormats = new()
        {
            { AVPixelFormat.AV_PIX_FMT_BGR0, PixelFormats.Rgb565 },
            { AVPixelFormat.AV_PIX_FMT_BGRA, PixelFormats.Bgra8888 }
        };

        /// <summary>
        /// The bitmap that is presented to the user.
        /// </summary>
        private WriteableBitmap? m_TargetBitmap;

        /// <summary>
        /// The reference to a bitmap data bound to the target bitmap.
        /// </summary>
        private BitmapDataBuffer? TargetBitmapData;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoRenderer"/> class.
        /// </summary>
        /// <param name="mediaCore">The core media element.</param>
        public VideoRenderer(MediaEngine mediaCore)
            : base(mediaCore)
        {
            // Check that the renderer supports the passed in Pixel format
            if (MediaPixelFormats.ContainsKey(Constants.VideoPixelFormat) == false)
                throw new NotSupportedException($"Unable to get equivalent pixel format from source: {Constants.VideoPixelFormat}");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the target bitmap.
        /// </summary>
        private WriteableBitmap? TargetBitmap
        {
            get
            {
                return m_TargetBitmap;
            }
            set
            {
                m_TargetBitmap = value;
                if (m_TargetBitmap == null)
                {
                    TargetBitmapData = null;
                }
                else
                {
                    using var w = m_TargetBitmap.Lock();
                    TargetBitmapData = new BitmapDataBuffer(w);
                }

                MediaElement.VideoView.Source = m_TargetBitmap;
            }
        }

        #endregion

        #region MediaRenderer Methods

        /// <inheritdoc />
        public override void Render(MediaBlock mediaBlock, TimeSpan clockPosition)
        {
            var block = BeginRenderingCycle(mediaBlock);
            if (block == null) return;

            Dispatcher.UIThread.Invoke(() =>
            {
                try
                {
                    var des = new byte[block.BufferLength];
                    Marshal.Copy(block.Buffer, des,0,block.BufferLength);
                    using (MemoryStream memoryStream = new MemoryStream(des))
                    {
                        var bitmap = new Bitmap(PixelFormat.Bgra8888, AlphaFormat.Unpremul, block.Buffer,
                            new PixelSize(block.PixelWidth, block.PixelHeight), new Vector(DpiX, DpiY),
                            block.PixelWidth * 4);
                        //bitmap.Save(@"C:\avalonia.png");
                        //memoryStream.Seek(0, SeekOrigin.Begin);
                        //var render=new RenderTargetBitmap(new PixelSize(block.PixelWidth, block.PixelHeight),
                        //    new Vector(DpiX, DpiY));

                        //var bitmapImage = new WriteableBitmap(new PixelSize(block.PixelWidth, block.PixelHeight),
                        //    new Vector(DpiX, DpiY),
                        //    MediaPixelFormats[Constants.VideoPixelFormat]);
                        //memoryStream.Position = 0;
                        //BitmapDecoder decoder = BitmapDecoder.Create(memoryStream,
                        //    BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    }

                    // Prepare and write frame data
                        if (PrepareVideoFrameBuffer(block))
                        WriteVideoFrameBuffer(block, clockPosition);
                }
                catch (Exception ex)
                {
                    this.LogError(Aspects.VideoRenderer, $"{nameof(VideoRenderer)}.{nameof(Render)} bitmap failed.", ex);
                }
                finally
                {
                    FinishRenderingCycle(block, clockPosition);
                }
            },
            DispatcherPriority.Normal);
        }

        #endregion

        /// <summary>
        /// Initializes the target bitmap if not available and returns a pointer to the back-buffer for filling.
        /// </summary>
        /// <param name="block">The block.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool PrepareVideoFrameBuffer(VideoBlock block)
        {
            // Figure out what we need to do
            var needsCreation = (TargetBitmapData == null || TargetBitmap == null) && MediaElement.HasVideo;
            var needsModification = MediaElement.HasVideo && TargetBitmap != null && TargetBitmapData != null &&
                (TargetBitmapData.PixelWidth != block.PixelWidth ||
                TargetBitmapData.PixelHeight != block.PixelHeight ||
                TargetBitmapData.Stride != block.PictureBufferStride);

            var hasValidDimensions = block.PixelWidth > 0 && block.PixelHeight > 0;

            if ((!needsCreation && !needsModification) && hasValidDimensions)
                return TargetBitmapData != null;

            if (!hasValidDimensions)
            {
                TargetBitmap = null;
                return false;
            }

            // Instantiate or update the target bitmap
            TargetBitmap = new WriteableBitmap(new PixelSize(block.PixelWidth, block.PixelHeight),
                new Vector(DpiX, DpiY),
                MediaPixelFormats[Constants.VideoPixelFormat]);

            return TargetBitmapData != null;
        }

        /// <summary>
        /// Loads that target data buffer with block data.
        /// </summary>
        /// <param name="block">The source.</param>
        /// <param name="clockPosition">Current clock position.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WriteVideoFrameBuffer(VideoBlock block, TimeSpan clockPosition)
        {
            var bitmap = TargetBitmap;
            var target = TargetBitmapData;
            if (bitmap == null || target == null || block == null || block.IsDisposed || !block.TryAcquireReaderLock(out var readLock))
                return;

            // Lock the video block for reading
            try
            {
                //bitmap.Save(@"c:\avalonia1.png");
                using (var b = bitmap.Lock())
                {
                    // Compute a safe number of bytes to copy
                    // At this point, we it is assumed the strides are equal
                var bufferLength = Math.Min(block.BufferLength, target.BufferLength);
                
                // Copy the block data into the back buffer of the target bitmap.
                // Buffer.MemoryCopy(
                //     block.Buffer.ToPointer(),
                //     b.Address.ToPointer(),
                //     bufferLength,
                //     bufferLength);
                //Buffer.MemoryCopy(block.Buffer.ToPointer(),b.Address.ToPointer(),bufferLength,bufferLength);
                    Unsafe.CopyBlock(
                        b.Address.ToPointer(), 
                        block.Buffer.ToPointer(),
                        (uint)bufferLength
                        );

                // with the locked video block, raise the rendering video event.
                MediaElement?.RaiseRenderingVideoEvent(block, target, clockPosition);

                    // Mark the region as dirty so it's updated on the UI

                    
                }
                //bitmap.Save(@"c:\avalonia2.png");
            }
            finally
            {
                readLock.Dispose();
            }
        }
    }
}
