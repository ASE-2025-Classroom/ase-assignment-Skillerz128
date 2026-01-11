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

            // Check if we have a real value for radius (if radius is 0, then it could be a real variable)
            if (this.Parameters.Length > 0)
            {
                string paramStr = this.Parameters[0];  

                if (this.Program.VariableExists(paramStr))
                {
                    Evaluation var = this.Program.GetVariable(paramStr);

                    // Check if it's a real value and convert it to int
                    if (var is OutputReal)
                    {
                        radius = (int)Math.Round(((OutputReal)var).Value);  // Round the real value to an integer
                    }
                    else if (var is Real)
                    {
                        radius = (int)Math.Round(((Real)var).Value);  // Round the real value to an integer
                    }
                    else
                    {
                        // If it's a different type, fallback to Paramsint
                        radius = Paramsint[0];
                    }
                }
                else
                {
                    // If it's not a variable, assume it's an integer or real constant
                    if (double.TryParse(paramStr, out double realValue))
                    {
                        radius = (int)Math.Round(realValue);
                    }
                    else
                    {
                        // Default to Paramsint if it’s an integer
                        radius = Paramsint[0];
                    }
                }
            }
            else
            {
                
                radius = Paramsint[0];
            }

            // If radius is less than 1, throw an error
            if (radius < 1)
            {
                throw new CanvasException("Circle radius must be greater than 0.");
            }

            // Call the Canvas method to draw the circle with the computed radius
            Canvas.Circle(radius, false);
        }
    }
}
