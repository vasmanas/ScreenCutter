﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using ScreenCutter.PluginContract;

namespace ScreenCutter.FastSavePlugin
{
    /// <summary>
    /// Converts a bitmap into an 8-bit grayscale bitmap
    /// </summary>
    public class GrayscaleFilter : IImageFilter
    {
        public Bitmap Filter(Bitmap img)
        {
            int w = img.Width,
                h = img.Height,
                r, ic, oc, bmpStride, outputStride, bytesPerPixel;
            var pfIn = img.PixelFormat;
            ColorPalette palette;
            Bitmap output;
            BitmapData bmpData, outputData;

            //Create the new bitmap
            output = new Bitmap(w, h, PixelFormat.Format8bppIndexed);

            //Build a grayscale color Palette
            palette = output.Palette;
            for (int i = 0; i < 256; i++)
            {
                Color tmp = Color.FromArgb(255, i, i, i);
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            output.Palette = palette;

            //No need to convert formats if already in 8 bit
            if (pfIn == PixelFormat.Format8bppIndexed)
            {
                output = (Bitmap)img.Clone();

                //Make sure the palette is a grayscale palette and not some other
                //8-bit indexed palette
                output.Palette = palette;

                return output;
            }

            //Get the number of bytes per pixel
            switch (pfIn)
            {
                case PixelFormat.Format24bppRgb: bytesPerPixel = 3; break;
                case PixelFormat.Format32bppArgb: bytesPerPixel = 4; break;
                case PixelFormat.Format32bppRgb: bytesPerPixel = 4; break;
                default: throw new InvalidOperationException(string.Format("Image format {0} not supported", pfIn));
            }

            //Lock the images
            bmpData = img.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, pfIn);
            bmpStride = bmpData.Stride;
            outputData = output.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            outputStride = outputData.Stride;

            //Traverse each pixel of the image
            unsafe
            {
                byte* bmpPtr = (byte*)bmpData.Scan0.ToPointer(),
                outputPtr = (byte*)outputData.Scan0.ToPointer();

                if (bytesPerPixel == 3)
                {
                    //Convert the pixel to it's luminance using the formula:
                    // L = .299*R + .587*G + .114*B
                    //Note that ic is the input column and oc is the output column
                    for (r = 0; r < h; r++)
                        for (ic = oc = 0; oc < w; ic += 3, ++oc)
                            outputPtr[r * outputStride + oc] = (byte)(int)
                                (0.299f * bmpPtr[r * bmpStride + ic] +
                                 0.587f * bmpPtr[r * bmpStride + ic + 1] +
                                 0.114f * bmpPtr[r * bmpStride + ic + 2]);
                }
                else //bytesPerPixel == 4
                {
                    //Convert the pixel to it's luminance using the formula:
                    // L = alpha * (.299*R + .587*G + .114*B)
                    //Note that ic is the input column and oc is the output column
                    for (r = 0; r < h; r++)
                        for (ic = oc = 0; oc < w; ic += 4, ++oc)
                            outputPtr[r * outputStride + oc] = (byte)(int)
                                ((bmpPtr[r * bmpStride + ic] / 255.0f) *
                                (0.299f * bmpPtr[r * bmpStride + ic + 1] +
                                 0.587f * bmpPtr[r * bmpStride + ic + 2] +
                                 0.114f * bmpPtr[r * bmpStride + ic + 3]));
                }
            }

            //Unlock the images
            img.UnlockBits(bmpData);
            output.UnlockBits(outputData);

            return output;
        }
    }
}
