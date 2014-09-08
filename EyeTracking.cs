using Microsoft.Devices;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPEyeTracking.Analysers;

namespace WPEyeTracking
{
    public class EyeTracking : IDisposable
    {
        public delegate void AnalysisComplete(EyeTracking sender, List<FaceEyesInfo> facesInfo);
        public event AnalysisComplete OnResult;

        internal ManualResetEvent doFrameAnalysis = new ManualResetEvent(true);
        public int Width;
        public int Height;
        private Analyser analyser;
        internal Dispatcher Dispatcher;

        public WriteableBitmap wb0 = new WriteableBitmap(640, 480);
        public WriteableBitmap wb1 = new WriteableBitmap(128, 128);

        private EyeTracking(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public EyeTracking(Dispatcher Dispatcher, PhotoCamera camera, int width, int height)
            : this(width, height)
        {
            this.Dispatcher = Dispatcher;
            this.analyser = new CameraAnalyser(this, camera);
        }

        public EyeTracking(int[] pixels, int width, int height)
            : this(width, height)
        {
            this.analyser = new PixelMatrixAnalyser(this);
        }

        public void StartAnalyse()
        {
            this.analyser.Analyse();
        }

        public void NotifyResultComplete()
        {
            doFrameAnalysis.Set();
        }

        public void Dispose()
        {
            this.analyser.Dispose();
            doFrameAnalysis.Set();
        }

        internal void AnalyserDone(List<FaceEyesInfo> facesInfo)
        {
            if (OnResult != null)
            {
                OnResult(this, facesInfo);
            }
        }

    }
}
