using System;
using System.Collections.Generic;
using System.Text;

namespace MC2FBX
{
    class PythonScript
    {
        StringBuilder script = new StringBuilder(
            "import bpy\n" +
            "bpy.data.objects['Camera'].select = True\n" +
            "bpy.data.objects['Lamp'].select = True\n" +
            "bpy.ops.object.delete()\n" +
            "class Cube :\n" + 
            "\tdef __init__(self, x, y, z, sx, sy, sz):\n" + 
            "\t\tself.x = x\n" + 
            "\t\tself.y = y\n" +
            "\t\tself.z = z\n" + 
            "\t\tself.sx = sx\n" + 
            "\t\tself.sy = sy\n" + 
            "\t\tself.sz = sz\n"
            );

        public void CreateCollisionBoxes(string name, List<Volume> volumes)
        {
            script.Append("cubes = [");
            for (int idx = 0; idx < volumes.Count; idx++)
            {
                Volume volume = volumes[idx];
                decimal deciX = volume.ScaleX / 2.0m + volume.Coord.X;
                decimal deciY = volume.ScaleY / 2.0m + volume.Coord.Y;
                decimal deciZ = volume.ScaleZ / 2.0m + volume.Coord.Z;

                if (idx > 0) script.Append(", ");
                script.Append(string.Format("Cube({0}, {1}, {2}, {3}, {4}, {5})",
                    deciX, deciY, deciZ, volume.ScaleX, volume.ScaleY, volume.ScaleZ));
            }
            script.Append(
                "]\n" + 
                "count = 0\n" + 
                "for cube in cubes:\n" + 
                "\tbpy.ops.mesh.primitive_cube_add(location = (cube.x, cube.y, cube.z), radius = 0.5)\n" + 
                "\tbpy.context.object.name = \"UBX_" + name + "_\"+'{0:02d}'.format(count)\n" + 
                "\tbpy.context.active_object.scale = (cube.sx,cube.sy,cube.sz)\n" +                 
                "\tbpy.context.object.scale[0] = bpy.context.object.scale[0] - 0.05\n" +
                "\tbpy.context.object.scale[1] = bpy.context.object.scale[1] - 0.05\n" + 
                "\tbpy.context.object.scale[2] = bpy.context.object.scale[2] - 0.05\n" +
                "\tcount += 1\n");
        }

        public void CreateBoxes(string name, List<Volume> volumes)
        {
            script.Append("cubes = [");
            for (int idx = 0; idx < volumes.Count; idx++)
            {
                Volume volume = volumes[idx];
                decimal deciX = volume.ScaleX / 2.0m + volume.Coord.X;
                decimal deciY = volume.ScaleY / 2.0m + volume.Coord.Y;
                decimal deciZ = volume.ScaleZ / 2.0m + volume.Coord.Z;

                if (idx > 0) script.Append(", ");
                script.Append(string.Format("Cube({0}, {1}, {2}, {3}, {4}, {5})",
                    deciX, deciY, deciZ, volume.ScaleX, volume.ScaleY, volume.ScaleZ));
            }
            script.Append(
                "]\n" +
                "count = 0\n" +
                "for cube in cubes:\n" +
                "\tbpy.ops.mesh.primitive_cube_add(location = (cube.x, cube.y, cube.z), radius = 0.5)\n" +
                "\tbpy.context.object.name = \"" + name + "\"\n" +
                "\tbpy.context.active_object.scale = (cube.sx,cube.sy,cube.sz)\n" +
                "\tif count > 0:\n" +
                "\t\tbpy.context.scene.objects.active = bpy.data.objects['" + name + "']\n" + 
                "\t\tbpy.data.objects['" + name + ".001'].select = True\n" +
                "\t\tbpy.data.objects['" + name + "'].select = True\n" +
                "\t\tbpy.ops.object.join()\n" + 
                "\tcount += 1\n");
        }

        public void AddBlock(int intX, int intY, int intZ, string strName,
            int spanX = 1, int spanY = 1, int spanZ = 1)
        {
            decimal deciX = spanX / 2.0m + intX;
            decimal deciY = spanY / 2.0m + intY;
            decimal deciZ = spanZ / 2.0m + intZ;

            string command = string.Format(
                "bpy.ops.mesh.primitive_cube_add(location = ({0}, {1}, {2}), radius = 0.5)\n" +
                "bpy.context.object.name = \"{3}\"\n" +
                "bpy.context.active_object.scale = ({4},{5},{6})\n",
                /* Collision box. */
                //"bpy.ops.mesh.primitive_cube_add(location = ({0}, {1}, {2}), radius = 0.5)\n" +
                //"bpy.context.object.name = \"UBX_{3}\"\n" +
                //"bpy.context.active_object.scale = ({4},{5},{6})\n" +
                //"bpy.context.object.scale[0] = bpy.context.object.scale[0] - 0.05\n" +
                //"bpy.context.object.scale[1] = bpy.context.object.scale[1] - 0.05\n" +
                //"bpy.context.object.scale[2] = bpy.context.object.scale[2] - 0.05\n",
                deciX, deciY, deciZ, strName, spanX, spanY, spanZ);
            script.Append(command);
        }

        public override string ToString()
        {
            return script.ToString();
        }
    }
}
