using System;
using System.Collections.Generic;
using System.Text;

namespace MC2Blender
{
    class PythonScript
    {
        StringBuilder script = new StringBuilder(
            "import bpy\n" +
            "bpy.data.objects['Camera'].select = True\n" +
            "bpy.data.objects['Lamp'].select = True\n" +
            "bpy.ops.object.delete()\n");

        public void AddBlock(int intX, int intY, int intZ, string strName,
            int spanX = 1, int spanY = 1, int spanZ = 1)
        {
            decimal deciX = spanX / 2.0m + intX;
            decimal deciY = spanY / 2.0m + intY;
            decimal deciZ = spanZ / 2.0m + intZ;

            string command = string.Format(
                "bpy.ops.mesh.primitive_cube_add(location = ({0}, {1}, {2}), radius = 0.5)\n" +
                "bpy.context.object.name = \"{3}\"\n" +
                "bpy.context.active_object.scale = ({4},{5},{6})\n" +
                /* Collision box. */
                "bpy.ops.mesh.primitive_cube_add(location = ({0}, {1}, {2}), radius = 0.5)\n" +
                "bpy.context.object.name = \"UBX_{3}\"\n" +
                "bpy.context.active_object.scale = ({4},{5},{6})\n" +
                "bpy.context.object.scale[0] = bpy.context.object.scale[0] - 0.05\n" +
                "bpy.context.object.scale[1] = bpy.context.object.scale[1] - 0.05\n" +
                "bpy.context.object.scale[2] = bpy.context.object.scale[2] - 0.05\n",
                deciX, deciY, deciZ, strName, spanX, spanY, spanZ);
            script.Append(command);
        }

        public override string ToString()
        {
            return script.ToString();
        }
    }
}
