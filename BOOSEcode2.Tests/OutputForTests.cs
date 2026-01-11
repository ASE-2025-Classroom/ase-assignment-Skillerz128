using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSE;

namespace BOOSEcode2.Tests
{
    /// <summary>
    /// Unit tests for the OutputFor loop command.
    /// </summary>
    [TestClass]
    public class OutputForTests
    {
        /// <summary>
        /// A simple canvas used only for testing.
        /// It does nothing visually but satisfies the ICanvas interface.
        /// </summary>
        private class DummyCanvas : ICanvas
        {
            /// <summary>Current X position.</summary>
            public int Xpos { get; set; }

            /// <summary>Current Y position.</summary>
            public int Ypos { get; set; }

            /// <summary>Current pen colour.</summary>
            public object PenColour { get; set; }

            public void Circle(int radius, bool filled) { }
            public void Clear() { }
            public void DrawTo(int x, int y) { Xpos = x; Ypos = y; }
            public object getBitmap() { return new object(); }
            public void MoveTo(int x, int y) { Xpos = x; Ypos = y; }
            public void Rect(int width, int height, bool filled) { }
            public void Reset() { Xpos = 0; Ypos = 0; }
            public void Set(int width, int height) { }
            public void SetColour(int red, int green, int blue) { }
            public void Tri(int width, int height) { }
            public void WriteText(string text) { }
        }

        /// <summary>
        /// Checks that the loop sets the start value
        /// when it runs for the first time.
        /// </summary>
        [TestMethod]
        public void ForLoop_SetsStartValue_OnFirstExecute()
        {
            var program = new StoredProgram(new DummyCanvas());

            var loop = new BOOSEcode2.OutputFor();
            loop.Set(program, "i = 1 to 5");
            loop.Compile();

            loop.Execute();

            Assert.AreEqual("1", program.GetVarValue("i"));
            Assert.IsTrue(loop.Condition);
        }

        /// <summary>
        /// Checks that the loop condition becomes false
        /// when the variable goes past the end value.
        /// </summary>
        [TestMethod]
        public void ForLoop_BecomesFalse_WhenPastEnd()
        {
            var program = new StoredProgram(new DummyCanvas());

            var loop = new BOOSEcode2.OutputFor();
            loop.Set(program, "i = 1 to 2");
            loop.Compile();

            loop.Execute();                 // i becomes 1
            program.UpdateVariable("i", 3); // simulate going past the end
            loop.Execute();                 // should now stop

            Assert.IsFalse(loop.Condition);
        }

        /// <summary>
        /// Checks that the loop reads the step value
        /// and the loop variable name correctly.
        /// </summary>
        [TestMethod]
        public void ForLoop_ReadsStepValue()
        {
            var program = new StoredProgram(new DummyCanvas());

            var loop = new BOOSEcode2.OutputFor();
            loop.Set(program, "i = 0 to 10 step 2");
            loop.Compile();

            loop.Execute();

            Assert.AreEqual(2, loop.Step);
            Assert.AreEqual("i", loop.LoopVariableName);
        }
    }
}
