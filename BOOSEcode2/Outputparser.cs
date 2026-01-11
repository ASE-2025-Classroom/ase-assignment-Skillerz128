using BOOSE;
using System;

namespace BOOSEcode2
{
    /// <summary>
    /// Parses lines and creates the right command objects for this language.
    /// </summary>
    public class OutputParser : Parser
    {
        /// <summary>
        /// The program we are building commands for.
        /// </summary>
        private readonly StoredProgram storedProgram;

        /// <summary>
        /// Creates a parser using the command factory and stored program.
        /// </summary>
        public OutputParser(CommandFactory factory, StoredProgram program)
            : base(factory, program)
        {
            storedProgram = program;
        }

        /// <summary>
        /// Converts one line of text into a command (or returns null for blank lines).
        /// </summary>
        public override ICommand ParseCommand(string line)
        {
            string trimmed = (line ?? string.Empty).Trim();

            // Remove UTF-8 BOM if it appears at the start (often on the first line).
            trimmed = trimmed.TrimStart('\uFEFF');

            if (string.IsNullOrWhiteSpace(trimmed))
                return null;

            // Small helper to avoid repeating Set + Compile.
            T Make<T>(string args) where T : Command, new()
            {
                var cmd = new T();
                cmd.Set(storedProgram, args);
                cmd.Compile();
                return cmd;
            }

            if (trimmed.StartsWith("int ", StringComparison.OrdinalIgnoreCase))
                return Make<OutputInt>(trimmed.Substring(4).Trim());

            if (trimmed.StartsWith("real ", StringComparison.OrdinalIgnoreCase))
                return Make<OutputReal>(trimmed.Substring(5).Trim());

            if (trimmed.StartsWith("method ", StringComparison.OrdinalIgnoreCase))
                return Make<OutputMethod>(trimmed.Substring(7).Trim());

            if (trimmed.StartsWith("call ", StringComparison.OrdinalIgnoreCase))
                return Make<OutputCall>(trimmed.Substring(5).Trim());

            // Don't treat compound lines as assignments even if they contain '='
            if (trimmed.StartsWith("for ", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("while ", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("if ", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("else", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("end ", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("end", StringComparison.OrdinalIgnoreCase))
            {
                return base.ParseCommand(trimmed);
            }

            // Assignment: x = expression (only if x already exists)
            int eq = trimmed.IndexOf('=');
            if (eq > 0)
            {
                string left = trimmed.Substring(0, eq).Trim();

                if (storedProgram.VariableExists(left))
                {
                    var e = new OutputEvaluation();
                    e.Set(storedProgram, trimmed);
                    e.Compile();
                    return e;
                }
            }

            return base.ParseCommand(trimmed);
        }
    }
}



