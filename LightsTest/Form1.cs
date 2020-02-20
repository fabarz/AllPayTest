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
        private LightsGame game;

        public frmMain()
        {
            InitializeComponent();

            Reset();

            pnlTop.Paint += PnlTop_Paint;
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
            Reset();
        }

        private void Reset()
        {
            if (game != null)
            {
                game.LightChangedEvent -= Game_LightChangedEvent;
                game.GameWonEvent -= Game_GameWonEvent;
            }
            game = new LightsGame((int)numericUpDown1.Value, 40);
            game.LightChangedEvent += Game_LightChangedEvent;
            game.GameWonEvent += Game_GameWonEvent;
            lblMoveNumer.Text = game.MoveNumber.ToString();
            pnlTop.Invalidate();
        }

        private int X;
        private int Y;

        private void pnlTop_Click(object sender, EventArgs e)
        {
            Point p = game.PixelToPoint(pnlTop.Width, pnlTop.Height, X, Y);
            Debug.WriteLine($"Clicked X:{X} Y:{Y} => {p.X} {p.Y}");
            game.Toggle(p);
            lblMoveNumer.Text = game.MoveNumber.ToString();
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
