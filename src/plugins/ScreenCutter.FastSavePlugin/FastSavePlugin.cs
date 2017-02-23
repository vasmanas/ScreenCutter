using System;
using System.Drawing;
using System.IO;
using ScreenCutter.PluginContract;

namespace ScreenCutter.FastSavePlugin
{
    public class FastSavePlugin : ISaveScreenAreaPlugin
    {
        public FastSavePlugin()
        {
            this.DirPath = string.Format("{0}\\images\\", Environment.CurrentDirectory);
        }

        public string DirPath { get; set; }

        public void Save(Bitmap screenArea)
        {
            if (!Directory.Exists(this.DirPath))
            {
                Directory.CreateDirectory(this.DirPath);
            }

            screenArea.Save(string.Format("{0}\\{1}.bmp", this.DirPath, Guid.NewGuid()));
        }
    }
}
