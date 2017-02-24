using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using log4net;
using ScreenCutter.App.Native;
using ScreenCutter.PluginContract;

namespace ScreenCutter.App
{
    /// <summary>
    /// Interaction logic for ScreenShotLenseWindow.xaml
    /// </summary>
    public partial class ScreenShotLenseWindow : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Point rightDown;

        public ScreenShotLenseWindow()
        {
            InitializeComponent();

            // alpha cannot be 0, otherwise hiding of cursor won't work!
            this.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(1, 0, 0, 0));

            this.Cursor = Cursors.None;

            this.Expand = ExpandDirection.All;
        }

        public ExpandDirection Expand { get; set; }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(this);
            this.rightDown = this.PointToScreen(mousePos);

            this.MouseMove -= Window_MouseMove;
            this.MouseWheel -= Window_MouseWheel;
            this.MouseLeftButtonUp -= Window_MouseLeftButtonUp;

            this.cropDimensionWidth.Text = (this.shotArea.ActualWidth - 2).ToString();
            this.cropDimensionHeight.Text = (this.shotArea.ActualHeight - 2).ToString();

            this.Cursor = null;
            
            this.menuArea.SetValue(Canvas.LeftProperty, mousePos.X - (this.menuArea.Width / 2));
            this.menuArea.SetValue(Canvas.TopProperty, mousePos.Y - (this.menuArea.Height / 2));

            this.menuArea.Visibility = Visibility.Visible;
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.None;

            WinApiMethods.SetCursorPos((int)this.rightDown.X, (int)this.rightDown.Y);

            this.menuArea.Visibility = Visibility.Collapsed;

            this.MouseMove += Window_MouseMove;
            this.MouseWheel += Window_MouseWheel;
            this.MouseLeftButtonUp += Window_MouseLeftButtonUp;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(this);

            this.shotArea.SetValue(Canvas.LeftProperty, pos.X - (this.shotArea.ActualWidth / 2));
            this.shotArea.SetValue(Canvas.TopProperty, pos.Y - (this.shotArea.ActualHeight / 2));
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.ChangeSize(Properties.Settings.Default.MouseWheelStepIsPixelCount * e.Delta / -120);
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = this.shotArea.PointToScreen(new Point(1d, 1d));
            var screenshot = this.TakeScreenshot((int)position.X, (int)position.Y, (int)this.shotArea.ActualWidth - 2, (int)this.shotArea.ActualHeight - 2);
            
            var plugin = Plugins.Get<ISaveScreenAreaPlugin>(Properties.Settings.Default.SaveScreenAreaPluginFullName);

            if (plugin == null)
            {
                var fileDialog = new Microsoft.Win32.SaveFileDialog();

                fileDialog.DefaultExt = ".bmp";
                fileDialog.Filter = "BMP Files (*.bmp)|*.bmp|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

                if (fileDialog.ShowDialog() == true)
                {
                    screenshot.Save(fileDialog.FileName);
                }
            }
            else
            {
                plugin.Save(screenshot);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Exit();

                return;
            }

            if (this.menuArea.Visibility == Visibility.Visible)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Up:
                    {
                        var posOnScreen = this.PointToScreen(Mouse.GetPosition(this));
                        WinApiMethods.SetCursorPos((int)posOnScreen.X, (int)posOnScreen.Y - 1);
                        break;
                    }

                case Key.Down:
                    {
                        var posOnScreen = this.PointToScreen(Mouse.GetPosition(this));
                        WinApiMethods.SetCursorPos((int)posOnScreen.X, (int)posOnScreen.Y + 1);
                        break;
                    }

                case Key.Left:
                    {
                        var posOnScreen = this.PointToScreen(Mouse.GetPosition(this));
                        WinApiMethods.SetCursorPos((int)posOnScreen.X - 1, (int)posOnScreen.Y);
                        break;
                    }

                case Key.Right:
                    {
                        var posOnScreen = this.PointToScreen(Mouse.GetPosition(this));
                        WinApiMethods.SetCursorPos((int)posOnScreen.X + 1, (int)posOnScreen.Y);
                        break;
                    }

                case Key.Add:
                    {
                        this.ChangeSize(2);
                        break;
                    }

                case Key.Subtract:
                    {
                        this.ChangeSize(-2);
                        break;
                    }
            }
        }

        private void Close_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Exit();
        }

        private void VerticalButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Expand = ExpandDirection.Vertical;
        }

        private void HorizontalButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Expand = ExpandDirection.Horizontal;
        }

        private void AllWaysButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Expand = ExpandDirection.All;
        }

        private System.Drawing.Bitmap TakeScreenshot(int fromX, int fromY, int width, int height)
        {
            var screenshot = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(fromX, fromY, 0, 0, screenshot.Size);
            }

            return screenshot;
        }

        private void ChangeSize(double newSize)
        {
            if (this.Expand.HasFlag(ExpandDirection.Horizontal))
            {
                double delta = newSize;

                var newWidth = this.shotArea.ActualWidth + delta;

                if (newWidth < 3)
                {
                    newWidth = 3;

                    delta = newWidth - this.shotArea.ActualWidth;
                }

                var x = (double)this.shotArea.GetValue(Canvas.LeftProperty);

                this.shotArea.Width = newWidth;

                this.shotArea.SetValue(Canvas.LeftProperty, x - (delta / 2));
            }

            if (this.Expand.HasFlag(ExpandDirection.Vertical))
            {
                double delta = newSize;

                var newHeight = this.shotArea.ActualHeight + delta;

                if (newHeight < 3)
                {
                    newHeight = 3;

                    delta = newHeight - this.shotArea.ActualHeight;
                }

                var y = (double)this.shotArea.GetValue(Canvas.TopProperty);

                this.shotArea.Height = newHeight;

                this.shotArea.SetValue(Canvas.TopProperty, y - (delta / 2));
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
