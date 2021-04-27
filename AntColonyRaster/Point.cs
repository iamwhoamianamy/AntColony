using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK;

namespace AntColonyRaster
{
   public class Point
   {
      public Vector2 loc, vel, acc;
      public float size;

      public Point(Vector2 _loc)
      {
         loc = new Vector2(_loc.X, _loc.Y);
         vel = new Vector2(0, 0);
         acc = new Vector2(0, 0);
         size = 0;
      }

      public Point(float _size, Vector2 _loc)
      {
         loc = new Vector2(_loc.X, _loc.Y);
         vel = new Vector2(0, 0);
         acc = new Vector2(0, 0);
         size = _size;
      }
      public Point(float _size, Vector2 _loc, Vector2 _vel)
      {
         loc = new Vector2(_loc.X, _loc.Y);
         vel = new Vector2(_vel.X, _vel.Y);
         acc = new Vector2(0, 0);
         size = _size;
      }

      public Point Copy()
      {
         return new Point(0, loc);
      }

      public void BounceFromBorders(float w, float h)
      {
         if (loc.X < 0)
         {
            loc.X = 0;
            vel.X *= -1;
         }
         else
         {
            if (loc.X > w)
            {
               loc.X = w;
               vel.X *= -1;
            }
         }

         if (loc.Y < 0)
         {
            loc.Y = 0;
            vel.Y *= -1;
         }
         else
         {
            if (loc.Y > h)
            {
               loc.Y = h;
               vel.Y *= -1;
            }
         }
      }
   }
}
