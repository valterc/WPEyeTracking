using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPEyeTracking.Tools
{
    public class PixelBlob
    {
        public int Id;

        public int RenderColor;
        public int PixelColor;

        public List<Pixel> Pixels;

        public int MinX;
        public int MinY;

        public int MaxX;
        public int MaxY;

        public int Count;

        public double CenterX;
        public double CenterY;

        public void CalculateCenter()
        {
            CenterX = ((double)MinX + MaxX) / 2;
            CenterY = ((double)MinY + MaxY) / 2;
        }

    }
}
