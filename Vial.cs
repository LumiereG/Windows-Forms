using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab2
{
    public partial class Vial : UserControl
    {
        public int MaxSegments { get; set; } = 4; // Default value for the number of segments
        public List<Color> Segments { get; set; } = new List<Color>();
        public Vial()
        {
            InitializeComponent();
        }

        public void AddSegment(Color color)
        {
            if (Segments.Count < MaxSegments)
            {
                Segments.Add(color);
                this.Invalidate(); // Pererysowywanie fioletki po dodaniu segmentu
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            int vialWidth = this.Width;
            int vialHeight = this.Height;
            float segmentHeight = (float)vialHeight / MaxSegments;

            using (Pen outlinePen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(outlinePen, 0, 0, vialWidth, vialHeight);
            }

            // Rysowanie segmentów
            for (int i = 0; i < Segments.Count; i++)
            {
                SolidBrush brush;

                if (i == 0) // Jeśli to pierwszy segment, rysujemy pustą (przezroczystą) komórkę
                {
                    brush = new SolidBrush(Color.Transparent); // Używamy przezroczystego koloru
                }
                else
                {
                    // Inne segmenty rysujemy w odpowiednim kolorze
                    brush = new SolidBrush(i - 1 < Segments.Count ? Segments[i - 1] : Color.Transparent);
                }

                // Rysowanie segmentu
                g.FillRectangle(brush, 0, i * segmentHeight, vialWidth, segmentHeight);
            }
        }
    }
}
