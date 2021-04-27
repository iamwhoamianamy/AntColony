using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyRaster
{
   public class Neighbour
   {
      public Point point;
      public float distance;

      public Neighbour(Point point, float distance)
      {
         this.point = point;
         this.distance = distance;
      }
   }
}
