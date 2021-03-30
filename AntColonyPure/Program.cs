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
   class Program
   {
      public static void Main()
      {
         using (Game game = new Game(1200, 800, "LearnOpenTK"))
         {
            
            game.Run(60.0);
         }
      }
   }
}
