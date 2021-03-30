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

      float mouseX, mouseY;

      bool doSpawn = false;
      int timeSteps = 0;

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

         base.OnLoad(e);
      }

      protected override void OnRenderFrame(FrameEventArgs e)
      {
         GL.Clear(ClearBufferMask.ColorBufferBit);

         UpdatePhysics();
         Draw();
         Title = colony.ants.Count().ToString();
         timeSteps = (timeSteps + 1) % 1000;

         Context.SwapBuffers();
         base.OnRenderFrame(e);
      }

      protected override void OnKeyDown(KeyboardKeyEventArgs e)
      {
         switch(e.Key)
         {
            case Key.Space:
            {
               doSpawn = !doSpawn;
               break;
            }
         }
         base.OnKeyDown(e);
      }

      private void Draw()
      {
         // Draw objects here
         GL.ClearColor(backgroundColor);
         colony.DrawPheromones();
         colony.DrawAnts();

         GL.PointSize(5);
         GL.Enable(EnableCap.PointSmooth);
         GL.Color3(0f, 1f, 0f);

         GL.Begin(PrimitiveType.Points);

         foreach (var f in food)
            GL.Vertex2(f.loc);

         GL.End();

         GL.Disable(EnableCap.PointSmooth);
      }

      private void UpdatePhysics()
      {
         colony.Wander(r);

         if (timeSteps % 5 == 0 && doSpawn)
            colony.ants.Add(new Ant(10, new Vector2(Width / 2, Height / 2),
               Misc.VecFromAng(r.NextDouble() * 360f)));

         if (timeSteps % 3 == 0)
            colony.UpdatePheromones();

         colony.pathPheromonesQTree = new QTree(new Vector2(Width / 2, Height / 2), new Vector2(Width, Height), 1);
         colony.pathPheromonesQTree.Fill(colony.pathPheromones.ToList<Point>());

         colony.foodPheromonesQTree = new QTree(new Vector2(Width / 2, Height / 2), new Vector2(Width, Height), 1);
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

      protected override void OnMouseDown(MouseButtonEventArgs e)
      {
         Vector2 clickCoords = new Vector2(e.X, e.Y);
         food.Add(new Point(clickCoords));

         base.OnMouseDown(e);
      }
   }

}
