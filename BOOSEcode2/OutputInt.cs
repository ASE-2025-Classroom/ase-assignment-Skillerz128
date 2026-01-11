using BOOSE;
using System;

namespace BOOSEcode2
{
    /// <summary>
    /// Represents an integer variable declaration and assignment.
    /// </summary>
    public class OutputInt : Evaluation, ICommand
    {
        public OutputInt() : base()
        {
        }

        /// <summary>
        /// Parses the variable name and its expression.
        /// </summary>
        public override void Compile()
        {
            string text = ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new CanvasException("Variable declaration requires a name");
            }

            if (!string.IsNullOrWhiteSpace(varName))
            {
                if (text.StartsWith("="))
                {
                    text = text.Substring(1).Trim();
                }

                expression = string.IsNullOrWhiteSpace(text) ? "0" : text;
                return;
            }

            int eq = text.IndexOf('=');

            if (eq >= 0)
            {
                varName = text.Substring(0, eq).Trim();
                expression = text.Substring(eq + 1).Trim();
            }
            else
            {
                varName = text.Trim();
                expression = "0";
            }

            Program.AddVariable(this);
        }

        /// <summary>
        /// Evaluates the expression and stores the integer value.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (!int.TryParse(evaluatedExpression, out int result))
            {
                throw new CanvasException(
                    $"Cannot convert '{evaluatedExpression}' to integer for variable '{varName}'");
            }

            value = result;
            Program.UpdateVariable(varName, result);
        }
    }
}
