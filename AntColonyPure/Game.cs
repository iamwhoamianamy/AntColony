using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace AntColonyPure
{
   class Game : GameWindow
   {
      private Random r;
      private Color4 backgroundColor;

      private Colony colony;
      private QTree foodQTree;
      private List<Point> food;

      bool doSpawn = false;
      int timeSteps = 0;

      float mouseX = 0;
      float mouseY = 0;

      float scalingFactor = 1f;
      Vector2 translationVector;
      public Game(int width, int height, string title) :
         base(width, height, GraphicsMode.Default, title)
      {
        
      }

      protected override void OnLoad(EventArgs e)
      {
         backgroundColor = new Color4(0.4f, 0.4f, 0.4f, 1f);
         GL.ClearColor(backgroundColor);
         colony = new Colony();
         food = new List<Point>();
         r = new Random();
         translationVector = Vector2.Zero;

         //scalePlace = new Vector2(Width / 2, Height / 2);

         base.OnLoad(e);
      }

      protected override void OnRenderFrame(FrameEventArgs e)
      {
         GL.Clear(ClearBufferMask.ColorBufferBit);

         UpdatePhysics();
         Draw();

         //Title = colony.ants.Count().ToString();
         Title = scalingFactor.ToString();
         timeSteps = (timeSteps + 1) % 1000;

         Context.SwapBuffers();
         base.OnRenderFrame(e);
      }

      private void Draw()
      {
         // Draw objects here
         GL.ClearColor(backgroundColor);
         colony.DrawPheromones(scalingFactor);
         colony.DrawAnts(scalingFactor);

         // Drawing plants
         GL.PointSize(5 * scalingFactor);
         GL.Enable(EnableCap.PointSmooth);
         GL.Color3(0f, 1f, 0f);

         GL.Begin(PrimitiveType.Points);

         foreach (var f in food)
            GL.Vertex2(f.loc);

         GL.End();
         GL.Disable(EnableCap.PointSmooth);


         GL.PointSize(20f);

         GL.Vertex2(Width / 4, Height / 4);
         GL.Vertex2(3 * Width / 4, Height / 4);
         GL.Vertex2(Width / 4, 3 * Height / 4);
         GL.Vertex2(3 * Width / 4, 3 * Height / 4);

         GL.Disable(EnableCap.PointSmooth);



         ///////////////////////////////////////////////////
         //QTree qTree = new QTree(new Vector2(Width / 2, Height / 2), new Vector2(Width, Height), 2);

         //List<Point> points = new List<Point>();
         //qTree.Fill(colony.pathPheromones.ToList<Point>());
         //qTree.QuarryLimited(new Point(new Vector2(mouseX, mouseY)), 100, points, 20);

         //GL.Color3(1f, 1f, 0f);
         //GL.PointSize(10f);

         //GL.Begin(PrimitiveType.Points);

         //foreach (var p in points)
         //   GL.Vertex2(p.loc);

         //GL.End();

         // Matrix manipulations
         //GL.LoadIdentity();
      }

      private void UpdatePhysics()
      {
         colony.Wander(r);

         if (timeSteps % 5 == 0 && doSpawn)
            colony.ants.Add(new Ant(10, new Vector2(Width / 2, Height / 2),
               Misc.VecFromAng(r.NextDouble() * 360f)));

         if (timeSteps % 9 == 0)
            colony.UpdatePheromones();

         colony.pathPheromonesQTree = new QTree(new Vector2(Width / 2, Height / 2), new Vector2(Width, Height), 4);
         colony.pathPheromonesQTree.Fill(colony.pathPheromones.ToList<Point>());

         colony.foodPheromonesQTree = new QTree(new Vector2(Width / 2, Height / 2), new Vector2(Width, Height), 4);
         colony.foodPheromonesQTree.Fill(colony.foodPheromones.ToList<Point>());

         foodQTree = new QTree(new Vector2(Width / 2, Height / 2), new Vector2(Width, Height), 1);
         foodQTree.Fill(food);

         colony.FollowPheromones();
         colony.SeekFood(foodQTree);
         colony.SeekHome(new Vector2(Width / 2, Height / 2));
         
         colony.AvoidBorders(30, Width, Height);
         colony.BounceFromBorders(Width, Height);
         colony.UpdateLocation();
      }

      protected override void OnResize(EventArgs e)
      {
         GL.Disable(EnableCap.DepthTest);
         GL.Viewport(0, 0, Width, Height);
         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadIdentity();
         GL.Ortho(0, Width, Height, 0, -1.0, 1.0);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadIdentity();

         base.OnResize(e);
      }

      protected override void OnKeyDown(KeyboardKeyEventArgs e)
      {
         switch (e.Key)
         {
            case Key.Space:
            {
               doSpawn = !doSpawn;
               break;
            }
            case Key.Enter:
            {
               translationVector.X = 0f;
               translationVector.Y = 0f;
               scalingFactor = 1f;

               GL.LoadIdentity();
               break;
            }
         }
         base.OnKeyDown(e);
      }

      protected override void OnMouseDown(MouseButtonEventArgs e)
      {
         switch(e.Button)
         {
            case MouseButton.Left:
            {
               Vector2 clickCoords = new Vector2(e.X, e.Y);
               //food.Add(new Point(new Vector2(Width, Height) * scalingFactor / 2f - 
               //                   new Vector2(Width, Height) / 2f));

               clickCoords -= new Vector2(Width / 2, Height / 2);
               clickCoords /= scalingFactor;
               clickCoords += translationVector;
               clickCoords += new Vector2(Width / 2, Height / 2);

               food.Add(new Point(clickCoords));
               break;
            }
         }

         base.OnMouseDown(e);
      }

      protected override void OnMouseMove(MouseMoveEventArgs e)
      {
         if (e.Mouse.IsButtonDown(MouseButton.Middle))
         {
            GL.Translate(e.XDelta, e.YDelta, 0);
            translationVector -= new Vector2(e.XDelta, e.YDelta);
         }

         mouseX = e.X;
         mouseY = e.Y;
          
         base.OnMouseMove(e);
      }

      protected override void OnMouseWheel(MouseWheelEventArgs e)
      {
         scalingFactor *= 1 + e.Delta * 0.05f;
         float fac = 1 + e.Delta * 0.05f;

         GL.Translate(Width / 2f + translationVector.X, Height / 2f + translationVector.Y, 0);
         GL.Scale(fac, fac, fac);
         GL.Translate(-Width / 2f - translationVector.X, -Height / 2f - translationVector.Y, 0);

         base.OnMouseWheel(e);
      }
   }

}
