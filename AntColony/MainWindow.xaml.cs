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
      private QTree pheromonesQTree;
      private QTree foodQTree;
      private List<Point> food;

      public int w, h;
      float mouseX, mouseY;

      int time = 0;

      public MainWindow()
      {
         InitializeComponent();
         r = new Random();
         backgroundColor = new Color4(0.4f, 0.4f, 0.4f, 1f);

         int w = glControl.Width;
         int h = glControl.Height;

         colony = new Colony();
         colony.wanderStrength = 0.25f;
         food = new List<Point>();
         
         //DispatcherTimer timer = new DispatcherTimer();
         //timer.Tick += dispatcherTimer_Tick;
         //timer.Interval = TimeSpan.FromMilliseconds(1);
         //timer.Start();
      }

      //private void dispatcherTimer_Tick(object sender, EventArgs e)
      //{
      //   time++;
      //   textBlock3.Text = time.ToString();
      //}

      private void glControl_OnLoad(object sender, EventArgs e)
      {
         GL.ClearColor(backgroundColor);
      }

      private void glControl_OnRenderFrame(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         // Dont touch
         GL.Clear(ClearBufferMask.ColorBufferBit);

         // Iplement physics here

         foreach (var ant in colony.ants)
         {
            //ant.Avoid(new Vector2(mouseX, mouseY));
            //ant.Wander(r.NextDouble(), 1f);
         }

         colony.Wander(r);


         if(time % 10 == 0)
            colony.UpdatePheromones();

         pheromonesQTree = new QTree(new Vector2(w / 2, h / 2), new Vector2(w, h), 1);
         foreach (var ant in colony.ants)
            pheromonesQTree.Fill(ant.pheromones.ToList<Point>());

         foodQTree = new QTree(new Vector2(w / 2, h / 2), new Vector2(w, h), 1);
         foodQTree.Fill(food);

         //foreach (var ant in colony.ants)
         //{
         //   List<Neighbour> neibs = new List<Neighbour>();

         //   foodQTree.Quarry(new Point(ant.loc + ant.vel.Normalized() * ant.size * 0.5f),
         //                    ant.size, neibs);

         //   if (neibs.Count != 0)
         //   {
         //      textBlock4.Text = "Нашли";
         //      ant.isCarryingFood = true;
         //      ant.isAvoiding = true;
         //      ant.avoidTarget = neibs[0].point.loc;
         //   }
         //   else
         //      textBlock4.Text = "Пока не нашли";
         //}

         colony.FollowPheromones(pheromonesQTree);

         colony.AvoidBorders(30, w, h);
         colony.BounceFromBorders(w, h);
         colony.UpdateLocation();

         //colony.qTree = new QTree(new Vector2(w / 2, h / 2), new Vector2(w, h), 1);
         //colony.qTree.Fill(colony.ants.ToList<Point>());

         //List<Neighbour> neibs = new List<Neighbour>();
         // colony.qTree.Quarry(new Point(new Vector2(mouseX, mouseY)), 100, neibs);

         //GL.LineWidth(5);
         //GL.Color3(1f, 0, 0);
         // Misc.DrawRect(mouseX, mouseY, 100, 100);


         // Draw objects here
         GL.ClearColor(backgroundColor);
         colony.DrawPheromones();
         colony.DrawAnts();

         //foreach (var ant in colony.ants)
         //{
         //   GL.LineWidth(2);
         //   Vector2 bord = new Vector2(ant.size, ant.size);
         //   Misc.DrawRect(ant.loc + ant.vel.Normalized() * ant.size * 1.75f, bord);
         //   Misc.DrawRect(ant.loc + ant.vel.Normalized() * ant.size * 0.5f, bord);
         //   Misc.DrawRect(ant.loc + Misc.RotateVector(ant.vel.Normalized() * ant.size * 1.75f, 30f * Math.PI / 180), bord);
         //   Misc.DrawRect(ant.loc + Misc.RotateVector(ant.vel.Normalized() * ant.size * 1.75f, -30f * Math.PI / 180), bord);
         //}

         GL.PointSize(5);
         GL.Enable(EnableCap.PointSmooth);
         GL.Color3(0f, 1f, 0f);

         GL.Begin(PrimitiveType.Points);

         foreach (var f in food)
            GL.Vertex2(f.loc);

         GL.End();

         GL.Disable(EnableCap.PointSmooth);

         //GL.LineWidth(3);
         //colony.qTree.Draw();

         //GL.PointSize(10);
         //GL.Color3(1f, 1f, 0);

         //GL.Begin(PrimitiveType.Points);

         //foreach (var n in neibs)
         //   GL.Vertex2(n.point.loc);

         //GL.End();

         // Dont touch
         glControl.SwapBuffers();
         glControl.Invalidate();

         time++;
         textBlock3.Text = time.ToString();
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
         colony.ants = new List<Ant>();
         for (int i = 0; i < 100; i++)
         {
            colony.ants.Add(new Ant(15, new Vector2(w / 2, h / 2), Misc.VecFromAng(r.NextDouble(), 1.1f)));
         }
      }
   }
}
