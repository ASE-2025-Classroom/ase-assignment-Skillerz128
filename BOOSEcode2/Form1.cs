using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BOOSE;

namespace BOOSEcode2
{
    /// <summary>
    /// Main form that runs BOOSE commands and displays the drawing canvas.
    /// </summary>

    public partial class Form1 : Form
    {
        private OutputCanvas canvas;

        private TextBox txtCommands;
        private Button btnRun;
        private Button btnClear;

        /// <summary>
        /// Sets up the form, canvas, and user interface controls.
        /// </summary>


        public Form1()
        {
            InitializeComponent();

            this.Text = "Boose Drawring Canvas";
            this.MinimumSize = new Size(800, 600);
            this.DoubleBuffered = true;

            // Create the canvas (where we draw)
            canvas = new OutputCanvas(1000, 600);
            canvas.Clear();

            // Build simple UI
            CreateUi();

            // Paint event to draw the bitmap
            this.Paint += Form1_Paint;
        }

        /// <summary>
        /// Creates and positions all controls on the form, such as buttons and text boxes.
        /// </summary>


        private void CreateUi()
        {
            // Text box for commands
            txtCommands = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10),
                Left = 10,
                Top = 10,
                Width = 300,
                Height = ClientSize.Height - 60,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(txtCommands);

            // Run button
            btnRun = new Button
            {
                Text = "Run",
                Left = 10,
                Top = ClientSize.Height - 40,
                Width = 80,
                Height = 30,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnRun.Click += (s, e) => { RunCommands(); };
            this.Controls.Add(btnRun);

            // Clear button
            btnClear = new Button
            {
                Text = "Clear",
                Left = btnRun.Right + 10,
                Top = btnRun.Top,
                Width = 80,
                Height = 30,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnClear.Click += (s, e) =>
            {
                canvas.Clear();
                Invalidate();
            };
            this.Controls.Add(btnClear);

            // Make Ctrl+Enter run the program
            txtCommands.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.Enter)
                {
                    RunCommands();
                    e.SuppressKeyPress = true;
                }
            };
        }

        /// <summary>
        /// Draws the canvas bitmap onto the form whenever the window repaints.
        /// </summary>
        /// <param name="sender">The paint event sender.</param>
        /// <param name="e">Paint event data used for drawing.</param>


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bmp = (Bitmap)canvas.getBitmap();
            if (bmp == null) return;

            // Draw the canvas to the right of the textbox
            int left = txtCommands.Right + 20;
            int top = 10;
            e.Graphics.DrawImage(bmp, left, top);
        }

        /// <summary>
        /// Clears the canvas and runs all commands entered in the command window.
        /// Shows an error message if the script fails.
        /// </summary>



        // Executes all drawring commands 

        private void RunCommands()
        {
            try
            {
                canvas.Clear(); // clears the canvas with fresh pane
                ExecuteScript(txtCommands.Text);
                Invalidate();   // redraw the form
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Reads the script line by line and performs the matching drawing commands.
        /// </summary>
        /// <param name="script">The text containing all BOOSE commands.</param>

        private void ExecuteScript(string script)
        {
            var lines = script
                .Replace("\r\n", "\n")
                .Split('\n');

            foreach (var raw in lines)
            {
                string line = raw.Trim();
                if (line.Length == 0) continue;                  
                if (line.StartsWith("#") || line.StartsWith("*")) 
                    continue;

                // Split into command and arguments
                /// <summary>
                /// Converts text in the form "r,g,b" into three integers.
                /// </summary>
                /// <param name="s">The text containing three numbers.</param>
                /// <returns>Three integers representing colour values.</returns>

                int spaceIndex = line.IndexOf(' ');
                string cmd = (spaceIndex < 0 ? line : line.Substring(0, spaceIndex)).ToLowerInvariant();
                string args = (spaceIndex < 0 ? "" : line.Substring(spaceIndex + 1)).Trim();

                switch (cmd)
                {
                    case "moveto":
                        {
                            var (x, y) = ParseTwoInts(args);
                            canvas.MoveTo(x, y);
                            break;
                        }

                    case "drawto":
                        {
                            var (x, y) = ParseTwoInts(args);
                            canvas.DrawTo(x, y);
                            break;
                        }

                    case "setcolour":
                    case "colour":
                    case "pen":
                        {
                            var (r, g, b) = ParseThreeInts(args); 
                            canvas.SetColour(r, g, b);
                            break;
                        }

                    case "circle":
                        {
                            int radius = ParseOneInt(args);
                            canvas.Circle(radius, false);
                            break;
                        }

                    case "rect":
                        {
                            var (w, h) = ParseTwoInts(args);
                            canvas.Rect(w, h, false);
                            break;
                        }

                    case "tri":
                        {
                            var (w, h) = ParseTwoInts(args);
                            canvas.Tri(w, h);
                            break;
                        }

                    case "write":
                    case "text":
                        {
                            string text = args.Trim().Trim('"');
                            canvas.WriteText(text);
                            break;
                        }

                    case "clear":
                        canvas.Clear();
                        break;

                    case "reset":
                        canvas.Reset();
                        break;

                    default:
                        throw new Exception($"Unknown command: {cmd}");
                }
            }
        }

        /// <summary>
        /// Converts text into a single integer value.
        /// </summary>
        /// <param name="s">The text containing one number.</param>
        /// <returns>The parsed integer.</returns>

        private static int ParseOneInt(string s)
        {
            return int.Parse(s.Trim());
        }

        private static (int, int) ParseTwoInts(string s)
        {
            var parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(p => p.Trim())
                         .ToArray();

            if (parts.Length != 2)
                throw new Exception("Expected 2 numbers (e.g. 100,200)");

            return (int.Parse(parts[0]), int.Parse(parts[1]));
        }

        private static (int, int, int) ParseThreeInts(string s)
        {
            var parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(p => p.Trim())
                         .ToArray();

            if (parts.Length != 3)
                throw new Exception("Expected 3 numbers (e.g. 255,0,0)");

            return (int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }
    }
}

