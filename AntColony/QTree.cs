using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntColony
{
   class QTree
   {
      Vector2 loc;
      Vector2 dimension;
      Vector3 colour;

      QTree topleft;
      QTree topright;
      QTree botleft;
      QTree botright;

      bool divided;

      public int capacity;

      float width = 8.888888f;
      float height = 5;

      List<Point> points;
      public QTree(int _capacity)
      {
         loc = Vector2.Zero;
         dimension = new Vector2(width * 2, height * 2);

         points = new List<Point>();

         divided = false;

         capacity = _capacity;

         colour = new Vector3(0, 255, 0);
      }
      public QTree(Vector2 _loc, Vector2 _dimension, int _capacity)
      {
         loc = new Vector2(_loc.X, _loc.Y);
         dimension = new Vector2(_dimension.X, _dimension.Y);

         points = new List<Point>();

         divided = false;

         capacity = _capacity;

         colour = new Vector3(0, 255, 0);
      }
      void Subdivide()
      {
         if (!divided)
         {
            topleft =  new QTree(new Vector2(loc.X - dimension.X / 4, loc.Y - dimension.Y / 4), new Vector2(dimension.X / 2, dimension.Y / 2), capacity);
            topright = new QTree(new Vector2(loc.X + dimension.X / 4, loc.Y - dimension.Y / 4), new Vector2(dimension.X / 2, dimension.Y / 2), capacity);
            botleft =  new QTree(new Vector2(loc.X - dimension.X / 4, loc.Y + dimension.Y / 4), new Vector2(dimension.X / 2, dimension.Y / 2), capacity);
            botright = new QTree(new Vector2(loc.X + dimension.X / 4, loc.Y + dimension.Y / 4), new Vector2(dimension.X / 2, dimension.Y / 2), capacity);

            divided = true;
         }
      }
      public void Render()
      {
        

         if (divided)
         {
            topleft.Render();
            topright.Render();
            botleft.Render();
            botright.Render();
         }
      }
      public void Fill(List<Point> points)
      {
         foreach (Point p in points)
            Insert(p);
      }
      void Insert(Point point)
      {
         if (!Contains(point.loc, loc, dimension))
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
         return (x - w / 2 <= loc.X + dimension.X / 2 && x + w / 2 >= loc.X - dimension.X / 2) &&
                (y - w / 2 <= loc.Y + dimension.Y / 2 && y + w / 2 >= loc.Y - dimension.Y / 2);
      }
      public void Quarry(Point centralPoint, float widthOfSearch, List<Neighbour> found)
      {
         if (!IntersectsWithRect(centralPoint.loc.X, centralPoint.loc.Y, widthOfSearch))
            return;
         else
         {
            colour = new Vector3(255, 255, 0);
            foreach (Point other in points)
               if (centralPoint != other && Contains(other.loc, new Vector2(centralPoint.loc.X, centralPoint.loc.Y), new Vector2(widthOfSearch, widthOfSearch)))
               {
                  //float xLen = centralPoint.loc.X - other.loc.X;
                  //float yLen = centralPoint.loc.Y - other.loc.Y;
                  found.Add(new Neighbour(other, Vector2.Distance(centralPoint.loc, other.loc)));
                  //found.add(new Neighbour(other, xLen * xLen + yLen * yLen));
               }
         }
         if (divided)
         {
            topleft.Quarry(centralPoint, widthOfSearch, found);
            topright.Quarry(centralPoint, widthOfSearch, found);
            botleft.Quarry(centralPoint, widthOfSearch, found);
            botright.Quarry(centralPoint, widthOfSearch, found);
         }
      }
   }
}
