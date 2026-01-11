using System;
using System.Drawing;
using BOOSE;

namespace BOOSEcode2
{
    public class OutputCanvas : ICanvas
    {
        private readonly Bitmap CanvasBitmap;
        private readonly Graphics g;

        private int xpos;
        private int ypos;

        private readonly Pen pen;

        private int textY;

        public OutputCanvas(int xsize, int ysize)
        {
            CanvasBitmap = new Bitmap(xsize, ysize);
            g = Graphics.FromImage(CanvasBitmap);

            pen = new Pen(Color.Green);

            xpos = 0;
            ypos = 0;

            textY = 10;
            g.Clear(Color.LightGray);
        }

        public int Xpos { get => xpos; set => xpos = value; }
        public int Ypos { get => ypos; set => ypos = value; }

        public object PenColour
        {
            get => pen.Color;
            set
            {
                if (value is Color c)
                    pen.Color = c;
            }
        }

        public void Circle(int radius, bool filled)
        {
            int d = radius * 2;
            int x = xpos - radius;
            int y = ypos - radius;

            if (filled)
                g.FillEllipse(new SolidBrush(pen.Color), x, y, d, d);
            else
                g.DrawEllipse(pen, x, y, d, d);
        }

        public void Clear()
        {
            g.Clear(Color.LightGray);
            Reset();
            textY = 10;
        }

        public void MoveTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
        }

        public void Rect(int width, int height, bool filled)
        {
            if (filled)
                g.FillRectangle(new SolidBrush(pen.Color), xpos, ypos, width, height);
            else
                g.DrawRectangle(pen, xpos, ypos, width, height);
        }

        public void SetColour(int red, int green, int blue)
        {
            pen.Color = Color.FromArgb(red, green, blue);
        }

        public void DrawTo(int x, int y)
        {
            g.DrawLine(pen, Xpos, Ypos, x, y);
            Xpos = x;
            Ypos = y;
        }

        public object getBitmap()
        {
            return CanvasBitmap;
        }

        public void Reset()
        {
            Xpos = 0;
            Ypos = 0;
        }

        public void Set(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void Tri(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void WriteText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            using (var font = new Font("Arial", 16))
            using (var brush = new SolidBrush(pen.Color))
            {
                // Measure the text size
                SizeF size = g.MeasureString(text, font);

                // Center text on current pen position
                float drawX = Xpos - (size.Width / 2);
                float drawY = Ypos - (size.Height / 2);

                g.DrawString(text, font, brush, drawX, drawY);
            }
        }
    }
}



