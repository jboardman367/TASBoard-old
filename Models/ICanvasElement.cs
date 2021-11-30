using Avalonia;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASBoard.MovieReaders;

namespace TASBoard.Models
{
    // Should this implement IDisposable?
    public interface ICanvasElement
    {
        public Bitmap Image { get; }

        public Rect Bounds { get; set; }

        public int zIndex { get; set; }

        public void SetKeyDown(bool value);

        public int FramesAheadNeeded { get; }

        public void OnBeginEncode();

        public void OnEndEncode();

        public System.Drawing.Bitmap GetEncodeFrame(InputFrame[] inputFrames);
    }
}
