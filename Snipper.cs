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
        public static returnTypes returnType;
        public enum returnTypes { Default, ToClipboard };

        private static Bitmap bitmapFullScreen;
        private static int offsetX, offsetY;

        private enum Mode { New, X, Width, Y, Height };

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

            Program.formCore.Opacity = 0;

            Rectangle rectangleFullScreen = SystemInformation.VirtualScreen;
            bitmapFullScreen = new Bitmap(rectangleFullScreen.Width, rectangleFullScreen.Height, PixelFormat.Format32bppRgb);
            offsetX = (rectangleFullScreen.X < 0) ? rectangleFullScreen.X * -1 : 0;
            offsetY = (rectangleFullScreen.Y < 0) ? rectangleFullScreen.Y * -1 : 0;

            using (Graphics gr = Graphics.FromImage(bitmapFullScreen))
            {
                // TODO: Fix this hack
                // Somehow, the color #0D0B0C (13, 11, 12) becomes a transparent pixel, or a black pixel if there is no alpha channel
                // This doesn't happen if the graphics object is cleared using this color first, however, this is a bit hacky (why only this color)
                gr.Clear(Color.FromArgb(13, 11, 12));
                gr.CopyFromScreen(rectangleFullScreen.X, rectangleFullScreen.Y, 0, 0, bitmapFullScreen.Size);
            }

            using (Snipper snipper = new Snipper())
            {
                DialogResult result = snipper.ShowDialog();
                Program.formCore.Opacity = 1;
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

            comboBoxScreen.SelectedIndexChanged += ScreenIndexChanged;
            comboBoxScreen.SelectedIndex = Settings.Default.Screen;
            comboBoxScreen.Visible = Settings.Default.ShowControls;

            this.Controls.Add(comboBoxScreen);

            Misc.SetForegroundWindow(Handle);
        }

        /// <summary>
        /// Handles changes in the screen selection.
        /// </summary>
        private void ScreenIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxScreen.SelectedIndex == 0)
            {
                rectangle = SystemInformation.VirtualScreen;
            }
            else
            {
                rectangle = Screen.AllScreens[comboBoxScreen.SelectedIndex - 1].Bounds;
            }

            Size = rectangle.Size;
            Location = rectangle.Location;

            Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppRgb);

            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                gr.DrawImage(bitmapFullScreen, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(rectangle.X + offsetX, rectangle.Y + offsetY, rectangle.Width, rectangle.Height), GraphicsUnit.Pixel);
            }

            BackgroundImage = bitmap;
        }

        /// <summary>
        /// Get the current mode based on the position of the cursor.
        /// </summary>
        /// <returns>The current mode.</returns>
        private Mode GetMode(MouseEventArgs e)
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

            mode = GetMode(e);

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
                switch (GetMode(e))
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
            if (rectangleSelection.Width <= 0 || rectangleSelection.Height <= 0)
                return;

            image = new Bitmap(rectangleSelection.Width, rectangleSelection.Height, PixelFormat.Format32bppRgb);

            using (Graphics gr = Graphics.FromImage(image))
            {
                gr.DrawImage(BackgroundImage, new Rectangle(0, 0, image.Width, image.Height), rectangleSelection, GraphicsUnit.Pixel);
            }

            returnType = (Control.ModifierKeys == Keys.Control) ? returnTypes.ToClipboard : returnTypes.Default;

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
                    "ESC - Cancel, ENTER - Confirm, F - Fullscreen, C - Toggle controls, Hold CTRL - To clipboard"),
                    new Font("Consolas", 8.25F), fg, rectangleDisplay, new StringFormat() { Alignment = StringAlignment.Center });
            }
        }

        /// <summary>
        /// Processes the hotkeys.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message message, Keys keyData)
        {
            returnType = (Control.ModifierKeys == Keys.Control) ? returnTypes.Default : returnTypes.ToClipboard;
            // Remove the control modifier from the keys
            var keys = (keyData & ~Keys.Control);

            if (keys == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                return true;
            }
            else if (keys == Keys.C)
            {
                Settings.Default.ShowControls = !Settings.Default.ShowControls;
                comboBoxScreen.Visible = Settings.Default.ShowControls;
                Settings.Default.Save();
                Invalidate();
                return true;
            }
            else if (keys == Keys.F)
            {
                rectangleSelection = new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height);
                OnMouseUp(null);
                return true;
            }
            else if (keys == Keys.Enter)
            {
                if (!Settings.Default.QuickScreenshots)
                    DialogResult = DialogResult.OK;

                return true;
            }

            return true;
        }
    }
}
