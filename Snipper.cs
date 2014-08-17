using NFU.Properties;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace NFU
{
    public class Snipper : Form
    {
        public static bool isActive;

        private static Bitmap FSB;
        private static int offsetX, offsetY;

        private Mode mode;
        private Image image;
        private Cursor cursor;
        private Point pointStart;
        private Rectangle rectangle;
        private Rectangle rectangleSelection;
        private ComboBox comboBoxScreen = new ComboBox();

        /// <summary>
        /// Snips the image.
        /// </summary>
        /// <returns>The image or null on failure.</returns>
        public static Image Snip()
        {
            isActive = true;

            Program.CoreForm.Opacity = 0;

            Rectangle FSR = SystemInformation.VirtualScreen;
            Bitmap bmp = new Bitmap(FSR.Width, FSR.Height);
            offsetX = (FSR.X < 0) ? FSR.X * -1 : 0;
            offsetY = (FSR.Y < 0) ? FSR.Y * -1 : 0;

            using (Graphics gr = Graphics.FromImage(bmp)) gr.CopyFromScreen(FSR.X, FSR.Y, 0, 0, bmp.Size);

            FSB = bmp;

            using (Snipper snipper = new Snipper())
            {
                DialogResult result = snipper.ShowDialog();
                Program.CoreForm.Opacity = 1;
                isActive = false;

                if (result == DialogResult.OK) return snipper.image;
            }

            return null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private Snipper()
        {
            ShowInTaskbar = false;
            DoubleBuffered = true;
            Cursor = Cursors.Arrow;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;

            comboBoxScreen.Items.Add("Merge screens");
            comboBoxScreen.Location = new Point(3, 3);
            comboBoxScreen.DropDownStyle = ComboBoxStyle.DropDownList;

            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                comboBoxScreen.Items.Add(String.Format("Screen {0}", i + 1));
            }

            comboBoxScreen.SelectedIndexChanged += cc_SelectedIndexChanged;
            comboBoxScreen.SelectedIndex = Settings.Default.Screen;
            comboBoxScreen.Visible = Settings.Default.ShowControls;

            this.Controls.Add(comboBoxScreen);

            Misc.SetForegroundWindow(Handle);
        }

        /// <summary>
        /// Handles changes in the screen selection.
        /// </summary>
        void cc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxScreen.SelectedIndex == 0) rectangle = SystemInformation.VirtualScreen;
            else rectangle = Screen.AllScreens[comboBoxScreen.SelectedIndex - 1].Bounds;

            Size = rectangle.Size;
            Location = rectangle.Location;

            Bitmap bmp = new Bitmap(rectangle.Width, rectangle.Height);

            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(FSB, new Rectangle(0, 0, bmp.Width, bmp.Height), new Rectangle(rectangle.X + offsetX, rectangle.Y + offsetY, rectangle.Width, rectangle.Height), GraphicsUnit.Pixel);
            }

            BackgroundImage = bmp;
        }

        enum Mode { New, X, Width, Y, Height };

        /// <summary>
        /// Get the current mode based on the position of the cursor.
        /// </summary>
        /// <returns>The current mode.</returns>
        private Mode getMode(MouseEventArgs e)
        {
            // Always return Mode.New if QuickScreenshots
            // is enabled
            if (Settings.Default.QuickScreenshots)
                return Mode.New;

            // Margin to create a virtual border of 5 px
            // 2px both sides + 1px real border
            int size = 5;
            int margin = 2;

            if (new Rectangle(rectangleSelection.X - margin, rectangleSelection.Y, size, rectangleSelection.Height).Contains(e.Location))
                return Mode.X;

            if (new Rectangle(rectangleSelection.X + rectangleSelection.Width - margin, rectangleSelection.Y, size, rectangleSelection.Height).Contains(e.Location))
                return Mode.Width;

            if (new Rectangle(rectangleSelection.X, rectangleSelection.Y - margin, rectangleSelection.Width, size).Contains(e.Location))
                return Mode.Y;

            if (new Rectangle(rectangleSelection.X, rectangleSelection.Y + rectangleSelection.Height - margin, rectangleSelection.Width, size).Contains(e.Location))
                return Mode.Height;

            return Mode.New;
        }

        /// <summary>
        /// Sets the current mode.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            mode = getMode(e);

            pointStart = e.Location;
        }

        /// <summary>
        /// Sets the cursor for the current mode.
        /// Also creates the rectangle of the current selection.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                switch (getMode(e))
                {
                    case Mode.X:
                    case Mode.Width:
                        cursor = Cursors.SizeWE;
                        break;

                    case Mode.Y:
                    case Mode.Height:
                        cursor = Cursors.SizeNS;
                        break;

                    case Mode.New:
                        cursor = Cursors.Arrow;
                        break;
                }

                Cursor.Current = cursor;
                return;
            }

            // Set the cursor on every move to prevent flickering
            Cursor.Current = cursor;

            int x = 0, y = 0, w = 0, h = 0;

            switch (mode)
            {
                case Mode.X:
                    x = e.X;
                    y = rectangleSelection.Y;
                    w = rectangleSelection.Width + (rectangleSelection.X - e.X);
                    h = rectangleSelection.Height;
                    break;

                case Mode.Width:
                    x = rectangleSelection.X;
                    y = rectangleSelection.Y;
                    w = e.X - rectangleSelection.X;
                    h = rectangleSelection.Height;
                    break;

                case Mode.Y:
                    x = rectangleSelection.X;
                    y = e.Y;
                    w = rectangleSelection.Width;
                    h = rectangleSelection.Height + (rectangleSelection.Y - e.Y);
                    break;

                case Mode.Height:
                    x = rectangleSelection.X;
                    y = rectangleSelection.Y;
                    w = rectangleSelection.Width;
                    h = e.Y - rectangleSelection.Y;
                    break;

                case Mode.New:
                    x = Math.Min(e.X, pointStart.X);
                    y = Math.Min(e.Y, pointStart.Y);
                    w = Math.Max(e.X, pointStart.X) - x;
                    h = Math.Max(e.Y, pointStart.Y) - y;
                    break;
            }

            if (w <= 10 || h <= 10)
                return;

            rectangleSelection = new Rectangle(x, y, w, h);
            Invalidate();
        }

        /// <summary>
        /// Creates an image of the selection.
        /// Also confirms if QuickScreenshots is enabled.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (rectangleSelection.Width <= 0 || rectangleSelection.Height <= 0) return;
            image = new Bitmap(rectangleSelection.Width, rectangleSelection.Height);
            using (Graphics gr = Graphics.FromImage(image))
            {
                gr.DrawImage(BackgroundImage, new Rectangle(0, 0, image.Width, image.Height), rectangleSelection, GraphicsUnit.Pixel);
            }

            if (Settings.Default.QuickScreenshots)
                DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Paints the rectangle.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            using (Brush br = new SolidBrush(Color.FromArgb(120, Color.White)))
            {
                int x1 = rectangleSelection.X;
                int x2 = rectangleSelection.X + rectangleSelection.Width;
                int y1 = rectangleSelection.Y;
                int y2 = rectangleSelection.Y + rectangleSelection.Height;
                e.Graphics.FillRectangle(br, new Rectangle(0, 0, x1, Height));
                e.Graphics.FillRectangle(br, new Rectangle(x2, 0, Width - x2, Height));
                e.Graphics.FillRectangle(br, new Rectangle(x1, 0, x2 - x1, y1));
                e.Graphics.FillRectangle(br, new Rectangle(x1, y2, x2 - x1, Height - y2));
            }
            using (Pen pen = new Pen(Color.Red, 1))
            {
                e.Graphics.DrawRectangle(pen, rectangleSelection);
            }
            using (Brush bg = new SolidBrush(Color.FromArgb(130, Color.Black)))
            using (Brush fg = new SolidBrush(Color.White))
            {
                if (!Settings.Default.ShowControls)
                    return;

                Rectangle rectangleDisplay = new Rectangle(0, 0, Width, 27);
                e.Graphics.FillRectangle(bg, rectangleDisplay);
                e.Graphics.DrawString(String.Format("X: {0} Y: {1} W: {2} H: {3}\n{4}",
                    rectangleSelection.X, rectangleSelection.Y, rectangleSelection.Width, rectangleSelection.Height,
                    "ESC - Cancel, ENTER - Confirm, F - Fullscreen, C - Toggle controls"),
                    new Font("Consolas", 8.25F), fg, rectangleDisplay, new StringFormat() { Alignment = StringAlignment.Center });
            }
        }

        /// <summary>
        /// Processes the hotkeys.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message aMSG, Keys aKeyData)
        {
            if (aKeyData == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                return true;
            }
            else if (aKeyData == Keys.C)
            {
                Settings.Default.ShowControls = !Settings.Default.ShowControls;
                comboBoxScreen.Visible = Settings.Default.ShowControls;
                Settings.Default.Save();
                Invalidate();
                return true;
            }
            else if (aKeyData == Keys.F)
            {
                rectangleSelection = new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height);
                OnMouseUp(null);
                return true;
            }
            else if (aKeyData == Keys.Enter)
            {
                if (!Settings.Default.QuickScreenshots)
                    DialogResult = DialogResult.OK;

                return true;
            }

            return base.ProcessCmdKey(ref aMSG, aKeyData);
        }
    }
}