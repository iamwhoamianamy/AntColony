using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace AntColonyRaster
{
   class Game : GameWindow
   {
      private Random r;
      private Color4 backgroundColor;

      private Colony colony;
      private List<Point> food;

      bool doSpawn = false;
      int timeSteps = 0;

      float mouseX = 0;
      float mouseY = 0;

      RasterGrid rasterGrid;
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
         rasterGrid = new RasterGrid(200, Width, Height, Color4.LightGray);

         base.OnLoad(e);
      }

      protected override void OnRenderFrame(FrameEventArgs e)
      {
         GL.Clear(ClearBufferMask.ColorBufferBit);

         UpdatePhysics();
         Draw();

         timeSteps = (timeSteps + 1) % 1000;

         Context.SwapBuffers();
         base.OnRenderFrame(e);
      }

      private void Draw()
      {
         // Raster grid drawing
         rasterGrid.DrawCells();
         //GL.Color3(0.5f, 0.5f, 0.5f);
         //rasterGrid.DrawBorders(0.5f);

         // Ants drawing
         //colony.DrawAnts();

         GL.Color3(1f, 1f, 1f);
         GL.PointSize(30f);
         GL.Enable(EnableCap.PointSmooth);
         GL.Begin(BeginMode.Points);
         GL.Vertex2(Width / 2, Height / 2);
         GL.End();
         GL.Disable(EnableCap.PointSmooth);

      }

      private void UpdatePhysics()
      {
         // Ants spawning
         if (timeSteps % 5 == 0 && doSpawn)
            colony.ants.Add(new Ant(10, new Vector2(Width / 2, Height / 2),
               Misc.VecFromAng(r.NextDouble() * 360f)));

         // Updating ants

         colony.PerhormBehaviour(r, Width, Height);

         // Rasterising data
         rasterGrid.ResetAnts();
         rasterGrid.UpdatePheromones();
         rasterGrid.RasterPheromones(colony.ants);
         rasterGrid.RasterAntsAndFood(colony.ants, food);
         rasterGrid.PerformBehaviour(colony.ants, food);
         rasterGrid.EatFood(colony.ants, food);
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

         rasterGrid.SetScreenSize(Width, Height);

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
            case Key.Plus:
            {
               rasterGrid.AddResolution(4);
               break;
            }
            case Key.Minus:
            {
               rasterGrid.AddResolution(-4);
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
               food.Add(new Point(clickCoords));
               break;
            }
         }

         base.OnMouseDown(e);
      }

      protected override void OnMouseMove(MouseMoveEventArgs e)
      {
         if (e.Mouse.IsButtonDown(MouseButton.Left))
         {
            Vector2 clickCoords = new Vector2(e.X, e.Y);
            food.Add(new Point(clickCoords));
         }

         mouseX = e.X;
         mouseY = e.Y;
          
         base.OnMouseMove(e);
      }

      protected override void OnMouseWheel(MouseWheelEventArgs e)
      {

         base.OnMouseWheel(e);
      }
   }

}
