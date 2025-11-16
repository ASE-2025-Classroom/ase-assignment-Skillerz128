using BOOSE;
using System;

namespace BOOSEcode2
{
    /// <summary>
    /// Creates output commands for the custom canvas.
    /// </summary>
    internal class OutputCommandFactory : CommandFactory
    {
        /// <summary>
        /// Returns a command object for the given command name.
        /// </summary>
        /// <param name="commandType">The name of the command.</param>
        /// <returns>A command that matches the name, or the base command if not found.</returns>
        public override ICommand MakeCommand(string commandType)
        {
            commandType = commandType.ToLowerInvariant();

            if (commandType == "circle")
            {
                return new OutputCircle();
            }

            if (commandType == "moveto")
            {
                return new OutputMoveTo();
            }

            if (commandType == "rect")
            {
                return new OutputRect();
            }

            return base.MakeCommand(commandType);
        }
    }
}


