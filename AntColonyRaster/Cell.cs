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
      public Color4 color;

      public bool isCarryingHomePher;
      public bool isCarryingFoodPher;

      public bool isCarryingFood;

      public float foodSaturation;
      public float homePherSat;
      public float foodPherSat;

      public bool isCarryingAnt;

      public float maxPherSat = 500;

      public Vector2 pos;

      public Cell(Vector2 pos)
      {
         color = Color4.Black;

         isCarryingHomePher = false;
         isCarryingFoodPher = false;
         isCarryingFood = false;

         isCarryingAnt = false;

         homePherSat = 0f;
         foodPherSat = 0f;
         foodSaturation = 0f;

         this.pos = pos;
      }

      public void Draw()
      {
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
                  float homeSat = homePherSat / maxPherSat;
                  color.R = homeSat;
               }
               if (isCarryingFoodPher)
               {
                  float foodSat = foodPherSat / maxPherSat;
                  color.B = foodSat;
               }
            }
         }

         GL.Color4(color);
         GL.Begin(BeginMode.Points);
         GL.Vertex2(pos.X, pos.Y);
         GL.End();
      }
   }
}
