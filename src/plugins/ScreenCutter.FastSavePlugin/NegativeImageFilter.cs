using System.Drawing;
using ScreenCutter.PluginContract;

namespace ScreenCutter.FastSavePlugin
{
    public class NegativeImageFilter : IImageFilter
    {
        public Bitmap Filter(Bitmap img)
        {
            var w = img.Width;
            var h = img.Height;
            var clone = new Bitmap(w, h, img.PixelFormat);

            for (var i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    var colorToConvert = img.GetPixel(i, j);

                    var invertedColor = Color.FromArgb((byte)~colorToConvert.R, (byte)~colorToConvert.G, (byte)~colorToConvert.B);

                    clone.SetPixel(i, j, invertedColor);
                }
            }

            return clone;
        }
    }
}
