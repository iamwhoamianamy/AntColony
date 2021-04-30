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
   class Ant : Point
   {
      public static float maxSpeed = 0.0f;
      public static float steerStrength = 0.1f;
      public static float avoidStrenght = 0.5f;

      public bool isCarryingFood = false;

      public bool isLockedOnHome = false;
      public Vector2 homeAim;

      public bool isLockedOnFood = false;
      public Cell foodAim;

      public bool isCarryingPheromone = true;
      public float pheromoneDurationLeft;
      
      public static readonly float pheromoneDuration = 0.25f;

      public int foodCount = 0;

      public Ant(float _size, Vector2 _loc, Vector2 _vel) :
         base(_size, _loc, _vel)
      {
         maxSpeed = _vel.Length;
         pheromoneDurationLeft = pheromoneDuration;
      }

      public void UpdateLocation()
      {
         vel = (vel + acc).Normalized() * maxSpeed;
         loc += vel;

         acc = Vector2.Zero;
      }

      public void Steer(Vector2 target)
      {
         Vector2 desiredDirection = (target - loc).Normalized();
         Vector2 desiredVelocity = desiredDirection * maxSpeed;
         Vector2 desiredSteeringForce = (desiredVelocity - vel) * steerStrength;
         acc += (desiredSteeringForce.Normalized() * steerStrength) / 1;
      }

      public void Steer(Vector2 target, float _steerStrength)
      {
         Vector2 desiredDirection = (target - loc).Normalized();
         Vector2 desiredVelocity = desiredDirection * maxSpeed;
         Vector2 desiredSteeringForce = (desiredVelocity - vel) * _steerStrength;
         acc += desiredSteeringForce.Normalized() * _steerStrength;
      }

      public void Avoid(Vector2 target)
      {
         Vector2 desiredDirection = (target - loc).Normalized();
         Vector2 desiredVelocity = desiredDirection * maxSpeed;
         Vector2 desiredSteeringForce = (desiredVelocity - vel) * avoidStrenght;
         acc -= desiredSteeringForce.Normalized() * avoidStrenght;
      }

      public void Wander(double r, float wanderStrength)
      {
         Vector2 desiredDirection = Misc.VecFromAng(r) * wanderStrength;
         Vector2 desiredVelocity = desiredDirection * maxSpeed;
         Vector2 desiredSteeringForce = (desiredVelocity - vel) * steerStrength;
         acc += desiredSteeringForce.Normalized() * steerStrength;
      }

      public void AvoidBorders(float perseption, float w, float h)
      {
         if (loc.Y < perseption)
         {
            if (loc.X < perseption)
               Avoid(new Vector2(-size, -size));
            else if (loc.X > w - perseption)
               Avoid(new Vector2(w + size, -size));
            else
               Avoid(new Vector2(loc.X, -size));
         }

         if (loc.Y > h - perseption)
         {
            if(loc.X < perseption)
               Avoid(new Vector2(-size, h + size));
            else if(loc.X > w - perseption)
               Avoid(new Vector2(w + size, h + size));
            else
               Avoid(new Vector2(loc.X, h + size));
         }

         if (loc.X < perseption)
         {
            if (loc.Y < perseption)
               Avoid(new Vector2(-size, -size));
            else if (loc.Y > h - perseption)
               Avoid(new Vector2(-size, h + size));
            else
               Avoid(new Vector2(-size, loc.Y));
         }

         if (loc.X > w - perseption)
         {
            if (loc.Y < perseption)
               Avoid(new Vector2(w + size, -size));
            else if (loc.Y > h - perseption)
               Avoid(new Vector2(w + size, h + size));
            else
               Avoid(new Vector2(w + size, loc.Y));
         }
      }

      public void SeekHome(Vector2 home)
      {
         if (isCarryingFood)
         {
            float dist = Vector2.Distance(home, loc);
            if (dist < size * 5.5f)
            {
               homeAim = home;
               isLockedOnHome = true;
            }
            // Ant has brought food to home
            if (dist < size * 2f)
            {
               isCarryingFood = false;
               isLockedOnHome = false;
               vel *= -1;

               pheromoneDurationLeft = Ant.pheromoneDuration;
               isCarryingPheromone = true;
            }
         }
      }
   }
}
