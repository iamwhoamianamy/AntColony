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
      private Color4 color;

      private List<Point> points;

      public MainWindow()
      {
         InitializeComponent();
         r = new Random();
         color = new Color4(0.631f, 0.6f, 0.227f, 1f);

         points = new List<Point>();

         for (int i = 0; i < 10; i++)
         {
            points.Add(new Point(10, new Vector2((float)r.NextDouble() * 500, (float)r.NextDouble() * 500)));
         }
      }

      private void OnLoad(object sender, EventArgs e)
      {
         GL.ClearColor(color);
      }

      private void OnRenderFrame(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         int w = glControl.Width;
         int h = glControl.Height;

         GL.Disable(EnableCap.DepthTest);
         GL.Viewport(0, 0, w, h);
         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadIdentity();
         GL.Ortho(0, w, 0, h, -1.0, 1.0);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadIdentity();

         GL.Clear(ClearBufferMask.ColorBufferBit);

         // Draw objects here
         GL.ClearColor(color);

         GL.PointSize(100);

         GL.Begin(PrimitiveType.Points);

         //foreach (var p in points)
         //{
         //   GL.Color3(0.5, 0, 0);
         //   GL.Vertex2(p.loc.X, p.loc.Y);
         //}

         GL.Color3(0.5, 0, 0);
         GL.Vertex3(50, 0, 0);

         GL.End();

         glControl.SwapBuffers();
         glControl.Invalidate();
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         color = new Color4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1f);
         glControl.Invalidate();
      }

      private void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         Vector2 clickCoords = new Vector2(e.X, e.Y);
         points.Add(new Point(10, clickCoords));

         textBlock.Text = clickCoords.X.ToString() + " " + clickCoords.Y.ToString();

      }
   }
}
