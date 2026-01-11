using BOOSE;

namespace BOOSEcode2
{
    /// <summary>
    /// Implements the BOOSE "else" command.
    /// Links to the matching "if" during compile, then skips the else block when needed.
    /// </summary>
    public class OutputElse : CompoundCommand, ICommand
    {
        /// <summary>
        /// Creates an else command.
        /// </summary>
        public OutputElse() : base() { }

        /// <summary>
        /// No extra parameter checks are needed for else.
        /// </summary>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Compile step:
        /// Pops the matching OutputIf from the program stack
        /// Sets where the if should jump to when false (the else line)
        /// Pushes this else onto the stack to later be matched by "end"
        /// </summary>
        public override void Compile()
        {
            LineNumber = Program.Count;

            var cmd = Program.Pop();
            if (cmd == null || cmd is not OutputIf)
                throw new CanvasException("else without matching if");

            cmd.EndLineNumber = LineNumber;

            CorrespondingCommand = cmd;
            Program.Push(this);
        }

        /// <summary>
        /// Execute step:
        /// if the program reaches "else", the if-part already ran,
        /// so jump to the end of the whole if/else block.
        /// </summary>
        public override void Execute()
        {
            Program.PC = EndLineNumber;
        }
    }
}
