using NbtToObj.Geometry;
using NbtToObj.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtToObj.Wavefront
{
    static class UnrealBoxCollision
    {
        public static void MakeCollisionUBX(string name, Volume volume, List<CoordinateDecimal> vertices,
            Dictionary<string, List<FaceVertices>> collisionBoxes)
        {
            const decimal collisionPadding = 0.05m;
            List<FaceVertices> listOfFaceVertices = new List<FaceVertices>();

            Iterators.FacesInVolume(vertices.Count, Face.None, (Face face, FaceVertices faceVertices) =>
            { listOfFaceVertices.Add(faceVertices); });

            Iterators.VerticesInVolume(((CoordinateDecimal)volume.Coord).Offset(
                collisionPadding, collisionPadding, collisionPadding),
                volume.ScaleX - 2 * collisionPadding,
                volume.ScaleY - 2 * collisionPadding,
                volume.ScaleZ - 2 * collisionPadding,
                (CoordinateDecimal a) => { vertices.Add(a); });

            collisionBoxes.Add(name, listOfFaceVertices);
        }
    }
}
