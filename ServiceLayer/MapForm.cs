using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tweets_Statistics.BusinessLayer;
using Tweets_Statistics.DataAccessLayer;
using Tweets_Statistics.PresentationLayer;

namespace Tweets_Statistics
{
    public partial class MapForm : Form
    {
        private ContextInitializer _contextInitializer;
        public MapForm()
        {
            InitializeComponent();
        }

        private void DrawMap(object sender, PaintEventArgs e)
        {
            Panel canvas = new Panel();
            canvas.Size = canvas.MaximumSize;
            Graphics graphics = e.Graphics;
            graphics.ScaleTransform(1, 1.5f);
            graphics.TranslateTransform(100, 100);

            int yellow = 0, blue = 0;
            foreach (var state in _contextInitializer.Context.StatesPolygonsPoints)
            {
                float sentiment = _contextInitializer.Context.StatesSentiments.Where(x => x.Key == state.Key).First().Value;
                if(sentiment == -2)
                {
                    yellow = 0;
                    blue = 0;
                }
                else
                {
                    if (sentiment == 0)
                    {
                        yellow = 255;
                        blue = 255;
                    }
                    else
                    {
                        yellow = 0;
                        blue = 255;
                        for (float i = -1; i <= 1; i += 0.1f)
                        {
                            if (i >= sentiment)
                                break;
                            yellow += 12;
                            blue -= 12;
                        }
                    }
                }
                using (Brush brush = new SolidBrush(Color.FromArgb(255, yellow, yellow, blue)))
                {
                    foreach (var polygon in state.Value)
                    {
                        graphics.FillPolygon(brush, polygon.ToArray());
                    }
                }
            }
        }
        private void MapForm_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(255, 255, 161, 176);
            this.DoubleBuffered = true;
            _contextInitializer = new ContextInitializer();
            IProcessInitializer initializer = new FormInitializer(_contextInitializer.Context);
            initializer.Start();

            this.WindowState = FormWindowState.Maximized;

            this.Paint += new PaintEventHandler(DrawMap);
            this.Refresh();
        }
    }
}
