using BOOSE;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BOOSEcode2.Tests
{
    /// <summary>
    /// Simple unit tests for the OutputInt class.
    /// </summary>
    [TestClass]
    public class OutputIntTests
    {
        private StoredProgram program;

        /// <summary>
        /// Creates a fresh BOOSE program before each test.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            var canvas = new BOOSEcode2.OutputCanvas(200, 200);
            program = new StoredProgram(canvas);
        }

        /// <summary>
        /// Checks that "int x" creates x with value 0.
        /// </summary>
        [TestMethod]
        public void DeclareInt_DefaultValueIsZero()
        {
            var cmd = new BOOSEcode2.OutputInt();
            cmd.Set(program, "x");
            cmd.Compile();
            cmd.Execute();

            Assert.AreEqual("0", program.GetVarValue("x"));
        }

        /// <summary>
        /// Checks that "int x = 10" stores the value 10.
        /// </summary>
        [TestMethod]
        public void DeclareInt_WithValue_StoresCorrectNumber()
        {
            var cmd = new BOOSEcode2.OutputInt();
            cmd.Set(program, "x = 10");
            cmd.Compile();
            cmd.Execute();

            Assert.AreEqual("10", program.GetVarValue("x"));
        }

        /// <summary>
        /// Checks that expressions like "2 + 3" are evaluated correctly.
        /// </summary>
        [TestMethod]
        public void DeclareInt_WithExpression_Works()
        {
            var cmd = new BOOSEcode2.OutputInt();
            cmd.Set(program, "x = 2 + 3");
            cmd.Compile();
            cmd.Execute();

            Assert.AreEqual("5", program.GetVarValue("x"));
        }
    }
}


