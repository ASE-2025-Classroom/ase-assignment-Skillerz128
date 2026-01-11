using BOOSE;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BOOSEcode2
{
    public partial class Form1 : Form
    {
        private OutputCanvas canvas;
        private CommandFactory factory;
        private StoredProgram program;
        private IParser parser;

        private TextBox programTextBox;
        private Button runButton;
        private Button clearButton;

        public Form1()
        {
            InitializeComponent();

            Width = 1100;
            Height = 800;

            canvas = new OutputCanvas(1000, 600);
            canvas.Clear();

            OutputWrite.SetCanvas(canvas);

            factory = new OutputCommandFactory();
            program = new StoredProgram(canvas);
            parser = new OutputParser(factory, program);

            CreateEditorControls();

            Paint += Form1_Paint;
            Load += Form1_Load;

            Debug.WriteLine(AboutBOOSE.about());
        }

        public OutputCanvas Canvas => canvas;

        private void CreateEditorControls()
        {
            programTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Calibri", 10),
                Left = 10,
                Top = 10,
                Width = 300,
                Height = 650
            };
            Controls.Add(programTextBox);

            runButton = new Button
            {
                Text = "Run",
                Font = new Font("Calibri", 10),
                Left = 10,
                Top = 670,
                Width = 80,
                Height = 30
            };
            runButton.Click += RunButton_Click;
            Controls.Add(runButton);

            clearButton = new Button
            {
                Text = "Clear",
                Font = new Font("Calibri", 10),
                Left = 100,
                Top = 670,
                Width = 80,
                Height = 30
            };
            clearButton.Click += ClearButton_Click;
            Controls.Add(clearButton);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            try
            {
                canvas.Clear();
                parser.ParseProgram(programTextBox.Text);
                program.Run();
                Invalidate();
            }
            catch (BOOSE.CanvasException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "BOOSE Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unexpected error: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            programTextBox.Text = string.Empty;
            canvas.Clear();
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var bmp = canvas.getBitmap() as Bitmap;

            if (bmp != null)
            {
                e.Graphics.DrawImage(bmp, 330, 10);
            }
        }

        public void RunScript(string source)
        {
            canvas.Clear();
            parser.ParseProgram(source);
            program.Run();
        }
    }
}





