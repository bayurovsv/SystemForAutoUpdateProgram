using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Infrastructure.DBInServer;

namespace AdminClient
{
    ///<summary>Логика взаимодействия для MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindow_Loaded();
            ListApps.SelectedIndex = -1;
        }
        private void MainWindow_Loaded()
        {
            VersionInDBServerRepo repo = new VersionInDBServerRepo("127.0.0.1", 8005);
            IList<AppVersion> versions = repo.FindAll();
            ListApps.ItemsSource = versions;
        }
        private void AddApp_Click(object sender, RoutedEventArgs e)
        {
            AddApps ApplicationsAdd = new AddApps();
            ApplicationsAdd.ShowDialog();
            MainWindow_Loaded();
        }
        #region Form control 
        private void CloseBar(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            MenuButton.IsChecked = false;
        }
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
        private void ListApps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListApps.SelectedIndex != -1)
            {
                SettingApp.Settings settings = new SettingApp.Settings((AppVersion)ListApps.SelectedItem);
                settings.ShowDialog();
                ListApps.SelectedIndex = -1;
            }
        }
    }
}
