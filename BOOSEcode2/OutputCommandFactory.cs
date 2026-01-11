using BOOSE;
using System;

namespace BOOSEcode2
{
    internal class OutputCommandFactory : CommandFactory
    {
        public override ICommand MakeCommand(string commandType)
        {
            if (commandType == null) return base.MakeCommand(commandType);

            commandType = commandType.ToLowerInvariant();

            if (commandType == "circle") return new OutputCircle();
            if (commandType == "moveto") return new OutputMoveTo();
            if (commandType == "rect") return new OutputRect();

            if (commandType == "int") return new OutputInt();
            if (commandType == "real") return new OutputReal();

            if (commandType == "array") return new OutputArray();
            if (commandType == "poke") return new OutputPoke();
            if (commandType == "peek") return new OutputPeek();

            if (commandType == "write") return new OutputWrite();
            if (commandType == "=") return new OutputEvaluation();

            if (commandType == "if") return new OutputIf();
            if (commandType == "else") return new OutputElse();
            if (commandType == "end") return new OutputEnd();

            if (commandType == "while") return new OutputWhile();
            if (commandType == "for") return new OutputFor();

            if (commandType == "method") return new OutputMethod();
            if (commandType == "call") return new OutputCall();

            return base.MakeCommand(commandType);
        }
    }
}




