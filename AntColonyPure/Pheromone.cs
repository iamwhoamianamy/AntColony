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
   class Pheromone : Point
   {
      public float duration = 0;
      public float durationLeft = 0;
      public float saturation = 0;
      //public PhTypes type = PhTypes.Path;
      
      public Pheromone(float _size, Vector2 _loc, float _duration, float _durationLeft) :
        base(_size, _loc)
      {
         duration = _duration;
         durationLeft = _durationLeft;
         saturation = durationLeft / duration;
         //type = _type;
      }

      public void UpdateSaturation(float decayRate)
      {
         saturation = durationLeft / duration;
         durationLeft -= decayRate;
      }
   }
}
