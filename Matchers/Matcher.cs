using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPEyeTracking.Matchers
{
    internal abstract class Matcher<T>
    {
        internal abstract List<FaceEyesInfo> Match(T t);
    }
}
