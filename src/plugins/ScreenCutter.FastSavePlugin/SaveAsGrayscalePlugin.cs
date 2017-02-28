using System.Drawing;
using ScreenCutter.PluginContract;

namespace ScreenCutter.FastSavePlugin
{
    public class SaveAsGrayscalePlugin : FastSavePlugin
    {
        private IImageFilter grayscaleFilter = new GrayscaleFilter();

        private IImageFilter removeBordersFilter = new RemoveBordersFilter();

        public override void Save(Bitmap screenArea)
        {
            var greyscale = this.grayscaleFilter.Filter(screenArea);

            var borderless = this.removeBordersFilter.Filter(greyscale);

            base.Save(borderless);
        }
    }
}
