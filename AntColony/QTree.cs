using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace AntColony
{
   class QTree
   {
      Vector2 loc;
      Vector2 dim;
      Vector3 colour;

      QTree topleft;
      QTree topright;
      QTree botleft;
      QTree botright;

      bool isDivided;
      public int capacity;

      List<Point> points;
      public QTree(Vector2 _loc, Vector2 _dimension, int _capacity)
      {
         loc = new Vector2(_loc.X, _loc.Y);
         dim = new Vector2(_dimension.X, _dimension.Y);

         points = new List<Point>();

         isDivided = false;

         capacity = _capacity;

         colour = new Vector3(0, 0.9f, 0);
      }
      void Subdivide()
      {
         if (!isDivided)
         {
            topleft =  new QTree(new Vector2(loc.X - dim.X / 4, loc.Y - dim.Y / 4), new Vector2(dim.X / 2, dim.Y / 2), capacity);
            topright = new QTree(new Vector2(loc.X + dim.X / 4, loc.Y - dim.Y / 4), new Vector2(dim.X / 2, dim.Y / 2), capacity);
            botleft =  new QTree(new Vector2(loc.X - dim.X / 4, loc.Y + dim.Y / 4), new Vector2(dim.X / 2, dim.Y / 2), capacity);
            botright = new QTree(new Vector2(loc.X + dim.X / 4, loc.Y + dim.Y / 4), new Vector2(dim.X / 2, dim.Y / 2), capacity);

            isDivided = true;
         }
      }
      public void Draw()
      {
         GL.Color3(colour);
         GL.LineWidth(5);

         Misc.DrawRect(loc, dim);

         if (isDivided)
         {
            topleft.Draw();
            topright.Draw();
            botleft.Draw();
            botright.Draw();
         }
      }
      public void Fill(List<Point> points)
      {
         foreach (Point p in points)
            Insert(p);
      }
      void Insert(Point point)
      {
         if (!Contains(point.loc, loc, dim))
            return;
         if (points.Count() < capacity)
            points.Add(point);
         else
         {
            Subdivide();
            topleft.Insert(point);
            topright.Insert(point);
            botleft.Insert(point);
            botright.Insert(point);
         }
      }
      bool Contains(Vector2 centerLocation, Vector2 location, Vector2 dimension)
      {
         return (centerLocation.X <= location.X + dimension.X / 2) &&
                (centerLocation.X >= location.X - dimension.X / 2) &&
                (centerLocation.Y <= location.Y + dimension.Y / 2) &&
                (centerLocation.Y >= location.Y - dimension.Y / 2);
      }
      bool IntersectsWithRect(float x, float y, float w)
      {
         return (x - w / 2 <= loc.X + dim.X / 2 && x + w / 2 >= loc.X - dim.X / 2) &&
                (y - w / 2 <= loc.Y + dim.Y / 2 && y + w / 2 >= loc.Y - dim.Y / 2);
      }
      public void Quarry(Point centralPoint, float widthOfSearch, List<Neighbour> found)
      {
         if (!IntersectsWithRect(centralPoint.loc.X, centralPoint.loc.Y, widthOfSearch))
            return;
         else
         {
            colour = new Vector3(1, 1, 0);
            foreach (Point other in points)
               if (centralPoint != other && Contains(other.loc, new Vector2(centralPoint.loc.X, centralPoint.loc.Y), new Vector2(widthOfSearch, widthOfSearch)))
               {
                  //float xLen = centralPoint.loc.X - other.loc.X;
                  //float yLen = centralPoint.loc.Y - other.loc.Y;
                  found.Add(new Neighbour(other, Vector2.Distance(centralPoint.loc, other.loc)));
                  //found.add(new Neighbour(other, xLen * xLen + yLen * yLen));
               }
         }
         if (isDivided)
         {
            topleft.Quarry(centralPoint, widthOfSearch, found);
            topright.Quarry(centralPoint, widthOfSearch, found);
            botleft.Quarry(centralPoint, widthOfSearch, found);
            botright.Quarry(centralPoint, widthOfSearch, found);
         }
      }
   }
}
