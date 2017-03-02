using System.Collections.Generic;
using System.Drawing;
using ScreenCutter.PluginContract;

namespace ScreenCutter.FastSavePlugin
{
    public class SingleColorVerticalCleaver : IImageCleaver
    {
        public IEnumerable<Bitmap> Cleave(Bitmap img)
        {
            var w = img.Width;
            var h = img.Height;

            var results = new List<Bitmap>();
            var cleaveStart = -1;
            var colorForSplitting = img.GetPixel(0, 0);

            for (var i = 0; i < w; i++)
            {
                var cleaveLine = true;

                for (var j = 0; j < h; j++)
                {
                    var color = img.GetPixel(i, j);
                    if (color != colorForSplitting)
                    {
                        cleaveLine = false;
                        break;
                    }
                }

                if (!cleaveLine && i + 1 < w)
                {
                    continue;
                }

                /// If previous line vas a cleav line, and this one is too
                /// we just move to the one that is further.
                if (i == cleaveStart + 1)
                {
                    cleaveStart++;
                }
                else
                {
                    var clone = img.Clone(new Rectangle(cleaveStart + 1, 0, i - cleaveStart - 1, h), img.PixelFormat);

                    results.Add(clone);

                    cleaveStart = i;
                }
            }

            if (results.Count == 0)
            {
                results.Add((Bitmap)img.Clone());
            }

            return results;
        }
    }
}
