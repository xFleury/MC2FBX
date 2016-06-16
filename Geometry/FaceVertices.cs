using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC2UE.Geometry
{
    /// <summary>A convex co-planar face defined by four counter-clockwise vertices</summary>
    struct FaceVertices
    {
        public int index1;
        public int index2;
        public int index3;
        public int index4;

        public FaceVertices(int index1, int index2, int index3, int index4)
        {
            this.index1 = index1;
            this.index2 = index2;
            this.index3 = index3;
            this.index4 = index4;
        }
    }
}
