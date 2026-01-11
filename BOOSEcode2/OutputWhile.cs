using BOOSE;
using System;
using System.Globalization;

namespace BOOSEcode2
{
    /// <summary>
    /// Handles a WHILE loop by checking a condition and controlling when the loop stops.
    /// </summary>
    public class OutputWhile : CompoundCommand, ICommand
    {
        /// <summary>
        /// Creates a new while command.
        /// </summary>
        public OutputWhile() : base() { }

        /// <summary>
        /// Checks any parameters given to the command.
        /// This command does not need parameters.
        /// </summary>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Stores the condition and registers this command in the program.
        /// </summary>
        public override void Compile()
        {
            expression = (ParameterList ?? string.Empty).Trim();
            LineNumber = Program.Count;
            Program.Push(this);
        }

        /// <summary>
        /// Runs the while command by checking if the condition is true or false.
        /// If it is false, the program jumps to the end of the loop.
        /// </summary>
        public override void Execute()
        {
            string eval = Program.EvaluateExpression(expression);
            bool ok = IsTrue(eval);

            Condition = ok;

            if (!ok)
            {
                Program.PC = EndLineNumber;
            }
        }

        /// <summary>
        /// Turns a piece of text into a true or false value.
        /// It understands words, numbers, and simple comparisons.
        /// </summary>
        private bool IsTrue(string text)
        {
            text = (text ?? string.Empty).Trim();

            if (text.Equals("true", StringComparison.OrdinalIgnoreCase)) return true;
            if (text.Equals("false", StringComparison.OrdinalIgnoreCase)) return false;

            if (text == "1") return true;
            if (text == "0") return false;

            if (TryCompare(text, ">=", out bool r)) return r;
            if (TryCompare(text, "<=", out r)) return r;
            if (TryCompare(text, "==", out r)) return r;
            if (TryCompare(text, "!=", out r)) return r;
            if (TryCompare(text, ">", out r)) return r;
            if (TryCompare(text, "<", out r)) return r;

            if (double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double num))
                return num != 0;

            return false;
        }

        /// <summary>
        /// Checks if a comparison like "5 > 3" or "x == y" is true.
        /// </summary>
        private bool TryCompare(string text, string op, out bool result)
        {
            int idx = text.IndexOf(op, StringComparison.Ordinal);
            if (idx < 0)
            {
                result = false;
                return false;
            }

            string leftExpr = text.Substring(0, idx).Trim();
            string rightExpr = text.Substring(idx + op.Length).Trim();

            double left = ToNumber(Program.EvaluateExpression(leftExpr));
            double right = ToNumber(Program.EvaluateExpression(rightExpr));

            double diff = Math.Abs(left - right);

            switch (op)
            {
                case ">=": result = left >= right; return true;
                case "<=": result = left <= right; return true;
                case "==": result = diff < 0.0001; return true;
                case "!=": result = diff >= 0.0001; return true;
                case ">": result = left > right; return true;
                case "<": result = left < right; return true;
            }

            result = false;
            return false;
        }

        /// <summary>
        /// Converts a string into a number.
        /// Returns 0 if it cannot be converted.
        /// </summary>
        private static double ToNumber(string s)
        {
            s = (s ?? string.Empty).Trim().Replace(',', '.');

            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double v))
                return v;

            return 0;
        }
    }
}

