using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASBoard.Models
{
    public static class ImageExtentions
    {       
        public static Avalonia.Media.Imaging.Bitmap? ConvertSystemToAvaloniaBitmap(System.Drawing.Bitmap? bitmap)
        {
            // Based on code here https://github.com/AvaloniaUI/Avalonia/discussions/5908
            if (bitmap == null)
                return null;

            // Lock the bits of a temp copy
            System.Drawing.Bitmap tmp = new(bitmap);
            var bitmapdata = tmp.LockBits(new System.Drawing.Rectangle(0, 0, tmp.Width, tmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Read the locked bits into a new bitmap
            Avalonia.Media.Imaging.Bitmap bitmap1 = new(Avalonia.Platform.PixelFormat.Bgra8888,
                Avalonia.Platform.AlphaFormat.Premul,
                bitmapdata.Scan0,
                new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
                new Avalonia.Vector(96, 96),
                bitmapdata.Stride);

            // Release and dispose the temp
            tmp.UnlockBits(bitmapdata);
            tmp.Dispose();

            // Return the new bitmap
            return bitmap1;
        }
    }
}
