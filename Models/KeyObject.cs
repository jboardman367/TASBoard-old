using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace TASBoard.Models
{
    public class KeyObject : ReactiveObject
    {

        public static bool DisplayKeyDown { get; set; } = false;

        private Bitmap keyDownImage;
        private Bitmap keyUpImage;

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


        public KeyObject(string keyStyle, string keyName, int x, int y, int zInd) 
        {
            keyDownImage = new("Assets\\KeySprites\\" + keyStyle + "\\" + keyName + "_down.png");
            keyUpImage = new("Assets\\KeySprites\\" + keyStyle + "\\" + keyName + "_up.png");
            _x = x;
            _y = y;
            _zIndex = zInd;
        }
        public KeyObject(string keyStyle, string keyName) : this(keyStyle, keyName, 0, 0, 0) { }

        public Bitmap Image => DisplayKeyDown ? keyDownImage : keyUpImage;
    }
}
