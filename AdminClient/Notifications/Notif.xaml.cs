using System.Windows;
using System.Windows.Input;

namespace AdminClient.Notifications
{
    /// <summary>
    /// Логика взаимодействия для Notif.xaml
    /// </summary>
    public partial class Notif : Window
    {
        public Notif(string message)
        {
            InitializeComponent();
            info.Content = message;
        }

        private void ОК_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #region Form control 
        private void ColorZone_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void BdClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BdСollapse_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }
        private void Btn_min_click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Minimized;
                WindowStyle = WindowStyle.None;
            }
            else
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Minimized;
                    WindowStyle = WindowStyle.None;
                }
                else
                {
                    if (WindowState == WindowState.Minimized)
                    {
                        WindowState = WindowState.Normal;
                    }
                }
            }
        }
        #endregion

    }
}
