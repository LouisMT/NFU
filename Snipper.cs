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

        private Image image;
        private Point pointStart;
        private Rectangle rectangle;
        private Rectangle rectangleSelection;
        private Label labelInfo = new Label();
        private ComboBox comboBoxScreen = new ComboBox();

        /// <summary>
        /// Snip the image.
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

        private Snipper()
        {
            ShowInTaskbar = false;
            DoubleBuffered = true;
            Cursor = Cursors.Cross;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;

            comboBoxScreen.Items.Add("Merge screens");
            comboBoxScreen.Location = new Point(12, 12);
            comboBoxScreen.DropDownStyle = ComboBoxStyle.DropDownList;

            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                comboBoxScreen.Items.Add(String.Format("Screen {0}", i + 1));
            }

            comboBoxScreen.SelectedIndexChanged += cc_SelectedIndexChanged;
            comboBoxScreen.SelectedIndex = Properties.Settings.Default.Screen;

            this.Controls.Add(comboBoxScreen);

            labelInfo.AutoSize = true;
            labelInfo.BackColor = Color.Black;
            labelInfo.ForeColor = Color.White;
            labelInfo.Location = new Point(12, 33);
            labelInfo.Font = new Font("Consolas", 8.25F);
            labelInfo.BorderStyle = BorderStyle.FixedSingle;
            labelInfo.MinimumSize = new Size(comboBoxScreen.Width, 0);
            labelInfo.Text = "ESC - Cancel\nF   - Fullscreen\nC   - Controls";

            this.Controls.Add(labelInfo);

            Misc.SetForegroundWindow(Handle);
        }

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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            pointStart = e.Location;
            rectangleSelection = new Rectangle(e.Location, new Size(0, 0));
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            int x1 = Math.Min(e.X, pointStart.X);
            int y1 = Math.Min(e.Y, pointStart.Y);
            int x2 = Math.Max(e.X, pointStart.X);
            int y2 = Math.Max(e.Y, pointStart.Y);
            rectangleSelection = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (rectangleSelection.Width <= 0 || rectangleSelection.Height <= 0) return;
            image = new Bitmap(rectangleSelection.Width, rectangleSelection.Height);
            using (Graphics gr = Graphics.FromImage(image))
            {
                gr.DrawImage(BackgroundImage, new Rectangle(0, 0, image.Width, image.Height), rectangleSelection, GraphicsUnit.Pixel);
            }
            DialogResult = DialogResult.OK;
        }

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
        }

        protected override bool ProcessCmdKey(ref Message aMSG, Keys aKeyData)
        {
            if (aKeyData == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                return true;
            }
            else if (aKeyData == Keys.C)
            {
                foreach (Control ctrl in Controls) ctrl.Visible = !ctrl.Visible;
                return true;
            }
            else if (aKeyData == Keys.F)
            {
                rectangleSelection = new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height);
                OnMouseUp(null);
                return true;
            }

            return base.ProcessCmdKey(ref aMSG, aKeyData);
        }
    }
}