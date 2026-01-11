using BOOSE;
using System;

namespace BOOSEcode2
{
    /// <summary>
    /// Handles the 'end' keyword for compound statements like if, while, for and method.
    /// </summary>
    public class OutputEnd : CompoundCommand, ICommand
    {
        private string endType;

        public OutputEnd() : base() { }

        /// <summary>
        /// End commands do not need any parameters.
        /// </summary>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Links this end command to its matching opening command.
        /// </summary>
        public override void Compile()
        {
            endType = (ParameterList ?? "").Trim().ToLowerInvariant();
            LineNumber = Program.Count - 1;

            var cmd = Program.Pop();
            if (cmd == null)
                throw new CanvasException("end without matching statement");

            CorrespondingCommand = cmd;
            cmd.EndLineNumber = LineNumber + 1;
        }

        /// <summary>
        /// Executes the correct action depending on what type of block is being ended.
        /// </summary>
        public override void Execute()
        {
            if (CorrespondingCommand is OutputIf || CorrespondingCommand is OutputElse)
                return;

            if (CorrespondingCommand is OutputWhile w)
            {
                if (w.Condition)
                    Program.PC = w.LineNumber - 1;
                return;
            }

            if (CorrespondingCommand is OutputFor f)
            {
                if (f.Condition)
                {
                    int current = int.Parse(Program.GetVarValue(f.LoopVariableName));
                    Program.UpdateVariable(f.LoopVariableName, current + f.Step);
                    Program.PC = f.LineNumber - 1;
                }
                return;
            }

            if (CorrespondingCommand is OutputMethod m)
            {
                if (m.ReturnLine != -1)
                {
                    Program.PC = m.ReturnLine;
                    m.ReturnLine = -1;
                }
                return;
            }
        }
    }
}



