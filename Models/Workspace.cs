using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASBoard.MovieReaders;
using System.Drawing.Imaging;
using System.Drawing;

namespace TASBoard.Models
{
    public class Workspace
    {
        public ObservableCollection<ICanvasElement> AllCanvasElements;
        public Workspace()
        {
            AllCanvasElements = new();
        }

        public Avalonia.Media.Brush backgroundColor = new SolidColorBrush(new Avalonia.Media.Color(255, 112, 112, 112));

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
        private Rect encodeBounds;

        public void Encode(string moviePath, string outputPath, Fraction frameRate)
        {
            // Find the bounds of the area that will be captured
            uint minX = uint.MaxValue, minY = uint.MaxValue, maxX = 0, maxY = 0;

            foreach (var element in AllCanvasElements)
            {
                if (element == null) { continue; }
                minX = Math.Min(minX, (uint)element.Bounds.Left);
                minY = Math.Min(minY, (uint)element.Bounds.Top);
                maxX = Math.Max(maxX, (uint)element.Bounds.Right);
                maxY = Math.Max(maxY, (uint)element.Bounds.Bottom);

                // Since we are looping over them anyway, tell them to prepare to encode
                element.OnBeginEncode();
            }

            // Get the reader for the movie file
            IMovieReader movieReader = IMovieReader.ReturnReaderByExtention(moviePath);
            

            // Tell the elements that we are ending the encode
            foreach (var element in AllCanvasElements)
            {
                element.OnEndEncode();
            }
        }

        public System.Drawing.Bitmap GetEncodeFrame(InputFrame[] inputFrames)
        {
            System.Drawing.Bitmap encodeFrame = new((int)encodeBounds.Size.Width, (int)encodeBounds.Size.Height);
        }
    }
}
