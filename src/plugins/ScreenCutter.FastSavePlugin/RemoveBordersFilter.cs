using System;
using System.Drawing;
using System.Drawing.Imaging;
using ScreenCutter.PluginContract;

namespace ScreenCutter.FastSavePlugin
{
    /// <summary>
    /// Removes borders if they are of same color.
    /// </summary>
    public class RemoveBordersFilter : IImageFilter
    {
        public Bitmap Filter(Bitmap img)
        {
            int w = img.Width,
                h = img.Height,
                bmpStride, bytesPerPixel;
            BitmapData bmpData;

            var pfIn = img.PixelFormat;
            
            //Get the number of bytes per pixel
            switch (pfIn)
            {
                case PixelFormat.Format8bppIndexed: bytesPerPixel = 1; break;
                case PixelFormat.Format24bppRgb: bytesPerPixel = 3; break;
                case PixelFormat.Format32bppArgb: bytesPerPixel = 4; break;
                case PixelFormat.Format32bppRgb: bytesPerPixel = 4; break;
                default: throw new InvalidOperationException(string.Format("Image format {0} not supported", pfIn));
            }

            var outputY = 0;
            var outputX = 0;
            var outputHeight = h;
            var outputWith = w;

            var output = (Bitmap)img.Clone();

            byte[] borderColor = null;
            for (var i = 0; i < 4; i++)
            {
                bmpData = output.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, pfIn);
                bmpStride = bmpData.Stride;

                //Traverse each pixel of the image
                unsafe
                {
                    byte* bmpPtr = (byte*)bmpData.Scan0.ToPointer();

                    // One time initialization
                    if (borderColor == null)
                    {
                        borderColor = new byte[bytesPerPixel];
                        for (var c = 0; c < bytesPerPixel; c++)
                        {
                            borderColor[c] = bmpPtr[c];
                        }
                    }

                    // top lines
                    int il, ic;
                    var exit = false;
                    for (var l = 0; l < h && !exit; l++)
                    {
                        for (il = ic = 0; il < w * bytesPerPixel; ic = (ic + 1) % bytesPerPixel, ++il)
                        {
                            if (borderColor[ic] != bmpPtr[l * bmpStride + il])
                            {
                                exit = true;

                                break;
                            }
                        }

                        if (exit)
                        {
                            break;
                        }

                        switch (i)
                        {
                            case 0: // Top
                                {
                                    outputY++;
                                    outputHeight--;
                                    break;
                                }

                            case 1: // Left 
                                {
                                    outputX++;
                                    outputWith--;
                                    break;
                                }

                            case 2: // Bottom
                                {
                                    outputHeight--;
                                    break;
                                }

                            case 3: // Right
                                {
                                    outputWith--;
                                    break;
                                }
                        }
                    }
                }

                /// Unlock the images
                output.UnlockBits(bmpData);
                
                /// All lines are of same color, so just return original image
                if (i == 0 && outputY == h)
                {
                    return output;
                }

                output.RotateFlip(RotateFlipType.Rotate90FlipNone);

                w = output.Width;
                h = output.Height;
            }

            // Crop
            output = this.CropImage(output, new Rectangle(outputX, outputY, outputWith, outputHeight));

            return output;
        }
        
        private Bitmap CropImage(Bitmap img, Rectangle cropArea)
        {
            return img.Clone(cropArea, img.PixelFormat);
        }
    }
}
