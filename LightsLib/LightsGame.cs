using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightsLib
{
    public class LightsGame: IEqualityComparer<Point>
    {
        public delegate void LightChanged(Light light);
        public delegate void GameWon();

        public event LightChanged LightChangedEvent;
        public event GameWon GameWonEvent;

        private Dictionary<Point, Light> AllLights;

        public int Size { get; private set; }

        /// <summary>
        /// Percent of lights to turn on at initialization
        /// </summary>
        public int PercentOn { get; private set; }

        public bool GameOver { get; private set; }
        public int MoveNumber { get; private set; }

        /// <summary>
        /// This is used as adjacent cells for lights at the border of the board
        /// where an adjacent cell would be outside of the board.
        /// </summary>
        private Light DummyLight;

        public LightsGame(int size, int percentOn)
        {
            AllLights = new Dictionary<Point, Light>(this);
            DummyLight = new Light(new Point(-1, -1), percentOn, isDummy: true);
            Size = size;
            PercentOn = percentOn;
            Reset();
        }

        public void Reset()
        {
            NewGame();
            //There must be at least one light on otherwise geme is not valid.
            while(NoLightIsOn())
            {
                NewGame();
            }
        }

        /// <summary>
        /// Generate a new set of lights.
        /// </summary>
        public void NewGame()
        {
            MoveNumber = 0;
            GameOver = false;
            for (int k = 0; k < Size; k++)
            {
                for (int j = 0; j < Size; j++)
                {
                    AllLights[new Point(k, j)] = new Light(new Point(k, j), PercentOn);
                    AllLights[new Point(k, j)].LightChangedEvent += LightsGame_LightChangedEvent;
                }
            }

            for (int k = 0; k < Size; k++)
            {
                for (int j = 0; j < Size; j++)
                {
                    AllLights[new Point(k, j)].SaveAdjacentLights(this);
                }
            }
        }

        /// <summary>
        /// Return true if no lights are on.
        /// </summary>
        /// <returns></returns>
        public bool NoLightIsOn()
        {
            return AllLights.All(x => !x.Value.IsOn && !x.Value.IsDummy);
        }

        /// <summary>
        /// Toggle a light cell and it's adjacent ones.
        /// </summary>
        /// <param name="p"></param>
        public bool Toggle(Point p)
        {
            if (GameOver)
            {
                return false;
            }
            foreach(var x in AllLights.Values)
            {
                x.JustToggled = false;
            }
            bool ret = GetLight(p).Toggle(propagate: true);
            if (ret)
            {
                MoveNumber++;
                GameOver = NoLightIsOn();
                if (GameOver)
                {
                    GameWonEvent?.Invoke();
                }
            }
            return ret;
        }

        /// <summary>
        /// Convert a pixel on a canvas to a Point.
        /// </summary>
        /// <param name="canvasWidth"></param>
        /// <param name="canvasHeight"></param>
        /// <param name="pixelX"></param>
        /// <param name="pixelY"></param>
        /// <returns>A Point</returns>
        public Point PixelToPoint(int canvasWidth, int canvasHeight, int pixelX, int pixelY)
        {
            int x = (int)(((decimal)pixelX / canvasWidth) * Size);
            int y = (int)(((decimal)pixelY / canvasHeight) * Size);
            return new Point(x, y);
        }

        /// <summary>
        /// Fire light changed event.
        /// </summary>
        /// <param name="light"></param>
        private void LightsGame_LightChangedEvent(Light light)
        {
            LightChangedEvent?.Invoke(light);
        }

        /// <summary>
        /// Retrieve light object
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Light GetLight(Point location)
        {
            return AllLights.ContainsKey(location) ? AllLights[location] : DummyLight;
        }

        /// <summary>
        /// Compare a point to another point
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns>True if both point to the same cell</returns>
        public bool Equals(Point point1, Point point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Unique value for each point</returns>
        public int GetHashCode(Point obj)
        {
            return obj.X + (obj.Y * Size);
        }

        /// <summary>
        /// Convert index of cell to pixel number on a ruler
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="maxPixels"></param>
        /// <returns></returns>
        private int IndexToPixel(int idx, int maxPixels)
        {   
            return (int)(idx * (decimal) maxPixels / Size) - 1;
        }

        /// <summary>
        /// Draw the game on a Graphics
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public void DrawOn(Graphics gr, int w, int h)
        {
            gr.ResetClip();
            var pen = new Pen(Color.Black, 2);
            if (GameOver)
            {
                DrawBlackBoard(gr, w, h);
                return;
            }
            DrawGrid(gr, w, h, pen);
            DrawLights(gr, w, h);
        }

        private static void DrawBlackBoard(Graphics gr, int w, int h)
        {
            //Show black board.
            gr.FillRectangle(new SolidBrush(Color.Black), new Rectangle(1, 1, w - 2, h - 2));
        }

        private void DrawLights(Graphics gr, int w, int h)
        {
            //Draw lights
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Light l1 = GetLight(new Point(x, y));
                    if (l1.IsDummy) continue;
                    if (l1.IsOn)
                    {
                        //Draw light as ON
                        int x1 = IndexToPixel(x, w);
                        int y1 = IndexToPixel(y, h);
                        int w1 = IndexToPixel(1, w);
                        int h1 = IndexToPixel(1, h);
                        gr.FillRectangle(new SolidBrush(Color.Lime), new Rectangle(
                            x1, y1, w1, h1)
                            );
                    }
                    if (l1.JustToggled)
                    {
                        //Draw light as just toggled.
                        gr.DrawString("O", new Font("Arial", 10), new SolidBrush(Color.Black), new PointF(IndexToPixel(x, w) + IndexToPixel(1, w) / 2, IndexToPixel(y, h) + IndexToPixel(1, h) / 2));
                    }
                }
            }
        }

        private void DrawGrid(Graphics gr, int w, int h, Pen pen)
        {
            //Draw border
            gr.DrawRectangle(pen, new Rectangle(1, 1, w - 2, h - 2));
            //Draw grid
            for (int num = 1; num < Size; num++)
            {
                //Vertical line
                gr.DrawLine(pen, new Point(IndexToPixel(num, w), 1), new Point(IndexToPixel(num, w), h - 1));
                //Horizontal line
                gr.DrawLine(pen, new Point(1, IndexToPixel(num, h)), new Point(w - 1, IndexToPixel(num, h)));
            }
        }
    }
}
