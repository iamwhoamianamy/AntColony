using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK;

namespace AntColony
{
   public class Point
   {
      public Vector2 loc, vel, acc;
      public float size;

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

      Point Copy()
      {
         return new Point(0, loc);
      }
   }
}
