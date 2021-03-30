using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace AntColonyPure
{

   class Colony
   {
      public List<Ant> ants;
      public QTree antsQTree;
      public QTree pathPheromonesQTree;
      public QTree foodPheromonesQTree;

      public List<Pheromone> pathPheromones;
      public List<Pheromone> foodPheromones;

      public float wanderStrength = 3f;

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
            ant.Wander(r.NextDouble() * 360f, wanderStrength);
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
               p.UpdateSaturation();
         }

         pathPheromones.RemoveAll(p => toRemove.Contains(p));

         toRemove = new List<Pheromone>();

         foreach (var p in foodPheromones)
         {
            if (p.durationLeft == 0)
               toRemove.Add(p);
            else
               p.UpdateSaturation();
         }

         foodPheromones.RemoveAll(p => toRemove.Contains(p));

         foreach (var ant in ants)
         {
            if(ant.isCarryingFood)
               foodPheromones.Add(new Pheromone(3, ant.loc, 150));
            else
               pathPheromones.Add(new Pheromone(3, ant.loc, 150));
         }
      }

      public void DrawPheromones()
      {
         GL.PointSize(2);
         GL.Enable(EnableCap.PointSmooth);

         GL.Begin(PrimitiveType.Points);
         foreach (var p in pathPheromones)
         {
            GL.Color3(0f, 0f, p.saturation);
            GL.Vertex2(p.loc);
         }
         GL.End();

         GL.Begin(PrimitiveType.Points);
         foreach (var p in foodPheromones)
         {
            GL.Color3(p.saturation, 0f, 0f);
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
               List<Point> neibs = new List<Point>();
               foodQTree.Quarry(new Point(ant.loc + ant.vel.Normalized() * ant.size * 3.5f), ant.size * 5f, neibs);

               if (neibs.Count != 0)
               {
                  Point minNeib = neibs[0];
                  float minDist = Vector2.Distance(neibs[0].loc, ant.loc);

                  foreach (var n in neibs)
                  {
                     float dist = Vector2.Distance(n.loc, ant.loc);
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
                     ant.Steer(minNeib.loc, 1f);
               }
            }
         }
      }
      //public void SeekFood(QTree foodQTree)
      //{
      //   foreach (var ant in ants)
      //   {
      //      GL.Color3(0, 0, 0);

      //      if (!ant.isCarryingFood)
      //      {
      //         List<Neighbour> neibs = new List<Neighbour>();
      //         foodQTree.QuarryOne(new Point(ant.loc + ant.vel.Normalized() * ant.size * 3f),
      //            ant.size * 6.0f, neibs);

      //         if (neibs.Count != 0)
      //         {
      //            float dist = Vector2.Distance(neibs[0].point.loc, ant.loc);
      //            if (dist < ant.size)
      //            {
      //               ant.isCarryingFood = true;
      //               ant.vel *= -1;
      //            }
      //            else
      //               ant.Steer(neibs[0].point.loc);
      //         }
      //      }
      //   }
      //}

      public void SeekHome(Vector2 home)
      {
         foreach (var ant in ants)
         {
            if (ant.isCarryingFood)
            {
               float dist = Vector2.Distance(home, ant.loc);
               if (dist < ant.size * 5f)
                  ant.Steer(home, 1f);

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
            QTree tTree;

            if (ant.isCarryingFood)
               tTree = pathPheromonesQTree;
            else
               tTree = foodPheromonesQTree;

            //Misc.DrawRect(ant.loc + ant.vel.Normalized() * ant.size * 3.1f, new Vector2(ant.size * 3.8f));

            List<Point> neibs = new List<Point>();
            tTree.Quarry(new Point(ant.loc + ant.vel.Normalized() * ant.size * 3.1f),
               ant.size * 3.8f, neibs);

            if (neibs.Count != 0)
            {
               Vector2 temp = Vector2.Zero;

               foreach (var n in neibs)
                  temp += n.loc;

               ant.Steer(temp / neibs.Count(), 0.1f);
            }
         }
         //foreach (var ant in ants)
         //{
         //   List<Neighbour> centNeibs = new List<Neighbour>();
         //   List<Neighbour> leftNeibs = new List<Neighbour>();
         //   List<Neighbour> rightNeibs = new List<Neighbour>();

         //   QTree tTree;

         //   if (ant.isCarryingFood)
         //      tTree = pathPheromonesQTree;
         //   else
         //      tTree = foodPheromonesQTree;

         //   Vector2 leftZone = ant.loc + Misc.RotateVector(ant.vel.Normalized() * ant.size * 1.5f, 30f * Math.PI / 180f);
         //   Vector2 centZone = ant.loc + ant.vel.Normalized() * ant.size * 1.5f;
         //   Vector2 rightZone = ant.loc + Misc.RotateVector(ant.vel.Normalized() * ant.size * 1.5f, -30f * Math.PI / 180f);

         //   //Misc.DrawRect(leftZone, new Vector2(ant.size * 1.25f));
         //   //Misc.DrawRect(centZone, new Vector2(ant.size * 1.25f));
         //   //Misc.DrawRect(rightZone, new Vector2(ant.size * 1.25f));

         //   tTree.Quarry(new Point(leftZone), ant.size * 1.25f, leftNeibs);
         //   tTree.Quarry(new Point(centZone), ant.size * 1.25f, centNeibs);
         //   tTree.Quarry(new Point(rightZone), ant.size * 1.25f, rightNeibs);

         //   if (leftNeibs.Count != 0 && leftNeibs.Count >= centNeibs.Count && leftNeibs.Count >= rightNeibs.Count)
         //      //ant.vel = Misc.RotateVector(ant.vel, 2f * Math.PI / 180f);
         //      ant.Steer(leftZone);
         //   else
         //   if (rightNeibs.Count != 0 && rightNeibs.Count >= centNeibs.Count && rightNeibs.Count >= leftNeibs.Count)
         //      //ant.vel = Misc.RotateVector(ant.vel, -2f * Math.PI / 180f);
         //      ant.Steer(rightZone);

         //   //else
         //   //if (centNeibs.Count != 0 && centNeibs.Count >= leftNeibs.Count && centNeibs.Count >= rightNeibs.Count) ;

         //   //float leftSat = 0;
         //   //float centSat = 0;
         //   //float rightSat = 0;

         //   //if(leftNeibs.Count != 0)
         //   //   foreach (var n in leftNeibs)
         //   //      leftSat += ((Pheromone)n.point).saturation;

         //   //if (centNeibs.Count != 0)
         //   //   foreach (var n in centNeibs)
         //   //      centSat += ((Pheromone)n.point).saturation;

         //   //if (rightNeibs.Count != 0)
         //   //   foreach (var n in rightNeibs)
         //   //      rightSat += ((Pheromone)n.point).saturation;

         //   //if(leftSat <= centSat && leftSat <= rightSat)
         //   //   ant.Steer(leftZone);

         //   //if (rightSat <= leftSat && rightSat <= centSat)
         //   //   ant.Steer(rightZone);
         //}
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
         GL.Enable(EnableCap.PointSmooth);
         GL.Color3(1f, 1f, 1f);

         GL.Begin(PrimitiveType.Points);

         foreach (var ant in ants)
         {
            GL.PointSize(ant.size);
            GL.Vertex2(ant.loc);
         }

         GL.End();

         GL.Disable(EnableCap.PointSmooth);
      }
   }
}
