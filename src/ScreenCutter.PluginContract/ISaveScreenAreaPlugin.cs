using System.Drawing;

namespace ScreenCutter.PluginContract
{
    public interface ISaveScreenAreaPlugin : IPlugin
    {
        void Save(Bitmap screenArea);
    }
}
