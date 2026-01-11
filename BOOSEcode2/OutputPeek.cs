using BOOSE;
using System;

namespace BOOSEcode2
{
    /// <summary>
    /// Reads a value from an array and puts it into another variable.
    /// </summary>
    public class OutputPeek : OutputArray, ICommand
    {
        /// <summary>
        /// Creates a new peek command.
        /// </summary>
        public OutputPeek() : base() { }

        /// <summary>
        /// No extra parameter checks are needed here.
        /// </summary>
        public override void CheckParameters(string[] parameter)
        {
        }

        /// <summary>
        /// Compiles the peek command by processing array parameters.
        /// </summary>
        public override void Compile()
        {
            ProcessArrayParametersCompile(PEEK);
        }

        /// <summary>
        /// Runs the peek command by reading from the array and storing the value.
        /// </summary>
        public override void Execute()
        {
            ProcessArrayParametersExecute(PEEK);

            if (!Program.VariableExists(varName))
                throw new CanvasException($"Array '{varName}' has not been declared");

            if (Program.GetVariable(varName) is not OutputArray array)
                throw new CanvasException($"Variable '{varName}' is not an array");

            if (array.type == "int")
            {
                int value = array.GetIntArray(row, column);
                Program.UpdateVariable(peekVar, value);
                return;
            }

            if (array.type == "real")
            {
                double value = array.GetRealArray(row, column);
                Evaluation targetVar = Program.GetVariable(peekVar);

                if (targetVar is OutputReal outReal)
                {
                    outReal.Value = value;
                    return;
                }

                if (targetVar is OutputReal myReal)
                {
                    myReal.Value = value;
                    return;
                }

                if (targetVar is Real real)
                {
                    real.Value = value;
                    return;
                }

                Program.UpdateVariable(peekVar, value);
                return;
            }

            throw new CanvasException($"Unsupported array type '{array.type}' for '{varName}'");
        }
    }
}

