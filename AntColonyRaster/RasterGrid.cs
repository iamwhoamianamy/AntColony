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
   // Rasterisation grid class
   class RasterGrid
   {
      public Color4 backgroundColor;
      public Cell[][] grid;
      private int _resolution = 0;

      private float _cellW, _cellH;
      private float _screenW, _screenH;

      private int _minRes = 2;
      private int _maxRes = 500;

      public RasterGrid(int resolution, float screenW, float screenH, Color4 basicColor)
      {
         this.backgroundColor = basicColor;
         SetScreenSize(screenW, screenH);
         AddResolution(Math.Min(Math.Max(resolution, _minRes), _maxRes));
      }

      // Adding resolution to current resolution
      public void AddResolution(int resolution)
      {
         int newResolution = _resolution + resolution;

         if (newResolution >= _minRes && newResolution <= _maxRes)
         {
            _resolution = newResolution;
            grid = new Cell[newResolution][];

            for (int i = 0; i < newResolution; i++)
               grid[i] = new Cell[newResolution];

            RecalcCellSize();
            ResetGrid();
         }
      }

      public void ResetGrid()
      {
         for (int i = 0; i < _resolution; i++)
            for (int j = 0; j < _resolution; j++)
               grid[i][j] = new Cell(new Vector2(_cellH * (i + 1) - _cellH / 2, _cellW * (j + 1) - _cellW / 2));
      }

      public void ResetAnts()
      {
         for (int i = 0; i < _resolution; i++)
         {
            for (int j = 0; j < _resolution; j++)
            {
               grid[i][j].isCarryingAnt = false;
               grid[i][j].color = Color4.Black;
            }
         }
      }

      public void SetScreenSize(float screenW, float screenH)
      {
         _screenW = screenW;
         _screenH = screenH;
      }

      private void RecalcCellSize()
      {
         _cellW = _screenW / _resolution;
         _cellH = _screenH / _resolution;
      }

      public void DrawBorders(float lineWidth)
      {
         GL.LineWidth(lineWidth);
         GL.Begin(BeginMode.Lines);

         for (int i = 0; i < _resolution - 1; i++)
         {
            GL.Vertex2(_cellH * (i + 1), 0);
            GL.Vertex2(_cellH * (i + 1),_screenW);
         }

         for (int i = 0; i < _resolution - 1; i++)
         {
            GL.Vertex2(0, _cellW * (i + 1));
            GL.Vertex2(_screenH, _cellW * (i + 1));
         }

         GL.End();
      }

      public void DrawCells()
      {
         GL.PointSize(_cellW);

         for (int i = 0; i < _resolution; i++)
         {
            for (int j = 0; j < _resolution; j++)
            {
               //grid[i][j].Draw(_cellW * (j + 1) - _cellW / 2, _cellH * (i + 1) - _cellH / 2);
               grid[i][j].Draw();
            }
         }
      }

      public void RasterAnts(List<Ant> ants)
      {
         for (int i = 0; i < ants.Count; i++)
         {
            int xi = (int)Math.Floor(ants[i].loc.X / _cellW);
            int yi = (int)Math.Floor(ants[i].loc.Y / _cellH);

            grid[xi][yi].isCarryingAnt = true;
         }
      }

      public void RasterPheromones(List<Ant> ants)
      {
         for (int i = 0; i < ants.Count; i++)
         {
            int xi = (int)Math.Floor(ants[i].loc.X / _cellW);
            int yi = (int)Math.Floor(ants[i].loc.Y / _cellH);

            if (!ants[i].isCarryingFood)
            {
               grid[xi][yi].isCarryingHomePher = true;
               grid[xi][yi].homePherSat = Math.Max((float)Math.Min(ants[i].pheromoneDurationLeft + grid[xi][yi].homePherSat, grid[xi][yi].maxPherSat), grid[xi][yi].homePherSat);

               ants[i].pheromoneDurationLeft -= 0.1f;
            }
            else
            {
               grid[xi][yi].isCarryingFoodPher = true;
               grid[xi][yi].foodPherSat = Math.Max((float)Math.Min(ants[i].pheromoneDurationLeft + grid[xi][yi].foodPherSat, grid[xi][yi].maxPherSat), grid[xi][yi].foodPherSat);

               ants[i].pheromoneDurationLeft -= 0.1f;
            }
         }
      }

      public void UpdatePheromones()
      {
         for (int i = 0; i < _resolution; i++)
         {
            for (int j = 0; j < _resolution; j++)
            {
               grid[i][j].foodPherSat -= 0.1f;
               grid[i][j].homePherSat -= 0.1f;

               if (grid[i][j].homePherSat <= 0f)
                   grid[i][j].isCarryingHomePher = false;

               if (grid[i][j].foodPherSat <= 0f)
                  grid[i][j].isCarryingFoodPher = false;
            }
         }
      }

      public void PerformBehaviour(List<Ant> ants)
      {
         float fwidth = 30;
         int width = (int)(fwidth / _cellW);
         if (width % 2 != 1)
            width++;

         foreach (var ant in ants)
         {
            if (!ant.isLockedOnHome && !ant.isLockedOnFood)
            {
               Vector2 cent1 = ant.loc + ant.vel.Normalized() * fwidth * 1.2f;
               int xCent1 = (int)(cent1.X / _cellW);
               int yCent1 = (int)(cent1.Y / _cellH);

               Vector2 cent2 = ant.loc + ant.vel.Normalized() * fwidth * 0.8f + new Vector2(-ant.vel.Y, ant.vel.X).Normalized() * fwidth * 1.2f;
               int xCent2 = (int)(cent2.X / _cellW);
               int yCent2 = (int)(cent2.Y / _cellH);

               Vector2 cent3 = ant.loc + ant.vel.Normalized() * fwidth * 0.8f - new Vector2(-ant.vel.Y, ant.vel.X).Normalized() * fwidth * 1.2f;
               int xCent3 = (int)(cent3.X / _cellW);
               int yCent3 = (int)(cent3.Y / _cellH);

               Vector2 toSteerForPher = Vector2.Zero;

               if (ant.isCarryingFood)
               {
                  int foundPheromones = 0;

                  if (xCent1 > width / 2 && xCent1 < _resolution - width / 2 &&
                      yCent1 > width / 2 && yCent1 < _resolution - width / 2)
                  {
                     for (int i = 0; i < width; i++)
                     {
                        int x = xCent1 - width / 2 + i;

                        for (int j = 0; j < width; j++)
                        {
                           int y = yCent1 - width / 2 + j;

                           if (grid[x][y].isCarryingHomePher)
                           {
                              foundPheromones++;
                              toSteerForPher.X += grid[x][y].pos.X;
                              toSteerForPher.Y += grid[x][y].pos.Y;
                           }
                        }
                     }
                  }

                  if (xCent2 > width / 2 && xCent2 < _resolution - width / 2 &&
                      yCent2 > width / 2 && yCent2 < _resolution - width / 2)
                  {
                     for (int i = 0; i < width; i++)
                     {
                        int x = xCent2 - width / 2 + i;

                        for (int j = 0; j < width; j++)
                        {
                           int y = yCent2 - width / 2 + j;

                           if (grid[x][y].isCarryingHomePher)
                           {
                              foundPheromones++;
                              toSteerForPher.X += grid[x][y].pos.X;
                              toSteerForPher.Y += grid[x][y].pos.Y;
                           }
                        }
                     }
                  }

                  if (xCent3 > width / 2 && xCent3 < _resolution - width / 2 &&
                      yCent3 > width / 2 && yCent3 < _resolution - width / 2)
                  {
                     for (int i = 0; i < width; i++)
                     {
                        int x = xCent3 - width / 2 + i;

                        for (int j = 0; j < width; j++)
                        {
                           int y = yCent3 - width / 2 + j;

                           if (grid[x][y].isCarryingHomePher)
                           {
                              foundPheromones++;
                              toSteerForPher.X += grid[x][y].pos.X;
                              toSteerForPher.Y += grid[x][y].pos.Y;
                           }
                        }
                     }
                  }

                  if (foundPheromones != 0)
                  {
                     toSteerForPher /= foundPheromones;
                     ant.Steer(toSteerForPher, 0.5f);
                  }
               }
               else
               {
                  int foundPheromones = 0;
                  bool didFindFood = false;

                  if (xCent1 > width && xCent1 < _resolution - width &&
                      yCent1 > width && yCent1 < _resolution - width)
                  {
                     for (int i = 0; i < width && !didFindFood; i++)
                     {
                        int x = xCent1 - width / 2 + i;
                        for (int j = 0; j < width && !didFindFood; j++)
                        {
                           int y = yCent1 - width / 2 + j;

                           if (grid[x][y].isCarryingFood)
                           {
                              didFindFood = true;
                              ant.isLockedOnFood = true;
                              ant.foodAim = new Vector2(grid[x][y].pos.X, grid[x][y].pos.Y);
                           }
                           else
                           {
                              if (grid[x][y].isCarryingFoodPher)
                              {
                                 foundPheromones++;
                                 toSteerForPher.X += grid[x][y].pos.X;
                                 toSteerForPher.Y += grid[x][y].pos.Y;
                              }
                           }

                           //grid[x][y].color = Color4.Yellow;
                        }
                     }
                  }

                  if (xCent2 > width && xCent2 < _resolution - width &&
                      yCent2 > width && yCent2 < _resolution - width)
                  {
                     for (int i = 0; i < width && !didFindFood; i++)
                     {
                        int x = xCent2 - width / 2 + i;
                        for (int j = 0; j < width && !didFindFood; j++)
                        {
                           int y = yCent2 - width / 2 + j;

                           if (grid[x][y].isCarryingFood)
                           {
                              didFindFood = true;
                              ant.isLockedOnFood = true;
                              ant.foodAim = new Vector2(grid[x][y].pos.X, grid[x][y].pos.Y);
                           }
                           else
                           {
                              if (grid[x][y].isCarryingFoodPher)
                              {
                                 foundPheromones++;
                                 toSteerForPher.X += grid[x][y].pos.X;
                                 toSteerForPher.Y += grid[x][y].pos.Y;
                              }
                           }

                           //grid[x][y].color = Color4.Yellow;
                        }
                     }
                  }

                  if (xCent3 > width && xCent3 < _resolution - width &&
                      yCent3 > width && yCent3 < _resolution - width)
                  {
                     for (int i = 0; i < width && !didFindFood; i++)
                     {
                        int x = xCent3 - width / 2 + i;
                        for (int j = 0; j < width && !didFindFood; j++)
                        {
                           int y = yCent3 - width / 2 + j;

                           if (grid[x][y].isCarryingFood)
                           {
                              didFindFood = true;
                              ant.isLockedOnFood = true;
                              ant.foodAim = new Vector2(grid[x][y].pos.X, grid[x][y].pos.Y);
                           }
                           else
                           {
                              if (grid[x][y].isCarryingFoodPher)
                              {
                                 foundPheromones++;
                                 toSteerForPher.X += grid[x][y].pos.X;
                                 toSteerForPher.Y += grid[x][y].pos.Y;
                              }
                           }

                           //grid[x][y].color = Color4.Yellow;
                        }
                     }
                  }

                  if (foundPheromones != 0 && !didFindFood)
                  {
                     toSteerForPher /= foundPheromones;
                     ant.Steer(toSteerForPher, 0.5f);
                  }
               }
            }
         }
      }

      public void EatFood(List<Ant> ants)
      {
         float fwidth = 30;
         int width = (int)Math.Max(1, Math.Min(fwidth / _cellW, 5));

         foreach (var ant in ants)
         {
            if (!ant.isCarryingFood)
            {
               Vector2 cent = ant.loc;
               int xCent = (int)(cent.X / _cellW);
               int yCent = (int)(cent.Y / _cellH);
               bool didFindFood = false;

               if (xCent > width / 2 && xCent < _resolution - width / 2 &&
                   yCent > width / 2 && yCent < _resolution - width / 2)
                  for (int i = 0; i < width && !didFindFood; i++)
                  {
                     int x = xCent - width / 2 + i;

                     for (int j = 0; j < width && !didFindFood; j++)
                     {
                        int y = yCent - width / 2 + j;

                        if (grid[x][y].isCarryingFood)
                        {
                           grid[x][y].foodSaturation -= 0.05f;

                           if (grid[x][y].foodSaturation <= 0)
                              grid[x][y].isCarryingFood = false;

                           ant.vel *= -1;
                           ant.isCarryingFood = true;
                           ant.isLockedOnFood = false;
                           ant.pheromoneDurationLeft = ant.pheromoneDuration;
                           didFindFood = true;
                        }
                        //grid[x][y].color = Color4.Yellow;
                     }
                  }
            }
         }
      }

      public void AddFood(Vector2 loc)
      {
         int x_i = (int)(loc.X / _cellW);
         int y_i = (int)(loc.Y / _cellH);

         grid[x_i][y_i].foodSaturation = 1f;
         grid[x_i][y_i].isCarryingFood = true;
      }
   }
}