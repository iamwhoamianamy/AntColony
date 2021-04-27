using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace AntColonyRaster
{

   class Colony
   {
      public List<Ant> ants;
      float pheromonesSize = 4f;
      public float wanderStrength = 3f;

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
            ant.Wander(r.NextDouble() * 360f, wanderStrength);
         }
      }

      public void SeekHome(Vector2 home)
      {
         foreach (var ant in ants)
         {
            if (ant.isCarryingFood)
            {
               float dist = Vector2.Distance(home, ant.loc);
               if (dist < ant.size * 3.5f)
                  ant.Steer(home, 1f);

               // Ant has brought food to home
               if(dist < ant.size * 2f)
               {
                  ant.isCarryingFood = false;
                  ant.vel *= -1;
                  if(ant.pheromoneDuration < ant.maxPheromoneDuration)
                     ant.pheromoneDuration += 5;

                  ant.pheromoneDurationLeft = ant.pheromoneDuration;
               }
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
         if(ants.Count != 0)
         {
            GL.Enable(EnableCap.PointSmooth);
            GL.Color3(1f, 1f, 1f);
            GL.PointSize(ants[0].size / 2f);

            GL.Begin(PrimitiveType.Points);

            foreach (var ant in ants)
               GL.Vertex2(ant.loc);

            GL.End();

            GL.Disable(EnableCap.PointSmooth);
         }
      }
   }
}
