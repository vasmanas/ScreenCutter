using System.Collections.Generic;
using System.Drawing;

namespace ScreenCutter.PluginContract
{
    public interface IImageCleaver
    {
        IEnumerable<Bitmap> Cleave(Bitmap img);
    }
}
