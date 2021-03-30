using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Windows.Threading;
using System.Timers;

namespace AntColony
{
   /// <summary>
   /// Логика взаимодействия для MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private readonly Random r;
      private Color4 backgroundColor;

      private Colony colony;
      private QTree foodQTree;
      private List<Point> food;

      public int w, h;
      float mouseX, mouseY;

      bool doSpawn = false;
      int timeSteps = 0;

      public MainWindow()
      {
         InitializeComponent();
         r = new Random();
         backgroundColor = new Color4(0.4f, 0.4f, 0.4f, 1f);

         int w = glControl.Width;
         int h = glControl.Height;

         colony = new Colony();
         food = new List<Point>();
      }

      private void UpdatePhysics()
      {
         colony.Wander(new Random());

         if (timeSteps % 10 == 0 && doSpawn)
            colony.ants.Add(new Ant(5, new Vector2(w / 2, h / 2), Misc.VecFromAng(r.NextDouble(), (float)(new Random().NextDouble()) * 2f)));

         if (timeSteps % 3 == 0)
            colony.UpdatePheromones();

         colony.pathPheromonesQTree = new QTree(new Vector2(w / 2, h / 2), new Vector2(w, h), 1);
         colony.pathPheromonesQTree.Fill(colony.pathPheromones.ToList<Point>());

         colony.foodPheromonesQTree = new QTree(new Vector2(w / 2, h / 2), new Vector2(w, h), 1);
         colony.foodPheromonesQTree.Fill(colony.foodPheromones.ToList<Point>());

         foodQTree = new QTree(new Vector2(w / 2, h / 2), new Vector2(w, h), 1);
         foodQTree.Fill(food);

         colony.FollowPheromones();
         colony.SeekFood(foodQTree);
         colony.SeekHome(new Vector2(w / 2, h / 2));

         colony.AvoidBorders(30, w, h);
         colony.BounceFromBorders(w, h);
         colony.UpdateLocation();
      }

      private void glControl_OnLoad(object sender, EventArgs e)
      {
         GL.ClearColor(backgroundColor);
      }

      private void glControl_OnRenderFrame(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         // Dont touch
         GL.Clear(ClearBufferMask.ColorBufferBit);

         UpdatePhysics();
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

         // Dont touch
         glControl.SwapBuffers();
         glControl.Invalidate();
      }

      private void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         Vector2 clickCoords = new Vector2(e.X, e.Y);

         //colony.ants.Add(new Ant(10, clickCoords, new Vector2((float)r.NextDouble() * 1f, (float)r.NextDouble() * 1f)));
         food.Add(new Point(clickCoords));

         //textBlock1.Text = clickCoords.X.ToString() + " " + clickCoords.Y.ToString();
      }

      private void glControl_Resize(object sender, EventArgs e)
      {
         w = glControl.Width;
         h = glControl.Height;

         GL.Disable(EnableCap.DepthTest);
         GL.Viewport(0, 0, w, h);
         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadIdentity();
         GL.Ortho(0, w, h, 0, -1.0, 1.0);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadIdentity();
      }

      private void glControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         mouseX = e.X;
         mouseY = e.Y;

         textBlock2.Text = mouseX + " " + mouseY;
      }

      private void textBox_TextChanged(object sender, TextChangedEventArgs e)
      {
         float f = 0;
         if (float.TryParse(textBox.Text, out f))
         {
            colony.wanderStrength = f;
            textBlock1.Text = f.ToString();
         }
      }

      private void Button1_Click(object sender, RoutedEventArgs e)
      {
         backgroundColor = new Color4((float)(r.NextDouble() * 0.5), (float)(r.NextDouble() * 0.5), (float)(r.NextDouble() * 0.5), 1f);
         glControl.Invalidate();
      }

      private void Button2_Click(object sender, RoutedEventArgs e)
      {
         //colony.ants = new List<Ant>();
         //for (int i = 0; i < 100; i++)
         //{
         //   colony.ants.Add(new Ant(5, new Vector2(w / 2, h / 2), Misc.VecFromAng(r.NextDouble(), (float)r.NextDouble() * 2f)));
         //}

         doSpawn = !doSpawn;
      }

      private void Button3_Click(object sender, RoutedEventArgs e)
      {
         food = new List<Point>();
      }
   }
}
