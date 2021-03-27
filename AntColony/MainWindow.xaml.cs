using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

      int w, h;
      float mouseX, mouseY;

      public MainWindow()
      {
         InitializeComponent();
         r = new Random();
         backgroundColor = new Color4(0.631f, 0.6f, 0.227f, 1f);

         int w = glControl.Width;
         int h = glControl.Height;

         colony = new Colony();
      }

      private void glControl_OnLoad(object sender, EventArgs e)
      {
         GL.ClearColor(backgroundColor);
      }

      private void glControl_OnRenderFrame(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         // Dont touch
         GL.Clear(ClearBufferMask.ColorBufferBit);

         // Iplement physics here

         colony.qTree = new QTree(new Vector2(w / 2, h / 2), new Vector2(w, h), 1);
         colony.qTree.Fill(colony.ants.ToList<Point>());

         List<Neighbour> neibs = new List<Neighbour>();
         colony.qTree.Quarry(new Point(new Vector2(mouseX, mouseY)), 100, neibs);

         GL.LineWidth(5);
         GL.Color3(1f, 0, 0);
         Misc.DrawRect(mouseX, mouseY, 100, 100);

         colony.BounceFromBorders(w, h);
         colony.Update();


         // Draw objects here
         GL.ClearColor(backgroundColor);
         colony.Draw();
         GL.LineWidth(3);
         colony.qTree.Draw();

         GL.PointSize(10);
         GL.Color3(1f, 1f, 0);

         GL.Begin(PrimitiveType.Points);

         foreach (var n in neibs)
            GL.Vertex2(n.point.loc);

         GL.End();

         // Dont touch
         glControl.SwapBuffers();
         glControl.Invalidate();
      }

      private void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         Vector2 clickCoords = new Vector2(e.X, e.Y);
         colony.ants.Add(new Ant(10, clickCoords, new Vector2((float)r.NextDouble(), (float)r.NextDouble())));

         textBlock1.Text = clickCoords.X.ToString() + " " + clickCoords.Y.ToString();
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

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         backgroundColor = new Color4((float)(r.NextDouble() * 0.5), (float)(r.NextDouble() * 0.5), (float)(r.NextDouble() * 0.5), 1f);
         glControl.Invalidate();
      }
   }
}
