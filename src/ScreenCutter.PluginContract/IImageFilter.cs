using System.Drawing;

namespace ScreenCutter.PluginContract
{
    public interface IImageFilter
    {
        Bitmap Filter(Bitmap img);
    }
}
