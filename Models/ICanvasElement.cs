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
    // Probably need to dispose when we get up to having them deletable.
    public interface ICanvasElement
    {
        public Bitmap Image { get; }

        public Rect Bounds { get; set; }

        public int zIndex { get; set; }

        public void SetKeyDown(bool value);

        // NB: You are not guaranteed to get this many seconds! The actual values may be either lower or higher
        public Fraction SecondsAheadNeeded { get; }

        public void OnBeginEncode();

        public void OnEndEncode();

        public System.Drawing.Bitmap GetEncodeFrame(List<InputFrame> inputFrames);

        public static Comparison<ICanvasElement> zComparator = new((ICanvasElement c1, ICanvasElement c2) => c1.zIndex - c2.zIndex);
    }
}
