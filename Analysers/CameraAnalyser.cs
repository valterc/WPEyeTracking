using Microsoft.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WPEyeTracking.Analysers
{
    internal class CameraAnalyser : Analyser
    {
        private PhotoCamera camera;
        private Thread workerThread;
        private Boolean exitThread;
        private PixelMatrixAnalyser pixelMatrixAnalyser;
        private int[] cameraBuffer;
        private int[] pixelBuffer;
        private ManualResetEvent resizeEvent;

        public CameraAnalyser(EyeTracking eyeTracking, PhotoCamera camera)
            : base(eyeTracking)
        {
            this.camera = camera;
            this.workerThread = new Thread(Analysis);
            this.pixelMatrixAnalyser = new PixelMatrixAnalyser(eyeTracking);
            this.cameraBuffer = new int[(int)camera.PreviewResolution.Width * (int)camera.PreviewResolution.Height];
            this.pixelBuffer = new int[eyeTracking.Width * eyeTracking.Height];
            this.resizeEvent = new ManualResetEvent(false);
        }



        private void Analysis()
        {
            Thread.Sleep(100);
            while (!exitThread)
            {
                EyeTracking.doFrameAnalysis.WaitOne();
                EyeTracking.doFrameAnalysis.Reset();

                if (exitThread)
                {
                    break;
                }

                camera.GetPreviewBufferArgb32(cameraBuffer);

                ResizePixelMatrix();

                pixelMatrixAnalyser.RegisterFrame(pixelBuffer);
                pixelMatrixAnalyser.Analyse();
            }

        }

        public static void CompensateForRender(int[] bitmapPixels)
        {
            int count = bitmapPixels.Length;

            for (int i = 0; i < count; i++)
            {
                uint pixel = unchecked((uint)bitmapPixels[i]);
                // decompose
                double a = (pixel >> 24) & 255;
                if ((a == 255) || (a == 0)) continue;

                double r = (pixel >> 16) & 255;
                double g = (pixel >> 8) & 255;
                double b = (pixel) & 255;

                double factor = 255 / a;
                uint newR = (uint)Math.Round(r * factor);
                uint newG = (uint)Math.Round(g * factor);
                uint newB = (uint)Math.Round(b * factor);
                // compose
                bitmapPixels[i] = unchecked((int)((pixel & 0xFF000000) | (newR << 16) | (newG << 8) | newB));
            }
        }

        private void ResizePixelMatrix()
        {
            if ((int)camera.PreviewResolution.Width != EyeTracking.Width || (int)camera.PreviewResolution.Height != EyeTracking.Height)
            {
                DispatcherOperation d = EyeTracking.Dispatcher.BeginInvoke(() =>
                {
                    //CompensateForRender(cameraBuffer);
                    cameraBuffer.CopyTo(EyeTracking.wb0.Pixels, 0);
                    EyeTracking.wb0.Invalidate();


                    WriteableBitmap wb = new WriteableBitmap((int)camera.PreviewResolution.Width, (int)camera.PreviewResolution.Height);
                    Array.Copy(cameraBuffer, wb.Pixels, cameraBuffer.Length);


                    MemoryStream ms = new MemoryStream();
                    wb.SaveJpeg(ms, EyeTracking.Width, EyeTracking.Height, 0, 100);
                    ms.Seek(0, SeekOrigin.Begin);

                    WriteableBitmap newWb = new WriteableBitmap(EyeTracking.Width, EyeTracking.Height);
                    newWb.LoadJpeg(ms);

                    pixelBuffer = newWb.Pixels;

                    //CompensateForRender(pixelBuffer);

                    pixelBuffer.CopyTo(EyeTracking.wb1.Pixels, 0);
                    EyeTracking.wb1.Invalidate();

                    ms.Dispose();
                    this.resizeEvent.Reset();
                    this.resizeEvent.Set();
                });

                this.resizeEvent.WaitOne();

            }
        }

        internal override void Analyse()
        {
            workerThread.Start();
        }

        internal override void Dispose()
        {
            exitThread = true;
            workerThread = null;
            this.pixelMatrixAnalyser.Dispose();
        }

    }
}
