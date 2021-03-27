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
      public Random r;

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

      public static float RayLineIntersect(Vector2 a, Vector2 b, Vector2 o, Vector2 d)
      {
         Vector2 v1 = o - a;
         Vector2 v2 = b - a;
         Vector2 v3 = new Vector2(-d.Y, d.X);

         float dot = Vector2.Dot(v2, v3);

         if (Math.Abs(dot) < 0.000001)
            return -1f;
         else
         {
            float t1 = Vector2.PerpDot(v2, v1) / dot;
            float t2 = Vector2.Dot(v1, v3) / dot;

            if (t1 >= 0.0 && (t2 >= 0.0 && t2 <= 1.0))
               return t1;
         }

         return -1f;
      }

      public static Vector2 VecFromAng(double ang, float length)
      {
         double rand = ang * 360f;
         return RotateVector(new Vector2(length, 0), rand);
      }

      public static Vector2 RotateVector(Vector2 vec, double ang)
      {
         Vector2 res = new Vector2(vec.X, vec.Y);
         res.X = res.X * (float)Math.Cos(ang) - res.Y * (float)Math.Sin(ang);
         res.Y = res.X * (float)Math.Sin(ang) + res.Y * (float)Math.Cos(ang);

         return res;
      }

      public static Vector2 VecFromAng(double ang)
      {
         return VecFromAng(ang, 1);
      }
   }

   
}
