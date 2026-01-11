using BOOSE;
using System;
using System.Globalization;

namespace BOOSEcode2
{
    /// <summary>
    /// Represents a real (double) variable in the program.
    /// It can be declared and assigned using an expression.
    /// </summary>
    public class OutputReal : Evaluation, ICommand
    {
        private double doubleValue;

        /// <summary>
        /// Gets or sets the current value of the real variable.
        /// </summary>
        public double Value
        {
            get { return doubleValue; }
            set { doubleValue = value; }
        }

        /// <summary>
        /// Creates a new real variable.
        /// </summary>
        public OutputReal() : base() { }

        /// <summary>
        /// No extra parameter checks are needed here.
        /// </summary>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Reads the variable name and optional starting value,
        /// then registers the variable in the program.
        /// </summary>
        public override void Compile()
        {
            string full = (this.ParameterList ?? "").Trim();

            if (string.IsNullOrWhiteSpace(full))
                throw new CanvasException("Variable declaration requires a name");

            int eq = full.IndexOf('=');

            if (eq >= 0)
            {
                this.varName = full.Substring(0, eq).Trim();
                this.expression = full.Substring(eq + 1).Trim();
            }
            else
            {
                this.varName = full.Trim();
                this.expression = "0.0";
            }

            this.Program.AddVariable(this);

            // Make sure the variable exists as a real value straight away
            this.Program.UpdateVariable(this.varName, 0.0);
        }

        /// <summary>
        /// Calculates the value of the expression and stores it
        /// in this real variable.
        /// </summary>
        public override void Execute()
        {
            string evaluated = (this.expression ?? "0.0").Trim();

            if (evaluated.Length == 0)
                evaluated = "0.0";

            string[] tokens = evaluated.Split(new[] { ' ', '+', '-', '*', '/', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (this.Program.VariableExists(token))
                {
                    Evaluation v = this.Program.GetVariable(token);
                    string valueText;

                    if (v is OutputReal r)
                        valueText = r.Value.ToString(CultureInfo.InvariantCulture);
                    else if (v is Real br)
                        valueText = br.Value.ToString(CultureInfo.InvariantCulture);
                    else
                        valueText = (this.Program.GetVarValue(token) ?? "0").Replace(',', '.');

                    evaluated = evaluated.Replace(token, valueText);
                }
            }

            evaluated = evaluated.Replace(',', '.');

            if (double.TryParse(evaluated, NumberStyles.Any, CultureInfo.InvariantCulture, out double direct))
            {
                this.Value = direct;
                this.Program.UpdateVariable(this.varName, direct);
                return;
            }

            try
            {
                var dt = new System.Data.DataTable();
                object result = dt.Compute(evaluated, "");
                double d = Convert.ToDouble(result, CultureInfo.InvariantCulture);

                this.Value = d;
                this.Program.UpdateVariable(this.varName, d);
            }
            catch
            {
                throw new CanvasException("Cannot convert '" + this.expression + "' to real for variable '" + this.varName + "'");
            }
        }
    }
}
