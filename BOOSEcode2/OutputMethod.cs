using BOOSE;
using System;
using System.Collections.Generic;

namespace BOOSEcode2
{
    /// <summary>
    /// Defines a method with a return type, name, and parameters.
    /// Syntax:
    /// method returnType methodName
    /// </summary>
    public class OutputMethod : CompoundCommand, ICommand
    {
        private string methodName;
        private string returnType;

        private readonly List<string> paramNames = new List<string>();
        private readonly List<string> paramTypes = new List<string>();

        private int returnLine = -1;

        /// <summary>
        /// Creates a new method command.
        /// </summary>
        public OutputMethod() : base() { }

        /// <summary>
        /// The name of the method.
        /// </summary>
        public string MethodName => methodName;

        /// <summary>
        /// The return type of the method.
        /// </summary>
        public string Type => returnType;

        /// <summary>
        /// The parameter names used inside the method.
        /// </summary>
        public string[] LocalVariables => paramNames.ToArray();

        /// <summary>
        /// The line number where the method returns.
        /// </summary>
        public int ReturnLine
        {
            get => returnLine;
            set => returnLine = value;
        }

        /// <summary>
        /// The types of each parameter.
        /// </summary>
        public List<string> ParameterTypes => paramTypes;

        /// <summary>
        /// The names of each parameter.
        /// </summary>
        public List<string> ParameterNames => paramNames;

        /// <summary>
        /// No extra parameter checking is needed here.
        /// </summary>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Reads the method header, stores its details,
        /// and registers it with the program.
        /// </summary>
        public override void Compile()
        {
            LineNumber = Program.Count;

            string text = (ParameterList ?? "").Trim();
            if (text.Length == 0)
                throw new CanvasException("method requires: returnType methodName [parameters]");

            string[] parts = text.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                throw new CanvasException("method requires: returnType methodName [parameters]");

            returnType = parts[0].Trim();
            methodName = parts[1].Trim();

            paramNames.Clear();
            paramTypes.Clear();

            if ((parts.Length - 2) % 2 != 0)
                throw new CanvasException("method parameters must be type/name pairs");

            for (int i = 2; i < parts.Length; i += 2)
            {
                paramTypes.Add(parts[i].Trim());
                paramNames.Add(parts[i + 1].Trim());
            }

            CreateReturnVariableIfMissing();

            Program.Push(this);
        }

        /// <summary>
        /// Skips the method body when it is not being called.
        /// </summary>
        public override void Execute()
        {
            if (returnLine == -1)
                Program.PC = EndLineNumber;
        }

        /// <summary>
        /// Creates a variable for the return value if it does not already exist.
        /// </summary>
        private void CreateReturnVariableIfMissing()
        {
            if (Program.VariableExists(methodName))
                return;

            string rt = (returnType ?? "").Trim().ToLowerInvariant();

            if (rt == "int")
            {
                var v = new OutputInt();
                v.Set(Program, methodName + " = 0");
                v.Compile();
                v.Execute();
                return;
            }

            if (rt == "real")
            {
                var v = new OutputReal();
                v.Set(Program, methodName + " = 0.0");
                v.Compile();
                v.Execute();
                return;
            }

            throw new CanvasException($"Unknown method return type '{returnType}'");
        }
    }
}

