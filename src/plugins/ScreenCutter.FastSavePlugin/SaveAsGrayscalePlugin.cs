using System.Drawing;
using ScreenCutter.PluginContract;

namespace ScreenCutter.FastSavePlugin
{
    public class SaveAsGrayscalePlugin : FastSavePlugin
    {
        private IImageFilter grayscaleFilter = new GrayscaleFilter();

        private IImageFilter removeBordersFilter = new RemoveBordersFilter();

        private IImageCleaver singleColorVerticalCleaver = new SingleColorVerticalCleaver();

        public override void Save(Bitmap screenArea)
        {
            var greyscale = this.grayscaleFilter.Filter(screenArea);

            var parts = this.singleColorVerticalCleaver.Cleave(greyscale);

            foreach (var part in parts)
            {
                var borderless = this.removeBordersFilter.Filter(part);

                base.Save(borderless);
            }
        }
    }
}
