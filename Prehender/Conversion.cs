using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace Prehender
{
    public sealed class Conversion
    {
        public static float[] ToFloat(JVector vector)
        {
            return new float[4] { vector.X, vector.Y, vector.Z, 0.0f };
        }

        public static float[] ToFloat(JMatrix matrix)
        {
            return new float[12] { matrix.M11, matrix.M21, matrix.M31, 0.0f,
                                   matrix.M12, matrix.M22, matrix.M32, 0.0f,
                                   matrix.M13, matrix.M23, matrix.M33, 1.0f };
        }
    }

}
