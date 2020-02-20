using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightsLib
{
    public class Light
    {
        public delegate void LightChanged(Light light);

        public event LightChanged LightChangedEvent;

        public bool IsDummy { get; private set; }
        public bool IsOn { get; private set; }
        public Point Location { get; private set; }
        private List<Light> adjacentLights = new List<Light>();
        private static Random s_Random = new Random();
        public bool JustToggled { get; set; }

        public Light(Point location, int rndPercent, bool isDummy = false)
        {
            int perCent = s_Random.Next(0, 100);
            IsOn = perCent <= rndPercent;
            IsDummy = isDummy;
            Location = location;
        }

        public void Toggle(bool propagate)
        {
            if (IsDummy)
            {
                return;
            }
            IsOn = !IsOn;
            JustToggled = true;
            if (propagate)
            {
                adjacentLights.ForEach(x => x.Toggle(propagate: false));
            }
            if (LightChangedEvent != null)
            {
                LightChangedEvent.Invoke(this);
            }
        }

        internal void SaveAdjacentLights(LightsGame game)
        {
            adjacentLights.Clear();
            Point up = new Point(Location.X, Location.Y - 1);
            adjacentLights.Add(game.GetLight(up));
            Point right = new Point(Location.X + 1, Location.Y);
            adjacentLights.Add(game.GetLight(right));
            Point down = new Point(Location.X, Location.Y + 1);
            adjacentLights.Add(game.GetLight(down));
            Point left = new Point(Location.X - 1, Location.Y);
            adjacentLights.Add(game.GetLight(left));
        }
    }
}
