using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace AntColony
{
   class Misc
   {
      public static void DrawRect(float x, float y, float w, float h)
      {
         GL.Begin(BeginMode.LineLoop);

         GL.Vertex2(x - w / 2, y - h / 2);
         GL.Vertex2(x + w / 2, y - h / 2);
         GL.Vertex2(x + w / 2, y + h / 2);
         GL.Vertex2(x - w / 2, y + h / 2);

         GL.End();
      }

      public static void DrawRect(Vector2 loc, Vector2 dim)
      {
         DrawRect(loc.X, loc.Y, dim.X, dim.Y);
      }
   }

   
}
