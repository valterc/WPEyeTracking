using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPEyeTracking.Tools;

namespace WPEyeTracking.Matchers
{
    internal class BlobMatcher : Matcher<List<PixelBlob>>
    {

        internal override List<FaceEyesInfo> Match(List<PixelBlob> blobs)
        {
            List<Tuple<PixelBlob, PixelBlob>> matches = new List<Tuple<PixelBlob, PixelBlob>>();

            foreach (var item in blobs)
            {
                foreach (var item2 in blobs)
                {
                    if (item != item2)
                    {
                        if (Math.Abs(item.CenterY - item2.CenterY) < 10 && item.Count > 100)
                        {
                            matches.Add(new Tuple<PixelBlob, PixelBlob>(item, item2));
                        }
                    }
                }
            }

            List<PixelBlob> eyes = new List<PixelBlob>();

            foreach (var item in matches)
            {
                if (Math.Abs(item.Item1.MaxX - item.Item1.MinX) < 2.5 * Math.Abs(item.Item1.MaxY - item.Item1.MinY))
                {
                    if (Math.Abs(item.Item1.MaxY - item.Item1.MinY) < 2.5 * Math.Abs(item.Item1.MaxX - item.Item1.MinX))
                    {
                        eyes.Add(item.Item1);
                        eyes.Add(item.Item2);
                    }
                }
            }

            List<FaceEyesInfo> facesInfo = new List<FaceEyesInfo>();

            while (eyes.Count > 1)
            {
                PixelBlob eye1 = eyes[0];
                PixelBlob eye2 = eyes[1];

                EyeInfo leftEye = null;
                EyeInfo rightEye = null;

                if (eye1.CenterX < eye2.CenterX)
                {
                    leftEye = new EyeInfo { Side = Side.Left, X = (int)eye1.CenterX, Y = (int)eye1.CenterY, Width = eye1.MaxX - eye1.MinX, Height = eye1.MaxY - eye1.MinY };
                    rightEye = new EyeInfo { Side = Side.Right, X = (int)eye2.CenterX, Y = (int)eye2.CenterY, Width = eye2.MaxX - eye2.MinX, Height = eye2.MaxY - eye2.MinY };
                }
                else
                {
                    rightEye = new EyeInfo { Side = Side.Right, X = (int)eye1.CenterX, Y = (int)eye1.CenterY, Width = eye1.MaxX - eye1.MinX, Height = eye1.MaxY - eye1.MinY };
                    leftEye = new EyeInfo { Side = Side.Left, X = (int)eye2.CenterX, Y = (int)eye2.CenterY, Width = eye2.MaxX - eye2.MinX, Height = eye2.MaxY - eye2.MinY };
                }

                FaceEyesInfo f = new FaceEyesInfo { Eyes = new List<EyeInfo> { leftEye, rightEye } };
                facesInfo.Add(f);
            }

            return facesInfo;
        }

    }
}
