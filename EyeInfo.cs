using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPEyeTracking
{
    public class EyeInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Boolean Closed { get; set; }
        public Side Side { get; set; }
    }
}
