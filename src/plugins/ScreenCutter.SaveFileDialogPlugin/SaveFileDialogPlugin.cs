using System.Drawing;
using ScreenCutter.PluginContract;

namespace ScreenCutter.SaveFileDialogPlugin
{
    public class SaveFileDialogPlugin : ISaveScreenAreaPlugin
    {
        public SaveFileDialogPlugin()
        {
            this.DefaultExt = ".bmp";
            this.FileFilter = "BMP Files (*.bmp)|*.bmp|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
        }

        public string DefaultExt { get; set; }

        public string FileFilter { get; set; }

        public void Save(Bitmap screenArea)
        {
            var fileDialog = new Microsoft.Win32.SaveFileDialog();

            fileDialog.DefaultExt = this.DefaultExt;
            fileDialog.Filter = this.FileFilter;

            if (fileDialog.ShowDialog() == true)
            {
                screenArea.Save(fileDialog.FileName);
            }
        }
    }
}
