using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPEyeTracking.Tools
{
    internal class PixelFilters
    {

        internal static int ColorBinaryFilter(int color, int threshold)
        {
            int a = color >> 24;
            int r = (color & 0x00ff0000) >> 16;
            int g = (color & 0x0000ff00) >> 8;
            int b = (color & 0x000000ff);

            if (r < threshold || g < threshold || b < threshold)
            {
                r = g = b = 0;
            }
            else
            {
                r = g = b = 255;
            }

            return ((a & 0xFF) << 24) | ((r & 0xFF) << 16) | ((g & 0xFF) << 8) | (b & 0xFF);
        }

        internal static int InvertColor(int color)
        {
            int c = 0;

            int a = color >> 24;
            int r = (color & 0x00ff0000) >> 16;
            int g = (color & 0x0000ff00) >> 8;
            int b = (color & 0x000000ff);


            r = 255 - r;
            g = 255 - g;
            b = 255 - b;

            c = ((a & 0xFF) << 24) | ((r & 0xFF) << 16) | ((g & 0xFF) << 8) | (b & 0xFF);

            return c;
        }

        internal static int ColorToGray(int color)
        {
            int gray = 0;

            int a = color >> 24;
            int r = (color & 0x00ff0000) >> 16;
            int g = (color & 0x0000ff00) >> 8;
            int b = (color & 0x000000ff);

            if ((r == g) && (g == b))
            {
                gray = color;
            }
            else
            {
                // Calculate for the illumination.
                // I =(int)(0.109375*R + 0.59375*G + 0.296875*B + 0.5)
                int i = (7 * r + 38 * g + 19 * b + 32) >> 6;

                gray = ((a & 0xFF) << 24) | ((i & 0xFF) << 16) | ((i & 0xFF) << 8) | (i & 0xFF);
            }
            return gray;
        }

    }
}
