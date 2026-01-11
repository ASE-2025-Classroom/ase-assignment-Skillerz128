using BOOSE;
using System;
using System.Globalization;

namespace BOOSEcode2
{
    /// <summary>
    /// Implements the BOOSE "array" variable.
    /// Supports 1D (size) and 2D (rows,cols) arrays for int and real.
    /// Also contains shared helper code used by peek/poke.
    /// </summary>
    public class OutputArray : Evaluation, ICommand
    {
        protected const bool PEEK = false;
        protected const bool POKE = true;

        public string type;

        protected int rows;
        protected int columns;

        protected int[,] intArray;
        protected double[,] realArray;

        protected string rowS;
        protected string columnS;
        protected int row;
        protected int column;

        protected int valueInt;
        protected double valueReal;
        protected string pokeValue;
        protected string peekVar;

        protected int Rows => rows;
        protected int Columns => columns;

        public OutputArray() : base() { }

        public override void CheckParameters(string[] parameterList) { }

        /// <summary>
        /// Compiles: array type name size  OR  array type name rows,cols
        /// </summary>
        public override void Compile()
        {
            string text = (ParameterList ?? "").Trim();
            string[] parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 3)
                throw new CanvasException("Array declaration requires: type name size");

            type = parts[0].Trim().ToLowerInvariant();
            varName = parts[1].Trim();

            ParseSize(parts[2].Trim(), out rows, out columns);

            Program.AddVariable(this);
        }

        /// <summary>
        /// Allocates the underlying array storage.
        /// </summary>
        public override void Execute()
        {
            if (type == "int")
            {
                intArray = new int[rows, columns];
                realArray = null;
                return;
            }

            if (type == "real")
            {
                realArray = new double[rows, columns];
                intArray = null;
                return;
            }

            throw new CanvasException("Unknown array type: " + type);
        }

        /// <summary>
        /// Writes an int value into the array.
        /// </summary>
        public virtual void SetIntArray(int val, int r, int c)
        {
            CheckBounds(r, c);
            intArray[r, c] = val;
        }

        /// <summary>
        /// Writes a real value into the array.
        /// </summary>
        public virtual void SetRealArray(double val, int r, int c)
        {
            CheckBounds(r, c);
            realArray[r, c] = val;
        }

        /// <summary>
        /// Reads an int value from the array.
        /// </summary>
        public virtual int GetIntArray(int r, int c)
        {
            CheckBounds(r, c);
            return intArray[r, c];
        }

        /// <summary>
        /// Reads a real value from the array.
        /// </summary>
        public virtual double GetRealArray(int r, int c)
        {
            CheckBounds(r, c);
            return realArray[r, c];
        }

        protected void CheckBounds(int r, int c)
        {
            if (r < 0 || r >= rows || c < 0 || c >= columns)
                throw new CanvasException($"Array index out of bounds: [{r},{c}]");
        }

        private static void ParseSize(string text, out int r, out int c)
        {
            string[] dims = (text ?? "").Split(',');

            if (!int.TryParse(dims[0].Trim(), out r) || r < 1)
                throw new CanvasException("Array size must be a positive integer");

            if (dims.Length > 1)
            {
                if (!int.TryParse(dims[1].Trim(), out c) || c < 1)
                    throw new CanvasException("Array columns must be a positive integer");
            }
            else
            {
                c = 1;
            }
        }

        /// <summary>
        /// Used by OutputPeek/OutputPoke during Compile to split out:
        /// - array name
        /// - row/col expressions
        /// - value (for poke) or target var (for peek)
        /// </summary>
        protected virtual void ProcessArrayParametersCompile(bool peekOrPoke)
        {
            string paramList = (ParameterList ?? "").Trim();
            int eq = paramList.IndexOf('=');

            if (eq < 0)
                throw new CanvasException(peekOrPoke == POKE ? "Poke requires = sign" : "Peek requires = sign");

            if (peekOrPoke == POKE)
            {
                string left = paramList.Substring(0, eq).Trim();
                pokeValue = paramList.Substring(eq + 1).Trim();

                string[] leftParts = left.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (leftParts.Length < 2)
                    throw new CanvasException("Poke requires: arrayName row [col]");

                varName = leftParts[0].Trim();
                rowS = leftParts[1].Trim();
                columnS = (leftParts.Length > 2) ? leftParts[2].Trim() : "0";
            }
            else
            {
                peekVar = paramList.Substring(0, eq).Trim();
                string right = paramList.Substring(eq + 1).Trim();

                string[] rightParts = right.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (rightParts.Length < 2)
                    throw new CanvasException("Peek requires: arrayName row [col]");

                varName = rightParts[0].Trim();
                rowS = rightParts[1].Trim();
                columnS = (rightParts.Length > 2) ? rightParts[2].Trim() : "0";
            }
        }

        /// <summary>
        /// Used by OutputPeek/OutputPoke during Execute to evaluate row/col (and poke value).
        /// </summary>
        protected virtual void ProcessArrayParametersExecute(bool peekOrPoke)
        {
            row = EvalInt(rowS, "row");
            column = EvalInt(columnS, "column");

            if (peekOrPoke != POKE)
                return;

            if (!(Program.GetVariable(varName) is OutputArray arr))
                throw new CanvasException($"Variable '{varName}' is not an array");

            if (arr.type == "int")
                valueInt = EvalInt(pokeValue, "value");
            else
                valueReal = EvalReal(pokeValue, "value");
        }

        private int EvalInt(string text, string label)
        {
            string s = (text ?? "").Trim();

            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n))
                return n;

            try
            {
                string eval = Program.EvaluateExpression(s);
                if (int.TryParse(eval, NumberStyles.Integer, CultureInfo.InvariantCulture, out n))
                    return n;
            }
            catch { }

            throw new CanvasException($"Cannot evaluate {label} expression: {text}");
        }

        private double EvalReal(string text, string label)
        {
            string s = (text ?? "").Trim().Replace(',', '.');

            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
                return d;

            try
            {
                string eval = (Program.EvaluateExpression(text) ?? "").Trim().Replace(',', '.');
                if (double.TryParse(eval, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                    return d;
            }
            catch { }

            throw new CanvasException($"Cannot evaluate {label} expression: {text}");
        }
    }
}


