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

   class Colony
   {
      public List<Ant> ants;
      public QTree antsQTree;
      public QTree pathPheromonesQTree;
      public QTree foodPheromonesQTree;

      public List<Pheromone> pathPheromones;
      public List<Pheromone> foodPheromones;

      public float wanderStrength = 0f;

      public Colony()
      {
         ants = new List<Ant>();
         pathPheromones = new List<Pheromone>();
         foodPheromones = new List<Pheromone>();
      }

      public void BounceFromBorders(float w, float h)
      {
         foreach (var ant in ants)
         {
            ant.BounceFromBorders(w, h);
         }
      }

      public void Wander(Random r)
      {
         foreach (var ant in ants)
         {
            ant.Wander(r.NextDouble(), wanderStrength);
         }
      }

      public void UpdatePheromones()
      {
         List<Pheromone> toRemove = new List<Pheromone>();

         foreach (var p in pathPheromones)
         {
            if (p.durationLeft == 0)
               toRemove.Add(p);
            else
               p.durationLeft--;
         }

         pathPheromones.RemoveAll(p => toRemove.Contains(p));

         toRemove = new List<Pheromone>();

         foreach (var p in foodPheromones)
         {
            if (p.durationLeft == 0)
               toRemove.Add(p);
            else
               p.durationLeft--;
         }

         foodPheromones.RemoveAll(p => toRemove.Contains(p));

         foreach (var ant in ants)
         {
            if(ant.isCarryingFood)
               foodPheromones.Add(new Pheromone(3, ant.loc, 50));
            else
               pathPheromones.Add(new Pheromone(3, ant.loc, 50));
         }
      }

      public void DrawPheromones()
      {
         GL.PointSize(4);
         GL.Enable(EnableCap.PointSmooth);

         GL.Begin(PrimitiveType.Points);
         foreach (var p in pathPheromones)
         {
            float value = (float)(p.durationLeft) / p.duration;
            GL.Color3(0f, 0f, value);
            GL.Vertex2(p.loc);
         }
         GL.End();

         GL.Begin(PrimitiveType.Points);
         foreach (var p in foodPheromones)
         {
            float value = (float)(p.durationLeft) / p.duration;
            GL.Color3(value, 0f, 0f);
            GL.Vertex2(p.loc);
         }
         GL.End();

         GL.Disable(EnableCap.PointSmooth);
      }
      public void SeekFood(QTree foodQTree)
      {
         foreach (var ant in ants)
         {
            if (!ant.isCarryingFood)
            {
               List<Neighbour> neibs = new List<Neighbour>();
               foodQTree.Quarry(new Point(ant.loc + ant.vel.Normalized() * ant.size * 1.5f), ant.size * 3.0f, neibs);
               
               if (neibs.Count != 0)
               {
                  Neighbour minNeib = neibs[0];
                  float minDist = Vector2.Distance(neibs[0].point.loc, ant.loc);

                  foreach (var n in neibs)
                  {
                     float dist = Vector2.Distance(n.point.loc, ant.loc);
                     if (dist < minDist)
                     {
                        minDist = dist;
                        minNeib = n;
                     }
                  }

                  if (minDist < ant.size)
                  {
                     ant.isCarryingFood = true;
                     ant.vel *= -1;
                  }
                  else
                     ant.Steer(minNeib.point.loc);
               }
            }
         }
      }

      public void SeekHome(Vector2 home)
      {
         foreach (var ant in ants)
         {
            if (ant.isCarryingFood)
            {
               float dist = Vector2.Distance(home, ant.loc);
               if (dist < ant.size * 3f)
                  ant.Steer(home);

               if(dist < ant.size)
               {
                  ant.isCarryingFood = false;
                  ant.vel *= -1;
               }
            }
            
         }
      }

      public void FollowPheromones()
      {
         foreach (var ant in ants)
         {
            List<Neighbour> centNeibs = new List<Neighbour>();
            List<Neighbour> leftNeibs = new List<Neighbour>();
            List<Neighbour> rightNeibs = new List<Neighbour>();

            QTree tTree;

            if (ant.isCarryingFood)
               tTree = pathPheromonesQTree;
            else
               tTree = foodPheromonesQTree;

            tTree.Quarry(new Point(ant.loc + ant.vel.Normalized() * ant.size * 1.75f),
                                ant.size, centNeibs);
            tTree.Quarry(new Point(ant.loc + Misc.RotateVector(ant.vel.Normalized() * ant.size * 1.75f, 30f * Math.PI / 180f)),
                             ant.size, leftNeibs);
            tTree.Quarry(new Point(ant.loc + Misc.RotateVector(ant.vel.Normalized() * ant.size * 1.75f, -30f * Math.PI / 180f)),
                             ant.size, rightNeibs);

            if (leftNeibs.Count != 0 && leftNeibs.Count >= centNeibs.Count && leftNeibs.Count >= rightNeibs.Count)
            {
               ant.vel = Misc.RotateVector(ant.vel, 1f * Math.PI / 180f);
            }
            else
            if (rightNeibs.Count != 0 && rightNeibs.Count >= centNeibs.Count && rightNeibs.Count >= leftNeibs.Count)
            {
               ant.vel = Misc.RotateVector(ant.vel, -1f * Math.PI / 180f);
            }
         }
      }

      public void AvoidBorders(float perseption, int w, int h)
      {
         foreach (var ant in ants)
         {
            ant.AvoidBorders(perseption, w, h);
         }
      }

      public void UpdateLocation()
      {
         foreach (var ant in ants)
         {
            ant.UpdateLocation();
         }
      }

      public void DrawAnts()
      {
         GL.PointSize(10);
         GL.Enable(EnableCap.PointSmooth);
         GL.Color3(1f, 1f, 1f);

         GL.Begin(PrimitiveType.Points);

         foreach (var ant in ants)
            GL.Vertex2(ant.loc);

         GL.End();

         GL.Disable(EnableCap.PointSmooth);
      }
   }
}
