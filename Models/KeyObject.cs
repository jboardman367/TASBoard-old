using Avalonia.Media.Imaging;

namespace TASBoard.Models
{
    public class KeyObject
    {

        public static bool DisplayKeyDown { get; set; } = false;

        private Bitmap keyDownImage;
        private Bitmap keyUpImage;

        public int X, Y;


        public KeyObject(string keyStyle, string keyName) 
        {
            keyDownImage = new("Assets\\KeySprites\\" + keyStyle + "\\" + keyName + "Down.png");
            keyUpImage = new("Assets\\KeySprites\\" + keyStyle + "\\" + keyName + "Up.png");
        }

        public Bitmap Image => DisplayKeyDown ? keyDownImage : keyUpImage;
    }
}
