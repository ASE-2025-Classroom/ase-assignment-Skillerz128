using BOOSE;
using System;

namespace BOOSEcode2
{
    /// <summary>
    /// Puts a value into an array.
    /// Examples:
    /// poke myArray row = value
    /// poke myArray row col = value
    /// </summary>
    public class OutputPoke : OutputArray, ICommand
    {
        /// <summary>
        /// Creates a new poke command.
        /// </summary>
        public OutputPoke() : base() { }

        /// <summary>
        /// Checks parameters (no extra checks used here).
        /// </summary>
        public override void CheckParameters(string[] parameter)
        {
            // No extra validation (keeps behaviour the same)
        }

        /// <summary>
        /// Compiles the poke command by processing the array parameters.
        /// </summary>
        public override void Compile()
        {
            ProcessArrayParametersCompile(POKE);
        }

        /// <summary>
        /// Runs the poke command by working out the indexes/value and writing into the array.
        /// </summary>
        public override void Execute()
        {
            // Work out row/col and value first (same as before)
            ProcessArrayParametersExecute(POKE);

            // Make sure the array exists
            if (!Program.VariableExists(varName))
                throw new CanvasException($"Array '{varName}' has not been declared");

            // Make sure the variable is actually an array
            if (Program.GetVariable(varName) is not OutputArray array)
                throw new CanvasException($"Variable '{varName}' is not an array");

            // Store the value based on the array type
            if (array.type == "int")
            {
                array.SetIntArray(valueInt, row, column);
                return;
            }

            if (array.type == "real")
            {
                array.SetRealArray(valueReal, row, column);
                return;
            }

            throw new CanvasException($"Unsupported array type '{array.type}' for '{varName}'");
        }
    }
}

