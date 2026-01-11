using BOOSE;
using System;
using System.Drawing;

namespace BOOSEcode2
{
    /// <summary>
    /// Writes text or calculated values to the output canvas.
    /// </summary>
    public class OutputWrite : Evaluation, ICommand
    {
        /// <summary>
        /// The canvas used to display output.
        /// </summary>
        private static OutputCanvas canvas;

        /// <summary>
        /// Sets the canvas that will receive the output.
        /// </summary>
        public static void SetCanvas(OutputCanvas c)
        {
            canvas = c;
        }

        /// <summary>
        /// Creates a new write command.
        /// </summary>
        public OutputWrite() : base()
        {
        }

        /// <summary>
        /// Checks any parameters for the command.
        /// This command does not require parameters.
        /// </summary>
        public override void CheckParameters(string[] parameter)
        {
        }

        /// <summary>
        /// Stores the expression that will be written or calculated.
        /// </summary>
        public override void Compile()
        {
            expression = ParameterList.Trim();
        }

        /// <summary>
        /// Runs the write command by replacing variables, calculating any maths,
        /// and sending the final text to the canvas.
        /// </summary>
        public override void Execute()
        {
            string evaluated = expression.Trim();

            // Split the expression into parts so variables can be found
            string[] tokens = evaluated.Split(new[] { ' ', '+', '-', '*', '/', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (Program.VariableExists(token))
                {
                    Evaluation var = Program.GetVariable(token);
                    string value;

                    if (var is OutputReal)
                    {
                        value = ((OutputReal)var).Value.ToString();
                    }
                    else if (var is Real)
                    {
                        value = ((Real)var).Value.ToString();
                    }
                    else if (var is OutputInt)
                    {
                        value = Program.GetVarValue(token);
                    }
                    else
                    {
                        value = Program.GetVarValue(token);
                    }

                    evaluated = evaluated.Replace(token, value);
                }
            }

            // If the text contains maths,it try to calculate it
            if (evaluated.Contains("+") || evaluated.Contains("-") ||
                evaluated.Contains("*") || evaluated.Contains("/"))
            {
                try
                {
                    var dataTable = new System.Data.DataTable();
                    var result = dataTable.Compute(evaluated, "");
                    evaluated = result.ToString();
                }
                catch
                {
                    // Ignore any calculation errors
                }
            }

            // Sends the final result to the canvas
            if (canvas != null)
            {
                canvas.WriteText(evaluated);
            }
        }
    }
}


