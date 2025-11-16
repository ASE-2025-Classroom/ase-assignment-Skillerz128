using BOOSE;
using System;

namespace BOOSEcode2
{
    /// <summary>
    /// Draws a circle on the canvas using the given radius.
    /// </summary>
    public class OutputCircle : CommandOneParameter
    {
        private int radius;

        /// <summary>
        /// Default constructor for the factory.
        /// </summary>
        public OutputCircle() : base()
        {
        }

        /// <summary>
        /// Creates the command with a canvas and radius.
        /// </summary>
        public OutputCircle(Canvas c, int radius) : base(c)
        {
            this.radius = radius;
        }

        /// <summary>
        /// Runs the circle command.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            radius = Paramsint[0];

            if (radius < 1)
            {
                throw new CanvasException("Circle radius must be greater than 0.");
            }

            Canvas.Circle(radius, false);
        }
    }
}
