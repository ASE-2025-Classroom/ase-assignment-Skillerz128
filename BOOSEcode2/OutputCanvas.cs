using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOOSE;

namespace BOOSEcode2
{
    internal class OutputCanvas : ICanvas
    {
        Bitmap CanvasBitmap;
        Graphics g;
        private int xpos, ypos;
        Pen pen;

        public OutputCanvas(int xsize, int ysize)
        {
            CanvasBitmap = new Bitmap(xsize, ysize);
            g = Graphics.FromImage(CanvasBitmap);
            xpos = 100;
            ypos = 100;
            pen = new Pen(Color.Green);
        }
        public int Xpos { get => xpos; set => xpos = value; }
        public int Ypos { get => ypos; set => ypos = value; }
        public object PenColour { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Circle(int radius, bool filled)
        {
            g.DrawEllipse(pen, xpos, ypos, radius * 2, radius * 2);
        }

        public void Clear()
        {
            //throw new NotImplementedException();
        }

        public void DrawTo(int x, int y)
        {
            //throw new NotImplementedException();
        }

        public object getBitmap()
        {
            return CanvasBitmap;
        }

        public void MoveTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
        }

        public void Rect(int width, int height, bool filled)
        {
            // Draw a rectangle starting from the current position (xPos, yPos)
            if (filled)
            {
                // Draw a filled rectangle
                g.FillRectangle(new SolidBrush(pen.Color), xpos, ypos, width, height);
            }
            else
            {
                // Draw only the outline
                g.DrawRectangle(pen, xpos, ypos, width, height);
            }
        }
        public void Reset()
        {
            Xpos = 100;
            Ypos = 100;
        }

        public void Set(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void SetColour(int red, int green, int blue)
        {
            throw new NotImplementedException();
        }

        public void Tri(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void WriteText(string text)
        {
            throw new NotImplementedException();
        }
    }
}
