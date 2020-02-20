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
        public int PercentOn { get; private set; }

        public bool GameOver { get; private set; }

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
            CreateNewSetOfLights();
            while(NoLightIsOn())
            {
                CreateNewSetOfLights();
            }
        }

        public void CreateNewSetOfLights()
        {
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

        public bool NoLightIsOn()
        {
            foreach(var x in AllLights)
            {
                if (x.Value.IsOn && !x.Value.IsDummy)
                {
                    return false;
                }
            }            
            return true;
        }

        public void Toggle(Point p)
        {
            if (GameOver)
            {
                return;
            }
            foreach(var x in AllLights)
            {
                x.Value.JustToggled = false;
            }
            GetLight(p).Toggle(propagate: true);
            GameOver = NoLightIsOn();
            if (GameOver && GameWonEvent != null)
            {
                GameWonEvent.Invoke();
            }
        }

        public Point PixelToPoint(int w, int h, int x1, int y1)
        {
            int x = (int)(((decimal)x1 / w) * Size);
            int y = (int)(((decimal)y1 / h) * Size);
            return new Point(x, y);
        }

        private void LightsGame_LightChangedEvent(Light light)
        {
            if (LightChangedEvent != null)
            {
                LightChangedEvent.Invoke(light);
            }
        }

        public Light GetLight(Point location)
        {
            if (AllLights.ContainsKey(location))
            {
                return AllLights[location];
            }
            return DummyLight;
        }

        public bool Equals(Point x, Point y)
        {
            return x.X == y.X && x.Y == y.Y;
        }

        public int GetHashCode(Point obj)
        {
            return obj.X + (obj.Y * Size);
        }

        private int IndexToPixel(int x, int max)
        {   
            return (int)(x * (decimal) max / Size) - 1;
        }

        public void DrawOn(Graphics gr, int w, int h)
        {
            gr.ResetClip();
            var pen = new Pen(Color.Black, 2);
            if (GameOver)
            {
                gr.FillRectangle(new SolidBrush(Color.Black), new Rectangle(1, 1, w - 2, h - 2));
                return;
            }
            //Draw border
            gr.DrawRectangle(pen, new Rectangle(1, 1, w - 2, h - 2));
            //Draw grid
            for(int num = 1; num < Size; num++)
            {
                //Vertical line
                gr.DrawLine(pen, new Point(IndexToPixel(num, w), 1), new Point(IndexToPixel(num, w), h - 1));
                //Horizontal line
                gr.DrawLine(pen, new Point(1, IndexToPixel(num, h)), new Point(w - 1, IndexToPixel(num, h)));
            }
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
    }
}
