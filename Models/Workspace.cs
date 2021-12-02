using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.ObjectModel;
using TASBoard.MovieReaders;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections.Generic;
using FFMediaToolkit.Encoding;
using FFmpeg.AutoGen;
using System.Linq;
using FFMediaToolkit.Graphics;
using FFMediaToolkit;
using System.IO;
using System.Runtime.InteropServices;

namespace TASBoard.Models
{
    public class Workspace
    {
        static bool FFmpegPathSet = false;

        public ObservableCollection<ICanvasElement> AllCanvasElements;
        public Workspace()
        {
            AllCanvasElements = new();
        }

        public SolidColorBrush backgroundColor = new(new Avalonia.Media.Color(255, 112, 112, 112));

        public void AddKey(string keyStyle, string keyName)
        {
            AllCanvasElements.Add(new KeyObject(keyStyle, keyName, displayKeyDown));
        }

        public void AddKey(KeyObject key)
        {
            AllCanvasElements.Add(key);
        }

        public void SetDisplayKeyDown(bool value)
        {
            foreach (var key in AllCanvasElements)
            {
                key.SetKeyDown(value);
            }
            displayKeyDown = value;
        }

        private bool displayKeyDown = false;
        private Rectangle encodeBounds;

        public void Encode(string moviePath, string outputPath, Fraction frameRate)
        {
#if DEBUG
            Console.WriteLine("Starting encode");
#endif
            // Get initial info out of the canvas elements
            int minX = int.MaxValue, minY = int.MaxValue, maxX = 0, maxY = 0;
            Fraction requiredBufferSeconds = 0;

            foreach (var element in AllCanvasElements)
            {
                if (element == null) { continue; }

                // Update the capture bounds if needed
                minX = Math.Min(minX, (int)element.Bounds.Left);
                minY = Math.Min(minY, (int)element.Bounds.Top);
                maxX = Math.Max(maxX, (int)element.Bounds.Right);
                maxY = Math.Max(maxY, (int)element.Bounds.Bottom);

                // Tell the elements to prepare for an encode
                element.OnBeginEncode();

                // Update the required buffer length if needed
                if (element.SecondsAheadNeeded > requiredBufferSeconds)
                    requiredBufferSeconds = element.SecondsAheadNeeded;
            }

#if DEBUG
            Console.WriteLine("Got info for canvas elements");
#endif
            encodeBounds = new(minX, minY, maxX - minX, maxY - minY);

            // Get the reader for the movie file
            IMovieReader movieReader = IMovieReader.ReturnReaderByExtention(moviePath);
#if DEBUG
            Console.WriteLine("Movie reader created");
#endif

            // Encoding settings
            var settings = new VideoEncoderSettings(width: maxX - minX, height: maxY - minY, codec: VideoCodec.H264)
            {
                EncoderPreset = EncoderPreset.Fast,
                CRF = 17,
                FramerateRational = (AVRational)frameRate
            };

#if DEBUG
            Console.WriteLine("Encoder settings created");
#endif
            // Loop over the input frames
            // TODO: Audio for combining with audiovideo elements
            Fraction secondsInCurrentFrame = 0;
            List<InputFrame> frameBuffer = new();

            // Set the ffmpeg path if not already set
            if (!FFmpegPathSet && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                FFmpegLoader.FFmpegPath = Path.GetFullPath("ffmpeg/x86_64");
                FFmpegPathSet = true;
            }

#if DEBUG
            Console.WriteLine("About to create video file");
#endif

            using (var file = MediaBuilder.CreateContainer(outputPath).WithVideo(settings).Create())
            {
#if DEBUG
                Console.WriteLine("Video File created");
#endif
                foreach (var inputFrame in movieReader)
                {
                    // Add to the buffer
                    frameBuffer.Add(inputFrame);
                    secondsInCurrentFrame += inputFrame.TimeDelta;

                    // If there is now more than the buffer requires, make the next frame
                    if (secondsInCurrentFrame >= requiredBufferSeconds && secondsInCurrentFrame >= ~frameRate)
                    {
                        // Add the frame
                        var outputFrame = GetEncodeFrame(frameBuffer);
                        var bitlock = outputFrame.LockBits(new Rectangle(System.Drawing.Point.Empty, outputFrame.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                        var bitmapData = ImageData.FromPointer(bitlock.Scan0, ImagePixelFormat.Bgr24, outputFrame.Size);

                        file.Video.AddFrame(bitmapData);

                        outputFrame.UnlockBits(bitlock);
                        outputFrame.Dispose();

                        // Dequeue the used frames, and reduce the duration of any incomplete one
                        Fraction timeToRemove = ~frameRate - frameBuffer[0].TimeDelta;
                        while (timeToRemove >= 0)
                        {
                            frameBuffer.RemoveAt(0);
                            if (frameBuffer.Count == 0)
                                break;
                            timeToRemove -= frameBuffer[0].TimeDelta;
                        }
                        if (frameBuffer.Count > 0)
                            frameBuffer[0] = new(frameBuffer[0].Inputs, -timeToRemove);
                        secondsInCurrentFrame -= ~frameRate;
                    }
                }
                // Add the stragler frames
                while (frameBuffer.Count > 0)
                {
                    // Add the frame
                    var outputFrame = GetEncodeFrame(frameBuffer);
                    var bitlock = outputFrame.LockBits(new Rectangle(System.Drawing.Point.Empty, outputFrame.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                    var bitmapData = ImageData.FromPointer(bitlock.Scan0, ImagePixelFormat.Bgr24, outputFrame.Size);

                    file.Video.AddFrame(bitmapData);

                    outputFrame.UnlockBits(bitlock);
                    outputFrame.Dispose();

                    // Dequeue the used frames, and reduce the duration of any incomplete one
                    Fraction timeToRemove = ~frameRate - frameBuffer[0].TimeDelta;
                    while (timeToRemove >= 0)
                    {
                        frameBuffer.RemoveAt(0);
                        if (frameBuffer.Count == 0)
                            break;
                        timeToRemove -= frameBuffer[0].TimeDelta;
                    }
                    if (frameBuffer.Count > 0)
                        frameBuffer[0] = new(frameBuffer[0].Inputs, -timeToRemove);
                }

                file.Dispose();
            }

            // Tell the elements that we are ending the encode
            foreach (var element in AllCanvasElements)
            {
                element.OnEndEncode();
            }
        }

        public Bitmap GetEncodeFrame(List<InputFrame> inputFrames)
        {
            // Create the bitmap
            Bitmap encodeFrame = new(encodeBounds.Size.Width, encodeBounds.Size.Height);

            // Create a graphics object from the bitmap to use drawing methods
            Graphics graphics = Graphics.FromImage(encodeFrame);

            // Fill in the background
            graphics.FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(backgroundColor.Color.A, backgroundColor.Color.R, backgroundColor.Color.G, backgroundColor.Color.B)),
                new Rectangle(new System.Drawing.Point(0, 0), encodeBounds.Size));

            // Add the canvas elements
            var sortedElements = new List<ICanvasElement>(AllCanvasElements);
            sortedElements.Sort(ICanvasElement.zComparator);
            foreach (var element in sortedElements)
            {
                graphics.DrawImage(element.GetEncodeFrame(inputFrames), new System.Drawing.Point((int)element.Bounds.Left - encodeBounds.Left, (int)element.Bounds.Top - encodeBounds.Top));
            }

            // Dispose the graphics
            graphics.Dispose();

            // Return the bitmap
            return encodeFrame;
        }
    }
}
