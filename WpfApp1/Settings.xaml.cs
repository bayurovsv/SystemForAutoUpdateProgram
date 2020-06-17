using AppVersionControl.Domain.ApplicationVersion.AppSetting;
using AppVersionControl.Infrastructure.ClientAppSetting;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace WpfApp1
{
    /// <summary>Логика взаимодействия для Settings.xaml</summary>
    public partial class Settings : Window
    {
        ///<summary>Путь до файла настроек </summary>
        private static string SettingPath = AppDomain.CurrentDomain.BaseDirectory + "Setting.Config";
        public Settings()
        {
            InitializeComponent();
            Loaded += Settings_Loaded;
        }
        ///<summary>Загрука настроек из файла</summary>
        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            SettingAppClient appClient = new SettingAppClient();
            ClientSetting clientSetting = appClient.GetSetting(SettingPath);
            if (clientSetting != null)
            {
                Path.Text = clientSetting.PathSetting;
                Check.IsChecked = clientSetting.FlagSettings;
                Check.IsEnabled = false;
            }
        }
        ///<summary>Получение пути для сохранеия приложений</summary>
        private void GetPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openDialog = new FolderBrowserDialog();
            openDialog.ShowDialog();
            Path.Text = openDialog.SelectedPath;
        }
        ///<summary>Сохранение настроек в файл</summary>
        private void SaveSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Path.Text != "")
                {
                    ClientSetting setting = new ClientSetting(Path.Text, Check.IsChecked.Value);
                    SettingAppClient appClient = new SettingAppClient();
                    appClient.SaveSetting(setting, SettingPath);
                    string message = "Настройки сохранены";
                    new Notifications.Notif(message).ShowDialog();
                    Close();
                }
                else
                {
                    string message = "Заполните все поля!";
                    new Notifications.Notif(message).ShowDialog();
                }
            }
            catch (Exception)
            {
                string message = "Ошибка сохранения настроек";
                new Notifications.Notif(message).ShowDialog();
                Close();
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
        #endregion
    }
}
