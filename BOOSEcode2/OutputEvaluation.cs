using BOOSE;
using System;
using System.Data;
using System.Globalization;

namespace BOOSEcode2
{
    /// <summary>
    /// Handles assignments 
    /// It evaluates the expression and stores the result in a variable.
    /// </summary>
    public class OutputEvaluation : Evaluation, ICommand
    {
        /// <summary>
        /// Creates a new evaluation command.
        /// </summary>
        public OutputEvaluation() : base()
        {
        }

        /// <summary>
        /// Reads and checks the assignment statement.
        /// </summary>
        public override void Compile()
        {
            string fullExpression = ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(fullExpression))
                throw new CanvasException("Assignment requires an expression");

            int eq = fullExpression.IndexOf('=');
            if (eq < 0)
                throw new CanvasException("Assignment must contain '='");

            varName = fullExpression.Substring(0, eq).Trim();
            expression = fullExpression.Substring(eq + 1).Trim();

            if (string.IsNullOrWhiteSpace(varName))
                throw new CanvasException("Assignment requires a variable name");

            if (!Program.VariableExists(varName))
                throw new CanvasException("Variable '" + varName + "' must be declared before assignment");
        }

        /// <summary>
        /// Runs the assignment by calculating the expression
        /// and updating the variable.
        /// </summary>
        public override void Execute()
        {
            string evaluated = EvaluateExpression(expression);
            Evaluation existingVar = Program.GetVariable(varName);

            if (existingVar is Real || existingVar is OutputReal)
            {
                if (!double.TryParse(evaluated, NumberStyles.Any, CultureInfo.InvariantCulture, out double r) &&
                    !double.TryParse(evaluated, NumberStyles.Any, CultureInfo.CurrentCulture, out r))
                {
                    throw new CanvasException("Cannot evaluate '" + expression + "' as real for variable '" + varName + "'");
                }

                Program.UpdateVariable(varName, r);
                return;
            }

            if (int.TryParse(evaluated, NumberStyles.Any, CultureInfo.InvariantCulture, out int i) ||
                int.TryParse(evaluated, NumberStyles.Any, CultureInfo.CurrentCulture, out i))
            {
                Program.UpdateVariable(varName, i);
                return;
            }

            if (double.TryParse(evaluated, NumberStyles.Any, CultureInfo.InvariantCulture, out double d) ||
                double.TryParse(evaluated, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
            {
                Program.UpdateVariable(varName, d);
                return;
            }

            throw new CanvasException("Cannot evaluate '" + expression + "' for variable '" + varName + "'");
        }

        /// <summary>
        /// Replaces variables with their values and calculates the result.
        /// </summary>
        private string EvaluateExpression(string expr)
        {
            string work = (expr ?? "").Trim();

            work = work.Replace("*", " * ")
                       .Replace("/", " / ")
                       .Replace("+", " + ")
                       .Replace("-", " - ")
                       .Replace("(", " ( ")
                       .Replace(")", " ) ");

            string[] tokens = work.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];

                if (!Program.VariableExists(token))
                    continue;

                string val = Program.GetVarValue(token);

                if (!double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out double num) &&
                    !double.TryParse(val, NumberStyles.Any, CultureInfo.CurrentCulture, out num))
                {
                    throw new CanvasException("Variable '" + token + "' is not a number");
                }

                tokens[i] = num.ToString(CultureInfo.InvariantCulture);
            }

            work = string.Join(" ", tokens);

            try
            {
                var dt = new DataTable();
                dt.Locale = CultureInfo.InvariantCulture;

                object result = dt.Compute(work, "");
                return Convert.ToString(result, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new CanvasException("Cannot evaluate expression: " + expr);
            }
        }
    }
}

