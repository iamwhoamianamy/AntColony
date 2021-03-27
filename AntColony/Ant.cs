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
   class Ant : Point
   {
      public static float maxSpeed = 0.0f;
      public static float steerStrength = 0.1f;
      public static float avoidStrenght = 0.5f;

      public Vector2 followTarget;
      public Vector2 avoidTarget;
      public bool isFollowing = false;
      public bool isAvoiding = false;

      public List<Pheromone> pheromones;
      public bool isCarryingFood = false;

      public Ant(float _size, Vector2 _loc, Vector2 _vel) :
         base(_size, _loc, _vel)
      {
         maxSpeed = _vel.Length;
         pheromones = new List<Pheromone>();
         avoidTarget = new Vector2();
      }

      public void UpdateLocation()
      {
         if (isAvoiding)
            if(Vector2.Distance(avoidTarget, loc) > size * 2)
               isAvoiding = false;
            else
               Avoid(avoidTarget);

         vel = (vel + acc).Normalized() * maxSpeed;
         loc += vel;

         acc = Vector2.Zero;
      }

      public void UpdatePheromones()
      {
         List<Pheromone> toRemove = new List<Pheromone>();

         foreach (var p in pheromones)
         {
            if (p.durationLeft == 0)
               toRemove.Add(p);
            else
               p.durationLeft--;
         }

         pheromones.RemoveAll(p => toRemove.Contains(p));
         pheromones.Add(new Pheromone(3, loc, 50, isCarryingFood ? PhTypes.PathWithFood : PhTypes.Path));
      }

      public void Steer(Vector2 target)
      {
         Vector2 desiredDirection = (target - loc).Normalized();
         Vector2 desiredVelocity = desiredDirection * maxSpeed;
         Vector2 desiredSteeringForce = (desiredVelocity - vel) * steerStrength;
         acc += (desiredSteeringForce.Normalized() * steerStrength) / 1;
      }

      public void Avoid(Vector2 target)
      {
         Vector2 desiredDirection = (target - loc).Normalized();
         Vector2 desiredVelocity = desiredDirection * maxSpeed;
         Vector2 desiredSteeringForce = (desiredVelocity - vel) * avoidStrenght;
         acc -= (desiredSteeringForce.Normalized() * avoidStrenght) / 1;
      }

      public void Wander(double r, float wanderStrength)
      {
         Vector2 desiredDirection = Misc.VecFromAng(r) * wanderStrength;
         Vector2 desiredVelocity = desiredDirection * maxSpeed;
         Vector2 desiredSteeringForce = (desiredVelocity - vel) * steerStrength;
         acc += (desiredSteeringForce.Normalized() * steerStrength) / 1;
      }

      public void AvoidBorders(float perseption, int w, int h)
      {
         if (loc.Y > h - perseption)
         {
            Avoid(new Vector2(loc.X, h));
         }

         if (loc.Y < perseption)
         {
            Avoid(new Vector2(loc.X, 0));
         }

         if (loc.X > w - perseption)
         {
            Avoid(new Vector2(w, loc.Y));
         }

         if (loc.X < perseption)
         {
            Avoid(new Vector2(0, loc.Y));
         }

         //Vector2 toAvoidCentr = loc + vel.Normalized() * perseption;
         //toAvoidCentr.X -= 0.01f;

         //float ang = -30f * (float)Math.PI / 180f;

         //Vector2 toAvoidLeft = vel.Normalized() * perseption;
         //toAvoidLeft.X = toAvoidLeft.X * (float)Math.Cos(ang) - toAvoidLeft.Y * (float)Math.Sin(ang);
         //toAvoidLeft.Y = toAvoidLeft.X * (float)Math.Sin(ang) + toAvoidLeft.Y * (float)Math.Cos(ang);
         //toAvoidLeft += loc;

         //ang = 30f * (float)Math.PI / 180f;

         //Vector2 toAvoidRight = vel.Normalized() * perseption;
         //toAvoidRight.X = toAvoidRight.X * (float)Math.Cos(ang) - toAvoidRight.Y * (float)Math.Sin(ang);
         //toAvoidRight.Y = toAvoidRight.X * (float)Math.Sin(ang) + toAvoidRight.Y * (float)Math.Cos(ang);
         //toAvoidRight += loc;

         //GL.Begin(BeginMode.Points);
         //GL.Vertex2(toAvoidCentr);
         //GL.Vertex2(toAvoidRight);
         //GL.Vertex2(toAvoidLeft);
         //GL.End();

         //if (loc.Y > h - perseption)
         //{
         //   //if (toAvoidRight.Y > h)
         //   //{
         //   //   Avoid(toAvoidCentr);
         //   //}
         //   //else
         //   //if (toAvoidCentr.Y > h)
         //   //{
         //   //   Avoid(toAvoidCentr);
         //   //}
         //   //else
         //   //if (toAvoidLeft.Y > h)
         //   //{
         //   //   Avoid(toAvoidCentr);
         //   //}
         //   if (toAvoidCentr.Y > h)
         //   {
         //      if(toAvoidRight.Y < toAvoidLeft.Y)
         //         Avoid(toAvoidLeft);
         //      else
         //         Avoid(toAvoidRight);

         //   }
         //}

         //if (loc.Y > h - perseption)
         //{
         //   float t = Misc.RayLineIntersect(new Vector2(0, h),
         //                                   new Vector2(w, h),
         //                                   loc, vel);

         //   if (t != -1f)
         //   {
         //      Vector2 toAvoid = loc + vel * t;

         //      if (Vector2.Distance(toAvoid, loc) < perseption)
         //      {
         //         Avoid(toAvoid);
         //         //Vector2 toSteer = new Vector2(toAvoid.X + toAvoid.X - o.X, o.Y);
         //         //Steer(toSteer);
         //         GL.Begin(BeginMode.Points);
         //         GL.Vertex2(toAvoid);
         //         GL.End();
         //      }
         //   }
         //}
      }

   }
}
