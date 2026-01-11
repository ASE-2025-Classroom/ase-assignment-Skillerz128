using BOOSE;
using System;
using System.Globalization;

namespace BOOSEcode2
{
    /// <summary>
    /// Handles an IF statement by checking a condition
    /// and controlling whether the block runs.
    /// </summary>
    public class OutputIf : CompoundCommand, ICommand
    {
        /// <summary>
        /// Creates a new IF command.
        /// </summary>
        public OutputIf() : base() { }

        /// <summary>
        /// This command does not need extra parameter checks.
        /// </summary>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Stores the condition and registers this IF command
        /// with the program.
        /// </summary>
        public override void Compile()
        {
            expression = (ParameterList ?? "").Trim();
            LineNumber = Program.Count;
            Program.Push(this);
        }

        /// <summary>
        /// Runs the IF statement by checking the condition.
        /// If it is false, execution jumps to the end of the IF block.
        /// </summary>
        public override void Execute()
        {
            string eval = Program.EvaluateExpression(expression);
            bool ok = IsTrue(eval);

            Condition = ok;

            if (!ok)
                Program.PC = EndLineNumber;
        }

        /// <summary>
        /// Turns a string into true or false.
        /// Supports words, numbers, and comparisons.
        /// </summary>
        private bool IsTrue(string text)
        {
            text = (text ?? "").Trim();

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
        /// Checks a comparison like "a &gt; b" or "x == y".
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

            switch (op)
            {
                case ">=": result = left >= right; return true;
                case "<=": result = left <= right; return true;
                case "==": result = Math.Abs(left - right) < 0.0001; return true;
                case "!=": result = Math.Abs(left - right) >= 0.0001; return true;
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
        private double ToNumber(string s)
        {
            s = (s ?? "").Trim().Replace(',', '.');

            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double v))
                return v;

            return 0;
        }
    }
}
