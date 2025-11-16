using BOOSE;
using System;

namespace BOOSEcode2
{
    /// <summary>
    /// Moves the canvas cursor to a new X and Y position without drawing.
    /// </summary>
    public class OutputMoveTo : CommandTwoParameters
    {
        private int x;
        private int y;

        /// <summary>
        /// Empty constructor used by the factory.
        /// </summary>
        public OutputMoveTo() : base()
        {
        }

        /// <summary>
        /// Constructor for setting the canvas and target position manually.
        /// </summary>
        public OutputMoveTo(Canvas c, int x, int y) : base(c)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Runs the moveto command and updates the canvas cursor.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            x = Paramsint[0];
            y = Paramsint[1];

            if (x < 0 || y < 0)
            {
                throw new CanvasException("MoveTo coordinates must be positive");
            }

            Canvas.MoveTo(x, y);
        }
    }
}

