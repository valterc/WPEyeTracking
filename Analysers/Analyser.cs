using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPEyeTracking.Analysers
{
    internal abstract class Analyser
    {
        protected EyeTracking EyeTracking;

        public Analyser(EyeTracking eyeTracking)
        {
            this.EyeTracking = eyeTracking;
        }

        protected void AnalysisComplete(List<FaceEyesInfo> facesInfo)
        {
            EyeTracking.AnalyserDone(facesInfo);
        }

        internal abstract void Analyse();

        internal virtual void Dispose()
        {
        }

    }
}
