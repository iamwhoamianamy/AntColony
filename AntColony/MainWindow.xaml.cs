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
      public MainWindow()
      {
         InitializeComponent();
         r = new Random();
         color = new Color4(0.631f, 0.6f, 0.227f, 1f);
      }

      private void OnLoad(object sender, EventArgs e)
      {
         GL.ClearColor(color);
      }

      private void OnRenderFrame(object sender, System.Windows.Forms.PaintEventArgs e)
      {
         GL.Viewport(0, 0, glControl.Width, glControl.Height);

         GL.Clear(ClearBufferMask.ColorBufferBit);

         // Draw objects here

         //GL.ClearColor(new Color4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1f));
         //GL.ClearColor(new Color4(1f, 0f, 0f, 1f));

         GL.ClearColor(color);


         glControl.SwapBuffers();
         glControl.Invalidate();
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         color = new Color4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), 1f);
         glControl.Invalidate();
      }
   }
}
