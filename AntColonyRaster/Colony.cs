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
      public float wanderStrength = 3f;
      private float antSaturationDecayRate = 0.0005f;

      public Colony()
      {
         ants = new List<Ant>();
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

      public void PerhormBehaviour(Random r, float width, float height)
      {
         Vector2 home = new Vector2(width / 2, height / 2);

         foreach (var ant in ants)
         {
            ant.Wander(r.NextDouble() * 360f, wanderStrength);
            ant.SeekHome(home);
            ant.AvoidBorders(30, width, height);
            ant.BounceFromBorders(width, height);

            if (ant.isLockedOnFood)
               if(ant.foodAim.isCarryingFood)
                  ant.Steer(ant.foodAim.loc, 2f);

            if (ant.isLockedOnHome)
               ant.Steer(ant.homeAim, 2f);

            ant.UpdateLocation();
         }

      }

      public void UpdatePheromones()
      {
         for (int i = 0; i < ants.Count; i++)
         {
            if(ants[i].isCarryingPheromone)
            {
               ants[i].pheromoneDurationLeft -= antSaturationDecayRate;
               if (ants[i].pheromoneDurationLeft <= 0)
                  ants[i].isCarryingPheromone = false;
            }
         }
      }
   }
}
