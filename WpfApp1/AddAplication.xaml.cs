using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.ApplicationVersion.AppSetting;
using AppVersionControl.Domain.Interfaces;
using AppVersionControl.Infrastructure;
using AppVersionControl.Infrastructure.AppVersSetting;
using AppVersionControl.Infrastructure.ClientAppSetting;
using AppVersionControl.Infrastructure.DBInServer;
using AppVersionControl.Infrastructure.FileSystem;
using AppVersionControl.Infrastructure.FileSystemOneVers;
using AppVersionControlServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    ///<summary>Логика взаимодействия для AddAplication.xaml</summary>
    public partial class AddAplication : Window
    {
        /// <summary>Путь сохранения приложений </summary>
        private string Path = AppDomain.CurrentDomain.BaseDirectory;
        ///<summary>Флаг выбора реализации сохранения версий </summary>
        private string Flag = "False";
        ///<summary>Путь до файла настроек </summary>
        private static string SettingPath = AppDomain.CurrentDomain.BaseDirectory + "Setting.Config";
        ///<summary>Уведомление для пользователя</summary>
        private string message = "";
        AppVersionControlService service;
        public AddAplication()
        {
            InitializeComponent();
            Loaded += AppAplication_loaded;
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
        ///<summary>Настройка сервиса в дависимости от настроек(сохранять одну версию или несколько)</summary>
        private void SettingService()
        {
            switch (Flag)
            {
                case "True":    //сохранение нескольких версий
                    service = new AppVersionControlService(new VersionInDBServerRepo("127.0.0.1", 8005),
                            new VersionSaveInDBServerRepo("127.0.0.1", 8005),
                            new FileInDBServerRepo("127.0.0.1", 8005),
                            new FileInSystemRepo(Path), new DiscriptionAppInServer("127.0.0.1", 8005));
                    break;
                case "False":   //сохранение одной серсии
                    service = new AppVersionControlService(new VersionInDBServerRepo("127.0.0.1", 8005),
                            new VersionSaveInFileSystemRepo(Path),
                            new FileInDBServerRepo("127.0.0.1", 8005),
                            new FileInSystemRepoOne(Path), new DiscriptionAppInServer("127.0.0.1", 8005));
                    break;
            }
        }
        private void AppAplication_loaded(object sender, RoutedEventArgs e)
        {
            CheckSetting();
            SettingService();
            ListApp.ItemsSource = listApp();
        }
        ///<summary>Список приложений подверженных версионному контролю</summary>
        private List<AppVersion> listApp()
        {
            VersionInDBServerRepo repo_fileDB = new VersionInDBServerRepo("127.0.0.1", 8005);
            IDiscriptionApp discription = new DiscriptionAppInServer("127.0.0.1", 8005);
            IList<AppVersion> versionsDB = repo_fileDB.FindAll();
            List<AppVersion> versionsSort = new List<AppVersion>();
            string[] VersionsApps = (from vers in versionsDB select vers.VersionName.ToString()).Distinct().ToArray();
            versionsSort.AddRange(from string vSort in VersionsApps where discription.GetAppVersion(vSort) != null
            select discription.GetAppVersion(vSort));
            return versionsSort;
        }
        ///<summary>Обработка события нажатия на элимент списка приложений (скачивание приложения)</summary>
        private void ListApp_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (service.UpdateApp(((AppVersion)ListApp.SelectedItem).VersionName).ToString() == new ServiceResult(true).ToString())
                {
                    message = "Приложение добавлено";
                }
                else
                {
                    message = "Ошибка добавления приложения";
                }
                new Notifications.Notif(message).ShowDialog();
            }
            catch (Exception)
            {
                string message = "Ошибка добавления приложения";
                new Notifications.Notif(message).ShowDialog();
            }
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
