using Nfu.Properties;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Nfu
{
    public sealed class Snipper : Form
    {
        public static bool IsActive;
        public static ReturnTypes ReturnType;
        public enum ReturnTypes { Default, ToClipboard };

        private static Bitmap _bitmapFullScreen;
        private static int _offsetX, _offsetY;
        private enum Mode { New, X, Width, Y, Height };

        private Mode _mode;
        private Image _image;
        private Cursor _cursor;
        private Point _pointStart;
        private Rectangle _rectangle;
        private Rectangle _rectangleSelection;
        private readonly ComboBox _comboBoxScreen = new ComboBox();

        /// <summary>
        /// Snips the image.
        /// </summary>
        /// <returns>The image or null on failure.</returns>
        public static Image Snip()
        {
            IsActive = true;

            Program.FormCore.Opacity = 0;

            var rectangleFullScreen = SystemInformation.VirtualScreen;
            _bitmapFullScreen = new Bitmap(rectangleFullScreen.Width, rectangleFullScreen.Height, PixelFormat.Format32bppRgb);
            _offsetX = (rectangleFullScreen.X < 0) ? rectangleFullScreen.X * -1 : 0;
            _offsetY = (rectangleFullScreen.Y < 0) ? rectangleFullScreen.Y * -1 : 0;

            using (var gr = Graphics.FromImage(_bitmapFullScreen))
            {
                // TODO: Fix this hack
                // Somehow, the color #0D0B0C (13, 11, 12) becomes a transparent pixel, or a black pixel if there is no alpha channel
                // This doesn't happen if the graphics object is cleared using this color first, however, this is a bit hacky (why only this color)
                gr.Clear(Color.FromArgb(13, 11, 12));
                gr.CopyFromScreen(rectangleFullScreen.X, rectangleFullScreen.Y, 0, 0, _bitmapFullScreen.Size);
            }

            using (var snipper = new Snipper())
            {
                var result = snipper.ShowDialog();
                Program.FormCore.Opacity = 1;
                IsActive = false;

                if (result == DialogResult.OK) return snipper._image;
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

            _comboBoxScreen.Items.Add(Resources.MergeScreens);
            _comboBoxScreen.Location = new Point(3, 3);
            _comboBoxScreen.DropDownStyle = ComboBoxStyle.DropDownList;

            for (var i = 0; i < Screen.AllScreens.Length; i++)
            {
                _comboBoxScreen.Items.Add(string.Format(Resources.Screen, i + 1));
            }

            _comboBoxScreen.SelectedIndexChanged += ScreenIndexChanged;
            _comboBoxScreen.SelectedIndex = Settings.Default.Screen;
            _comboBoxScreen.Visible = Settings.Default.ShowControls;

            Controls.Add(_comboBoxScreen);

            Misc.SetForegroundWindow(Handle);
        }

        /// <summary>
        /// Handles changes in the screen selection.
        /// </summary>
        private void ScreenIndexChanged(object sender, EventArgs e)
        {
            if (_comboBoxScreen.SelectedIndex == 0)
            {
                _rectangle = SystemInformation.VirtualScreen;
            }
            else
            {
                _rectangle = Screen.AllScreens[_comboBoxScreen.SelectedIndex - 1].Bounds;
            }

            Size = _rectangle.Size;
            Location = _rectangle.Location;

            var bitmap = new Bitmap(_rectangle.Width, _rectangle.Height, PixelFormat.Format32bppRgb);

            using (var gr = Graphics.FromImage(bitmap))
            {
                gr.DrawImage(_bitmapFullScreen, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(_rectangle.X + _offsetX, _rectangle.Y + _offsetY, _rectangle.Width, _rectangle.Height), GraphicsUnit.Pixel);
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
            const int size = 5;
            const int margin = 2;

            if (new Rectangle(_rectangleSelection.X - margin, _rectangleSelection.Y, size, _rectangleSelection.Height).Contains(e.Location))
                return Mode.X;

            if (new Rectangle(_rectangleSelection.X + _rectangleSelection.Width - margin, _rectangleSelection.Y, size, _rectangleSelection.Height).Contains(e.Location))
                return Mode.Width;

            if (new Rectangle(_rectangleSelection.X, _rectangleSelection.Y - margin, _rectangleSelection.Width, size).Contains(e.Location))
                return Mode.Y;

            if (new Rectangle(_rectangleSelection.X, _rectangleSelection.Y + _rectangleSelection.Height - margin, _rectangleSelection.Width, size).Contains(e.Location))
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

            _mode = GetMode(e);

            _pointStart = e.Location;
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
                        _cursor = Cursors.SizeWE;
                        break;

                    case Mode.Y:
                    case Mode.Height:
                        _cursor = Cursors.SizeNS;
                        break;

                    case Mode.New:
                        _cursor = Cursors.Arrow;
                        break;
                }

                Cursor.Current = _cursor;
                return;
            }

            // Set the cursor on every move to prevent flickering
            Cursor.Current = _cursor;

            int x = 0, y = 0, w = 0, h = 0;

            switch (_mode)
            {
                case Mode.X:
                    x = e.X;
                    y = _rectangleSelection.Y;
                    w = _rectangleSelection.Width + (_rectangleSelection.X - e.X);
                    h = _rectangleSelection.Height;
                    break;

                case Mode.Width:
                    x = _rectangleSelection.X;
                    y = _rectangleSelection.Y;
                    w = e.X - _rectangleSelection.X;
                    h = _rectangleSelection.Height;
                    break;

                case Mode.Y:
                    x = _rectangleSelection.X;
                    y = e.Y;
                    w = _rectangleSelection.Width;
                    h = _rectangleSelection.Height + (_rectangleSelection.Y - e.Y);
                    break;

                case Mode.Height:
                    x = _rectangleSelection.X;
                    y = _rectangleSelection.Y;
                    w = _rectangleSelection.Width;
                    h = e.Y - _rectangleSelection.Y;
                    break;

                case Mode.New:
                    x = Math.Min(e.X, _pointStart.X);
                    y = Math.Min(e.Y, _pointStart.Y);
                    w = Math.Max(e.X, _pointStart.X) - x;
                    h = Math.Max(e.Y, _pointStart.Y) - y;
                    break;
            }

            if (w <= 10 || h <= 10)
                return;

            _rectangleSelection = new Rectangle(x, y, w, h);
            Invalidate();
        }

        /// <summary>
        /// Creates an image of the selection.
        /// Also confirms if QuickScreenshots is enabled.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_rectangleSelection.Width <= 0 || _rectangleSelection.Height <= 0)
                return;

            _image = new Bitmap(_rectangleSelection.Width, _rectangleSelection.Height, PixelFormat.Format32bppRgb);

            using (var gr = Graphics.FromImage(_image))
            {
                gr.DrawImage(BackgroundImage, new Rectangle(0, 0, _image.Width, _image.Height), _rectangleSelection, GraphicsUnit.Pixel);
            }

            ReturnType = (ModifierKeys == Keys.Control) ? ReturnTypes.ToClipboard : ReturnTypes.Default;

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
                var x1 = _rectangleSelection.X;
                var x2 = _rectangleSelection.X + _rectangleSelection.Width;
                var y1 = _rectangleSelection.Y;
                var y2 = _rectangleSelection.Y + _rectangleSelection.Height;
                e.Graphics.FillRectangle(br, new Rectangle(0, 0, x1, Height));
                e.Graphics.FillRectangle(br, new Rectangle(x2, 0, Width - x2, Height));
                e.Graphics.FillRectangle(br, new Rectangle(x1, 0, x2 - x1, y1));
                e.Graphics.FillRectangle(br, new Rectangle(x1, y2, x2 - x1, Height - y2));
            }
            using (var pen = new Pen(Color.Red, 1))
            {
                e.Graphics.DrawRectangle(pen, _rectangleSelection);
            }
            using (Brush bg = new SolidBrush(Color.FromArgb(130, Color.Black)))
            using (Brush fg = new SolidBrush(Color.White))
            {
                if (!Settings.Default.ShowControls)
                    return;

                // Prevent the info bar from showing on multiple screens
                var selectedScreenBounds = (_comboBoxScreen.SelectedIndex == 0) ?
                    Screen.PrimaryScreen.Bounds : Screen.AllScreens[_comboBoxScreen.SelectedIndex - 1].Bounds;
                var rectangleDisplay = new Rectangle(selectedScreenBounds.X, selectedScreenBounds.Y, selectedScreenBounds.Width, 27);
                e.Graphics.FillRectangle(bg, rectangleDisplay);
                e.Graphics.DrawString(string.Format(Resources.CoordinatesOverlay,
                    _rectangleSelection.X, _rectangleSelection.Y, _rectangleSelection.Width, _rectangleSelection.Height, Resources.HotKeysOverlay),
                    new Font("Consolas", 8.25F), fg, rectangleDisplay, new StringFormat() { Alignment = StringAlignment.Center });
            }
        }

        /// <summary>
        /// Processes the hotkeys.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message message, Keys keyData)
        {
            ReturnType = (ModifierKeys == Keys.Control) ? ReturnTypes.ToClipboard : ReturnTypes.Default;
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
                _comboBoxScreen.Visible = Settings.Default.ShowControls;
                Invalidate();
                return true;
            }
            else if (keys == Keys.F)
            {
                _rectangleSelection = new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height);
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
