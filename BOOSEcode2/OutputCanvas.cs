using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BOOSE;

namespace BOOSEcode2
{
    /// <summary>
    /// Canvas used by the BOOSE commands to draw shapes onto a bitmap.
    /// </summary>
    internal class OutputCanvas : ICanvas
    {
        Bitmap CanvasBitmap;
        Graphics g;

        private int xpos;
        private int ypos;

        /// <summary>
        /// Pen used for drawing lines and outlines.
        /// </summary>
        Pen pen;

        /// <summary>
        /// Creates a new drawing canvas with the given size.
        /// </summary>
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

        /// <summary>
        /// Draws a circle using the current pen position as the centre.
        /// </summary>
        public void Circle(int radius, bool filled)
        {
            int d = radius * 2;
            int x = xpos - radius;
            int y = ypos - radius;
            g.DrawEllipse(pen, x, y, d, d);
        }

        /// <summary>
        /// Clears the canvas to white and resets the pen.
        /// </summary>
        public void Clear()
        {
            g.Clear(Color.White);
            Reset();
        }

        /// <summary>
        /// Moves the pen to the given coordinates without drawing.
        /// </summary>
        public void MoveTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
        }

        /// <summary>
        /// Draws a rectangle from the current pen position.
        /// </summary>
        public void Rect(int width, int height, bool filled)
        {
            if (filled)
            {
                g.FillRectangle(new SolidBrush(pen.Color), xpos, ypos, width, height);
            }
            else
            {
                g.DrawRectangle(pen, xpos, ypos, width, height);
            }
        }

        /// <summary>
        /// Sets the pen colour using RGB values.
        /// </summary>
        public void SetColour(int red, int green, int blue)
        {
            pen.Color = Color.FromArgb(red, green, blue);
        }

        public void DrawTo(int x, int y)
        {
            // Not implemented yet
        }

        public object getBitmap()
        {
            return CanvasBitmap;
        }

        /// <summary>
        /// Returns the pen to the default starting position.
        /// </summary>
        public void Reset()
        {
            Xpos = 100;
            Ypos = 100;
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
            throw new NotImplementedException();
        }
    }
}

