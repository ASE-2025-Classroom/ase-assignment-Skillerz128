using BOOSE;
using System.Diagnostics;

namespace BOOSEcode2
{
    public partial class Form1 : Form
    {
        OutputCanvas canvas;
        CommandFactory Factory;
        StoredProgram Program;
        IParser parser;
        public Form1()
        {
            InitializeComponent();
            Debug.WriteLine(AboutBOOSE.about());
            canvas = new OutputCanvas(640, 480);
            canvas.Circle(100, true);
            canvas.Circle(50, true);
            canvas.Rect(180, 100, true);   // Filled rectangle
            canvas.MoveTo(250, 150);
            canvas.Rect(70, 30, false);  // Outline rectangle
            Factory = new CommandFactory();
            parser = new Parser(Factory, Program);
            Program = new StoredProgram(canvas);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Bitmap b = (Bitmap) canvas.getBitmap();
            Program.Run();

            g.DrawImage(b, 0, 0);

        }
    }
}
