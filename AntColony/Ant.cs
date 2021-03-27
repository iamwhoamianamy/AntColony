using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace AntColony
{
   class Ant : Point
   {
      public Ant(float _size, Vector2 _loc) :
         base(_size, _loc) { }
      public Ant(float _size, Vector2 _loc, Vector2 _vel) :
         base(_size, _loc, _vel) { }


   }
}
