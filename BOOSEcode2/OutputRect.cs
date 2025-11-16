using BOOSE;
using System;

namespace BOOSEcode2
{
    /// <summary>
    /// Draws a rectangle on the canvas with the given width and height.
    /// </summary>
    public class OutputRect : CommandTwoParameters
    {
        private int width;
        private int height;

        /// <summary>
        /// Default constructor for the factory.
        /// </summary>
        public OutputRect() : base()
        {
        }

        /// <summary>
        /// Creates the command with a canvas, width, and height.
        /// </summary>
        public OutputRect(Canvas c, int width, int height) : base(c)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Runs the rectangle command.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            width = Paramsint[0];
            height = Paramsint[1];

            if (width < 1 || height < 1)
            {
                throw new CanvasException("Rectangle width and height must be greater than 0.");
            }

            Canvas.Rect(width, height, false);
        }
    }
}

