using BOOSE;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace BOOSEcode2
{
    /// <summary>
    /// Calls a method that was defined using "method".
    /// </summary>
    public class OutputCall : CompoundCommand, ICommand
    {
        private string targetName;
        private readonly List<string> args = new List<string>();

        /// <summary>
        /// Creates a call command.
        /// </summary>
        public OutputCall() : base() { }

        /// <summary>
        /// No extra checks used here.
        /// </summary>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Reads the method name and argument list from the command line.
        /// </summary>
        public override void Compile()
        {
            LineNumber = Program.Count;

            args.Clear();

            string p = (ParameterList ?? "").Trim();
            string[] parts = p.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 1)
                throw new CanvasException("Call requires: methodName [arguments]");

            targetName = parts[0].Trim();

            for (int i = 1; i < parts.Length; i++)
                args.Add(parts[i].Trim());
        }

        /// <summary>
        /// Creates the parameter variables and jumps to the method.
        /// </summary>
        public override void Execute()
        {
            OutputMethod method = GetMethod(targetName);
            if (method == null)
                throw new CanvasException("Method '" + targetName + "' not found");

            string[] paramNames = method.LocalVariables;

            if (args.Count != paramNames.Length)
                throw new CanvasException(
                    "Method '" + targetName + "' expects " + paramNames.Length +
                    " arguments, got " + args.Count);

            for (int i = 0; i < args.Count; i++)
            {
                string valueText = ResolveArgument(args[i]);

                string pName = paramNames[i];
                string pType = method.ParameterTypes[i].Trim().ToLowerInvariant();

                if (pType == "int")
                {
                    var v = new OutputInt();
                    v.Set(Program, pName + " = " + valueText);
                    v.Compile();
                    v.Execute();
                }
                else if (pType == "real")
                {
                    var v = new OutputReal();
                    v.Set(Program, pName + " = " + valueText);
                    v.Compile();
                    v.Execute();
                }
            }

            method.ReturnLine = LineNumber;

            // Jump to the method start
            Program.PC = method.LineNumber;
        }

        /// <summary>
        /// Returns a method definition from the stored program.
        /// </summary>
        private OutputMethod GetMethod(string name)
        {
            for (int i = 0; i < Program.Count; i++)
            {
                if (Program[i] is OutputMethod m)
                {
                    if (string.Equals(m.MethodName, name, StringComparison.OrdinalIgnoreCase))
                        return m;
                }
            }
            return null;
        }

        /// <summary>
        /// Converts a literal or expression into a value string to pass to a parameter.
        /// </summary>
        private string ResolveArgument(string raw)
        {
            string s = (raw ?? "").Trim();

            if (int.TryParse(s, out _))
                return s;

            if (double.TryParse(s, out _))
                return s;

            return Program.EvaluateExpression(s);
        }
    }
}
