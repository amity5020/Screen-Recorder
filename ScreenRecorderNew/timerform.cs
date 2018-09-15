using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
namespace ScreenRecorderNew
{
    public partial class timerform : Form
    {
        public timerform()
        {
            InitializeComponent();
        }

        private void timerform_Load(object sender, EventArgs e)
        {
            this.TransparencyKey = Color.Turquoise;
            this.BackColor = Color.Turquoise;
            timer1.Start();
        }
        int count = 3;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (count == 1)
            { lblTimer.Text = "Go"; }
            else if (count == 0)
            {
                timer1.Stop(); this.Close();
            }
            else
            {
                lblTimer.Text = (count - 1).ToString();
            }
            count--;
        }

        private void timerform_Paint(object sender, PaintEventArgs e)
        {
            //var hb = new HatchBrush(HatchStyle.Percent90, this.TransparencyKey);
            //e.Graphics.FillRectangle(hb, this.DisplayRectangle);
        }
    }
}
