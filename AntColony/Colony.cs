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

      public void Update()
      {
         foreach (var ant in ants)
         {
            ant.Update();
         }
      }

      public void Draw()
      {
         GL.PointSize(10);
         GL.Begin(PrimitiveType.Points);
         GL.Color3(1f, 1f, 1f);

         foreach (var ant in ants)
            GL.Vertex2(ant.loc);

         GL.End();
      }
   }
}
