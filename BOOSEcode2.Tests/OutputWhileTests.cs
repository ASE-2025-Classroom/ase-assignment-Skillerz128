using BOOSE;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOOSEcode2.Tests
{
    /// <summary>
    /// Unit tests for OutputWhile.
    /// Uses variables in conditions because BOOSE EvaluateExpression
    /// does not evaluate raw "0" / "1" in this setup.
    /// </summary>
    [TestClass]
    public class OutputWhileTests
    {
        private StoredProgram program;

        /// <summary>
        /// Creates a new BOOSE program before each test.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            var canvas = new BOOSEcode2.OutputCanvas(200, 200);
            program = new StoredProgram(canvas);
        }

        /// <summary>
        /// When the condition is false, the while should jump to EndLineNumber.
        /// </summary>
        [TestMethod]
        
        public void While_FalseCondition_SetsConditionFalse()
        {
            var cond = new BOOSEcode2.OutputInt();
            cond.Set(program, "cond = 0");
            cond.Compile();
            cond.Execute();

            var w = new BOOSEcode2.OutputWhile();
            w.Set(program, "cond");
            w.Compile();

            w.EndLineNumber = 99;

            w.Execute();

            Assert.IsFalse(w.Condition);
        }

        /// <summary>
        /// When the condition is true, the while should not jump to EndLineNumber.
        /// </summary>
        [TestMethod]
        public void While_TrueCondition_DoesNotJumpToEnd()
        {
            // cond = 1
            var cond = new BOOSEcode2.OutputInt();
            cond.Set(program, "cond = 1");
            cond.Compile();
            cond.Execute();

            var w = new BOOSEcode2.OutputWhile();
            w.Set(program, "cond");
            w.Compile();

            w.EndLineNumber = 50;
            int before = program.PC;

            w.Execute();

            Assert.IsTrue(w.Condition);
            Assert.AreNotEqual(50, program.PC);
            Assert.AreEqual(before, program.PC);
        }

        /// <summary>
        /// Checks a basic comparison using variables (a > b).
        /// </summary>
        [TestMethod]
        public void While_ComparisonCondition_Works()
        {
            var a = new BOOSEcode2.OutputInt();
            a.Set(program, "a = 5");
            a.Compile();
            a.Execute();

            var b = new BOOSEcode2.OutputInt();
            b.Set(program, "b = 2");
            b.Compile();
            b.Execute();

            var w = new BOOSEcode2.OutputWhile();
            w.Set(program, "a > b");
            w.Compile();

            w.EndLineNumber = 123;

            int before = program.PC;
            w.Execute();

            Assert.IsTrue(w.Condition);
            Assert.AreEqual(before, program.PC);
        }
    }
}


