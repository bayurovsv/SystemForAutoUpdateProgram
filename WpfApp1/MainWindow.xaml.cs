using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.ApplicationVersion.AppSetting;
using AppVersionControl.Infrastructure;
using AppVersionControl.Infrastructure.AppVersSetting;
using AppVersionControl.Infrastructure.ClientAppSetting;
using AppVersionControl.Infrastructure.DBInServer;
using AppVersionControl.Infrastructure.FileSystem;
using AppVersionControl.Infrastructure.FileSystemOneVers;
using AppVersionControlServices;

namespace WpfApp1
{
    /// <summary>Логика взаимодействия для MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        /// <summary>Путь сохранения приложений </summary>
        private string Path = AppDomain.CurrentDomain.BaseDirectory;
        ///<summary>Флаг выбора реализации сохранения версий </summary>
        private string Flag = "False";
        ///<summary>Путь до файла настроек </summary>
        private static string SettingPath = AppDomain.CurrentDomain.BaseDirectory + "Setting.Config";
        ///<summary>Уведомление для пользователя</summary>
        private string message = "";
        private bool check = false;
        AppVersionControlService service;
        AppVersionControlService serviceCheck;
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 1800000;// раз в 30 минут
            timer1.Tick += setData;
            timer1.Enabled = true;
            setData(timer1, EventArgs.Empty);
        }
        private void setData(object sender, EventArgs e)
        {
            MainWindow_Loaded();
        }
        ///<summary>Метод получения настроек приложения из файла</summary>
        private void CheckSetting()
        {
            SettingAppClient appClient = new SettingAppClient();
            ClientSetting clientSetting = appClient.GetSetting(SettingPath);
            if (clientSetting != null)
            {
                Path = clientSetting.PathSetting;
                Flag = clientSetting.FlagSettings.ToString();
            }
        }
        ///<summary>Настройка сервиса в зависимости от настроек(сохранять одну версию или несколько)</summary>
        private void SettingService()
        {
            switch (Flag)
            {
                case "True":    //сохранение нескольких версий
                    service = new AppVersionControlService(new VersionInFileSystemRepo(Path),
                            new VersionSaveInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory),
                            new FileInDBServerRepo("127.0.0.1", 8005),
                            new FileInSystemRepo(Path), new DiscriptionAppInServer("127.0.0.1", 8005));
                    serviceCheck = new AppVersionControlService(new VersionInFileSystemRepo(Path),
                           new VersionSaveInDBServerRepo("127.0.0.1", 8005),
                           new FileInDBServerRepo("127.0.0.1", 8005),
                           new FileInSystemRepo(Path), new DiscriptionAppInServer("127.0.0.1", 8005));
                    break;
                case "False":   //сохранение одной серсии
                    service = new AppVersionControlService(new VersionInFileSystemRepoOne(Directory.GetCurrentDirectory()),
                            new VersionSaveInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory),
                            new FileInDBServerRepo("127.0.0.1", 8005),
                            new FileInSystemRepoOne(Path), new DiscriptionAppInServer("127.0.0.1", 8005));
                    serviceCheck = new AppVersionControlService(new VersionInFileSystemRepoOne(Path),
                           new VersionSaveInDBServerRepo("127.0.0.1", 8005),
                           new FileInDBServerRepo("127.0.0.1", 8005),
                           new FileInSystemRepo(Path), new DiscriptionAppInServer("127.0.0.1", 8005));
                    break;
            }
        }
        public void MainWindow_Loaded()
        {
            CheckSetting();
            SettingService();
            СheckApps();
        }
        ///<summary>Метод отобрадения приложений подверженных версионному контролю</summary>
        private void СheckApps()
        {
            Directory.CreateDirectory(Path + "\\Versions");
            ListApps.ItemsSource = service.GetApplications(serviceCheck.GetVersionFileSystem());
        }
        ///<summary>Обработка события нажатия на элимент списка приложений (обновление приложения)</summary>
        private void ListApps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (check == false)
            {
                try
                {
                    DisplayApplication application = (DisplayApplication)ListApps.SelectedItem;
                    if (application.FlagUpdate == "Доступно обновление")
                    {
                        var test = service.UpdateApp(application.ApplicationName);
                        if (test.IsSuccess)
                        {
                            message = "Приложение обновлено";
                        }
                        else
                        {
                            message = "Приложение не обновлено";
                        }
                        new Notifications.Notif(message).ShowDialog();
                        СheckApps();
                    }
                    else
                    {
                        VersionNumber VerNum;
                        VersionNumber.TryParse(application.ApplicationNumber, out VerNum);
                        service.StartApp(Path, new AppVersion(application.ApplicationName, VerNum));
                    }
                }
                catch (Exception)
                {
                    string message = "Приложение не обновлено";
                    new Notifications.Notif(message).ShowDialog();
                }
                check = true;
            }
            else
            {
                check = false;
            }
        }
        #region Form control 
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
            MainWindow_Loaded();
        }
        private void AddApp_Click(object sender, RoutedEventArgs e)
        {
            AddAplication AddAplication = new AddAplication();
            AddAplication.ShowDialog();
            MainWindow_Loaded();
        }
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
            Application.Current.Shutdown();
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
        private void Check_Update(object sender, RoutedEventArgs e)
        {
            MainWindow_Loaded();
        }
    }
}
