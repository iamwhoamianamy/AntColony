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
      public QTree qTree;

      public float wanderStrength = 0f;

      public Colony()
      {
         ants = new List<Ant>();
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
         foreach (var ant in ants)
         {
            ant.UpdatePheromones();
         }
      }

      public void DrawPheromones()
      {
         foreach (var ant in ants)
         {
            GL.PointSize(4);
            GL.Enable(EnableCap.PointSmooth);

            GL.Begin(PrimitiveType.Points);

            foreach (var p in ant.pheromones)
            {
               float value = (float)(p.durationLeft) / p.duration;

               if (p.type == PhTypes.Path)
                  GL.Color3(0f, 0f, value);
               else
                  GL.Color3(value, 0f, 0f);

               GL.Vertex2(p.loc);
            }

            GL.End();

            GL.Disable(EnableCap.PointSmooth);
         }
      }

      public void FollowPheromones(QTree pheromonesQTree)
      {
         foreach (var ant in ants)
         {
            List<Neighbour> centNeibs = new List<Neighbour>();
            List<Neighbour> leftNeibs = new List<Neighbour>();
            List<Neighbour> rightNeibs = new List<Neighbour>();

            pheromonesQTree.Quarry(new Point(ant.loc + ant.vel.Normalized() * ant.size * 1.75f),
                             ant.size, centNeibs);
            pheromonesQTree.Quarry(new Point(ant.loc + Misc.RotateVector(ant.vel.Normalized() * ant.size * 1.75f, 30f * Math.PI / 180f)),
                             ant.size, leftNeibs);
            pheromonesQTree.Quarry(new Point(ant.loc + Misc.RotateVector(ant.vel.Normalized() * ant.size * 1.75f, -30f * Math.PI / 180f)),
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
