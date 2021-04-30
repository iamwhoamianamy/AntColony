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
   class Cell
   {
      public bool isCarryingHomePher;
      public bool isCarryingFoodPher;

      public bool isCarryingFood;

      public float foodSaturation;
      public float homePherSat;
      public float foodPherSat;

      public bool isCarryingAnt;

      public Vector2 loc;

      public Cell(Vector2 loc)
      {
         isCarryingHomePher = false;
         isCarryingFoodPher = false;
         isCarryingFood = false;

         isCarryingAnt = false;

         homePherSat = 0f;
         foodPherSat = 0f;
         foodSaturation = 0f;

         this.loc = loc;
      }

      public void Draw()
      {
         Color4 color = new Color4(0.3f, 0.1f, 0.1f, 1f);

         if (isCarryingFood)
         {
            color.G = foodSaturation;
         }
         else
         {
            if (isCarryingAnt)
               color = Color4.White;
            else
            {
               if (isCarryingHomePher)
               {
                  float homeSat = homePherSat;
                  color.R += homeSat;
               }
               if (isCarryingFoodPher)
               {
                  float foodSat = foodPherSat;
                  color.B += foodSat;
               }
            }
         }

         GL.Color4(color);
         GL.Begin(BeginMode.Points);
         GL.Vertex2(loc.X, loc.Y);
         GL.End();
      }
   }
}
