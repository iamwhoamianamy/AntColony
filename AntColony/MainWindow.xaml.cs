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

         //List<Point> toFillQTree = new List<Point>();

         //foreach (Ant ant in colony.ants)
         //{
         //   toFillQTree.Add(ant);
         //}

         //colony.qTree.Fill(toFillQTree);

         colony.qTree.Fill(colony.ants.ToList<Point>());
         List<Neighbour> neibs = new List<Neighbour>();

         //Vector2 mousePos = new Vector2((float)Mouse.GetPosition(LayoutRoot).X + w,
         //                               (float)Mouse.GetPosition(LayoutRoot).Y);

         //colony.qTree.Quarry(new Point(mousePos), 100, neibs);

         //Misc.DrawRect(mousePos, new Vector2(100, 100));

         

         colony.BounceFromBorders(w, h);
         colony.Update();


         // Draw objects here
         GL.ClearColor(backgroundColor);
         colony.Draw();
         colony.qTree.Draw();


         // Dont touch
         glControl.SwapBuffers();
         glControl.Invalidate();
      }

      private void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         Vector2 clickCoords = new Vector2(e.X, e.Y);
         colony.ants.Add(new Ant(10, clickCoords, new Vector2((float)r.NextDouble(), (float)r.NextDouble())));

         textBlock.Text = clickCoords.X.ToString() + " " + clickCoords.Y.ToString();
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

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         backgroundColor = new Color4((float)(r.NextDouble() * 0.5), (float)(r.NextDouble() * 0.5), (float)(r.NextDouble() * 0.5), 1f);
         glControl.Invalidate();
      }
   }
}
