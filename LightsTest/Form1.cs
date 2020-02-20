using LightsLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightsTest
{
    public partial class frmMain : Form
    {
        private LightsGame game = new LightsGame(5, 10);

        public frmMain()
        {
            InitializeComponent();
            pnlTop.Paint += PnlTop_Paint;
            game.LightChangedEvent += Game_LightChangedEvent;
            game.GameWonEvent += Game_GameWonEvent;
        }

        private void Game_GameWonEvent()
        {
            MessageBox.Show("You win!");
        }

        private void Game_LightChangedEvent(Light light)
        {
            pnlTop.Invalidate();
        }

        private void PnlTop_Paint(object sender, PaintEventArgs e)
        {
            game.DrawOn(e.Graphics, pnlTop.Width, pnlTop.Height);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            game.Reset();
            pnlTop.Invalidate();
        }

        private int X;
        private int Y;

        private void pnlTop_Click(object sender, EventArgs e)
        {
            Point p = game.PixelToPoint(pnlTop.Width, pnlTop.Height, X, Y);
            Debug.WriteLine($"Clicked X:{X} Y:{Y} => {p.X} {p.Y}");
            game.Toggle(p);
            pnlTop.Invalidate();
        }

        private void pnlTop_MouseDown(object sender, MouseEventArgs e)
        {
            X = e.X;
            Y = e.Y;
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            pnlTop.Invalidate();
        }
    }
}
