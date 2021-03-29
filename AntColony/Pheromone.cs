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
   enum PhTypes
   {
      Path,
      PathWithFood
   }

   class Pheromone : Point
   {
      public int duration = 0;
      public int durationLeft = 0;
      //public PhTypes type = PhTypes.Path;
      
      public Pheromone(float _size, Vector2 _loc, int _duration) :
        base(_size, _loc)
      {
         duration = _duration;
         durationLeft = _duration;
         //type = _type;
      }
   }
}
