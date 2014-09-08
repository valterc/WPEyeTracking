using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPEyeTracking.Matchers;
using WPEyeTracking.Tools;

namespace WPEyeTracking.Analysers
{
    internal class PixelMatrixAnalyser : Analyser
    {
        private int[] pixels;
        private PixelBlobFinder blobFinder;
        private BlobMatcher blobMatcher;

        public PixelMatrixAnalyser(EyeTracking eyeTracking)
            : base(eyeTracking)
        {
            this.blobFinder = new PixelBlobFinder(null, eyeTracking.Width, -1, 15);
            this.blobMatcher = new BlobMatcher();
        }


        public void RegisterFrame(int[] pixels)
        {
            this.pixels = pixels;
        }

        internal List<FaceEyesInfo> InternalAnalyse()
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = PixelFilters.ColorToGray(PixelFilters.InvertColor(pixels[i]));
            }

            EyeTracking.Dispatcher.BeginInvoke(() =>
                {
                    pixels.CopyTo(EyeTracking.wb1.Pixels, 0);
                    EyeTracking.wb1.Invalidate();
                });

            List<PixelBlob> blobs = blobFinder.FindBlobsInPixels(pixels);

            List<FaceEyesInfo> facesInfo = blobMatcher.Match(blobs);
            return facesInfo;
        }

        internal override void Analyse()
        {
            AnalysisComplete(InternalAnalyse());
        }

    }
}
