using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;
using System.Collections.Generic;
using TASBoard.MovieReaders;

namespace TASBoard.Models
{
    public class KeyObject : ReactiveObject, ICanvasElement
    {
        private Bitmap keyDownImage;
        private Bitmap keyUpImage;
        private Bitmap currentImage;
        private System.Drawing.Bitmap keyDownBitmap;
        private System.Drawing.Bitmap keyUpBitmap;
        private string keyName;
        private string keyStyle;
        private Fraction encodeRate;

        public Fraction SecondsAheadNeeded { get; } = 0;

        private int _x, _y, _zIndex;

        public int X
        {
            get => _x;
            set => this.RaiseAndSetIfChanged(ref _x, value);
        }
        public int Y
        {
            get => _y;
            set => this.RaiseAndSetIfChanged(ref _y, value);
        }
        public int zIndex
        {
            get => _zIndex;
            set => this.RaiseAndSetIfChanged(ref _zIndex, value);
        }

        public Rect Bounds { get; set; }


        public KeyObject(string keyStyle, string keyName, int x, int y, int zInd, bool displayDown) 
        {
            keyDownImage = new("Assets/KeySprites/" + keyStyle + "/" + keyName + "_down.png");
            keyUpImage = new("Assets/KeySprites/" + keyStyle + "/" + keyName + "_up.png");
            currentImage = displayDown ? keyDownImage : keyUpImage;
            _x = x;
            _y = y;
            _zIndex = zInd;
            this.keyName = keyName;
            this.keyStyle = keyStyle;
        }

        public void SetKeyDown(bool displayDown)
        {
            Image = displayDown ? keyDownImage : keyUpImage;
        }

        public KeyObject(string keyStyle, string keyName, bool displayDown) : this(keyStyle, keyName, 0, 0, 0, displayDown) { }

        public Bitmap Image { get => currentImage; set => this.RaiseAndSetIfChanged(ref currentImage, value); }

        public void OnBeginEncode(Fraction framerate)
        {
            encodeRate = framerate;
            keyDownBitmap = new("Assets/KeySprites/" + keyStyle + "/" + keyName + "_down.png");
            keyUpBitmap = new("Assets/KeySprites/" + keyStyle + "/" + keyName + "_up.png");
        }

        public void OnEndEncode()
        {
            // Don't want to have these just sitting in memory
            keyDownBitmap.Dispose();
            keyUpBitmap.Dispose();
        }

        public System.Drawing.Bitmap GetEncodeFrame(List<InputFrame> inputFrames)
        {
            InputFrame relevantFrame = inputFrames[0];
            int i = 1;
            while (relevantFrame.TimeDelta < ~encodeRate)
            {
                relevantFrame += inputFrames[i];
                i++;
            }

            if (inputFrames.Sum().Inputs.Contains(keyName))
            {
                return keyDownBitmap;
            }
            return keyUpBitmap;
        }
    }
}
