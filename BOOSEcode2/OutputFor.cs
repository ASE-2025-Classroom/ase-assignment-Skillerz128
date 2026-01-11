using BOOSE;
using System;

namespace BOOSEcode2
{
    /// <summary>
    /// Implements a "for" loop command (e.g. for i = start to end step n).
    /// </summary>
    public class OutputFor : ConditionalCommand, ICommand
    {
        private string name;
        private string startExpr;
        private string endExpr;
        private string stepExpr;

        private int startVal;
        private int endVal;
        private int stepVal;

        private bool started;

        public OutputFor() : base()
        {
            started = false;
        }

        /// <summary>
        /// For commands do not require separate parameter validation here.
        /// </summary>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Parses the for statement and stores the loop details.
        /// </summary>
        public override void Compile()
        {
            LineNumber = Program.Count;

            string s = (ParameterList ?? "").Trim();
            if (s.Length == 0) throw new CanvasException("for requires parameters");

            var t = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            int eq = Find(t, "=");
            int to = Find(t, "to");
            if (eq <= 0 || to < 0 || eq + 1 >= t.Length || to + 1 >= t.Length)
                throw new CanvasException("for syntax: var = start to end [step step]");

            name = t[0].Trim();
            startExpr = t[eq + 1].Trim();
            endExpr = t[to + 1].Trim();

            int step = Find(t, "step");
            stepExpr = (step >= 0 && step + 1 < t.Length) ? t[step + 1].Trim() : "1";

            Program.Push(this);
        }

        /// <summary>
        /// Runs the loop logic and checks whether the loop should continue.
        /// </summary>
        public override void Execute()
        {
            EnsureInt(name);

            startVal = AsInt(startExpr);
            endVal = AsInt(endExpr);
            stepVal = AsInt(stepExpr);

            if (stepVal == 0) throw new CanvasException("for step cannot be zero");

            if (!started)
            {
                Program.UpdateVariable(name, startVal);
                started = true;
            }

            int current = AsInt(name);

            Condition = stepVal > 0 ? current <= endVal : current >= endVal;

            if (!Condition)
            {
                started = false;
                Program.PC = EndLineNumber;
            }
        }

        /// <summary>
        /// Ensures the loop variable exists and is an integer.
        /// </summary>
        private void EnsureInt(string varName)
        {
            if (Program.VariableExists(varName)) return;

            var v = new OutputInt();
            v.Set(Program, varName + " = 0");
            v.Compile();
            v.Execute();
        }

        /// <summary>
        /// Converts a value, variable, or expression into an integer.
        /// </summary>
        private int AsInt(string x)
        {
            x = (x ?? "").Trim();

            if (int.TryParse(x, out int n)) return n;

            if (Program.VariableExists(x))
            {
                string v = Program.GetVarValue(x);
                if (int.TryParse(v, out n)) return n;
            }

            string eval = Program.EvaluateExpression(x);
            if (int.TryParse(eval, out n)) return n;

            throw new CanvasException("for values must be integers");
        }

        /// <summary>
        /// Finds the position of a token in the parameter list.
        /// </summary>
        private int Find(string[] t, string token)
        {
            for (int i = 0; i < t.Length; i++)
                if (t[i].Equals(token, StringComparison.OrdinalIgnoreCase))
                    return i;
            return -1;
        }

        public int Step => stepVal;
        public string LoopVariableName => name;
    }
}
