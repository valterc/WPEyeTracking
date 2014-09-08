using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPEyeTracking.Tools
{
    internal class PixelBlobFinder
    {
        private static int[] colors = {GetArgbValues(Colors.Blue), 
                               GetArgbValues(Colors.Green), 
                               GetArgbValues(Colors.Red), 
                               GetArgbValues(Colors.Yellow), 
                               GetArgbValues(Colors.Purple), 
                               GetArgbValues(Colors.Orange), 
                               GetArgbValues(Colors.Brown), 
                               GetArgbValues(Colors.Gray),  
                               GetArgbValues(Colors.Cyan), 
                               GetArgbValues(Colors.DarkGray), 
                               GetArgbValues(Colors.LightGray), 
                               GetArgbValues(Colors.Magenta), 
                               GetArgbValues(Colors.White), 
                           };

        private int[] pixels;
        private int width;
        private int color;
        private int distance;


        public PixelBlobFinder(int[] pixels, int width, int color, int distance)
        {
            this.pixels = pixels;
            this.width = width;
            this.color = color;
            this.distance = distance;
        }

        public List<PixelBlob> FindBlobsInPixels(int[] pixels)
        {

            List<PixelBlob> blobs = new List<PixelBlob>();

            for (int i = 0; i < pixels.Length; i++)
            {
                int currentPixel = pixels[i];
                int x = i % width;
                int y = i / width;

                if (currentPixel == color)
                {
                    //look for nearby blob
                    double min = double.MaxValue;
                    PixelBlob b = null;

                    if (blobs.Count > 0)
                    {

                        foreach (var item in blobs)
                        {
                            foreach (var px in item.Pixels)
                            {
                                double dist = DistanceBetweenPoints(px.X, px.Y, x, y);
                                if (dist < distance && dist < min)
                                {
                                    //TODO: Optimize Blob search
                                    b = item;
                                    break;
                                }
                            }
                        }

                    }

                    //Found nearby blob
                    if (b != null)
                    {
                        b.Count++;

                        if (x < b.MinX)
                        {
                            b.MinX = x;
                        }

                        if (y < b.MinY)
                        {
                            b.MinY = y;
                        }

                        if (x > b.MaxX)
                        {
                            b.MaxX = x;
                        }

                        if (y > b.MaxY)
                        {
                            b.MaxY = y;
                        }

                        pixels[i] = b.RenderColor;
                        b.Pixels.Add(new Pixel { X = x, Y = y, Color = currentPixel });

                    }
                    else
                    {
                        //Create new blob
                        blobs.Add(new PixelBlob { Id = blobs.Count, Count = 1, MaxX = x, MinX = x, MaxY = y, MinY = y, PixelColor = color, Pixels = new List<Pixel> { new Pixel { X = x, Y = y, Color = currentPixel } }, RenderColor = GetNextRenderColor(blobs.Count) });
                        pixels[i] = blobs[blobs.Count - 1].RenderColor;
                    }
                }
            }

            foreach (var item in blobs)
            {
                item.CalculateCenter();
            }

            return blobs;
        }

        public List<PixelBlob> FindBlobsInPixels()
        {

            List<PixelBlob> blobs = new List<PixelBlob>();

            for (int i = 0; i < pixels.Length; i++)
            {
                int currentPixel = pixels[i];
                int x = i % width;
                int y = i / width;

                if (currentPixel == color)
                {
                    //look for nearby blob
                    double min = double.MaxValue;
                    PixelBlob b = null;

                    if (blobs.Count > 0)
                    {

                        foreach (var item in blobs)
                        {
                            foreach (var px in item.Pixels)
                            {
                                double dist = DistanceBetweenPoints(px.X, px.Y, x, y);
                                if (dist < distance && dist < min)
                                {
                                    b = item;
                                }
                            }
                        }

                    }

                    //Found nearby blob
                    if (b != null)
                    {
                        b.Count++;

                        if (x < b.MinX)
                        {
                            b.MinX = x;
                        }

                        if (y < b.MinY)
                        {
                            b.MinY = y;
                        }

                        if (x > b.MaxX)
                        {
                            b.MaxX = x;
                        }

                        if (y > b.MaxY)
                        {
                            b.MaxY = y;
                        }

                        pixels[i] = b.RenderColor;
                        b.Pixels.Add(new Pixel { X = x, Y = y, Color = currentPixel });

                    }
                    else
                    {
                        //Create new blob
                        blobs.Add(new PixelBlob { Id = blobs.Count, Count = 1, MaxX = x, MinX = x, MaxY = y, MinY = y, PixelColor = color, Pixels = new List<Pixel> { new Pixel { X = x, Y = y, Color = currentPixel } }, RenderColor = GetNextRenderColor(blobs.Count) });
                        pixels[i] = blobs[blobs.Count - 1].RenderColor;
                    }
                }
            }


            foreach (var item in blobs)
            {
                item.CalculateCenter();
            }

            return blobs;
        }


        private int GetNextRenderColor(int count)
        {
            return colors[count % colors.Length];
        }

        private double DistanceBetweenPoints(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        private Color ColorFromIntArgb(int c)
        {
            int a = c >> 24;
            int r = (c & 0x00ff0000) >> 16;
            int g = (c & 0x0000ff00) >> 8;
            int b = (c & 0x000000ff);

            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }


        private static int GetArgbValues(Color c)
        {
            return ((c.A & 0xFF) << 24) | ((c.R & 0xFF) << 16) | ((c.G & 0xFF) << 8) | (c.B & 0xFF);
        }

    }
}
