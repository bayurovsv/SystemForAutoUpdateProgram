using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using AppVersionControl.Infrastructure.AppVersSetting;
using AppVersionControl.Infrastructure.Bilders;
using AppVersionControl.Infrastructure.DBInServer;
using AppVersionControl.Infrastructure.FileSystem;
using AppVersionControlServices;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace AdminClient
{
    ///<summary>Логика взаимодействия для AddApps.xaml</summary>
    public partial class AddApps : Window
    {
        ///<summary>Уведомление для пользователя</summary>
        private string message = "";
        AppVersionControlService service;
        public AddApps()
        {
            InitializeComponent();
            SettingService();
        }
        private void SettingService()
        {
            service = new AppVersionControlService(new VersionInDBServerRepo("127.0.0.1", 8005),
                                new VersionSaveInDBServerRepo("127.0.0.1", 8005),
                                new FileInSystemRepo(AppDomain.CurrentDomain.BaseDirectory),
                                new FileInDBServerRepo("127.0.0.1", 8005), new DiscriptionAppInServer("127.0.0.1", 8005));
        }
        private void GetPath_Click(object sender, RoutedEventArgs e) /// получение пути к приложению
        {
            FolderBrowserDialog openDialog = new FolderBrowserDialog();
            openDialog.ShowDialog();
            Path.Text = openDialog.SelectedPath;
        }
        private void AddApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Number.Text != "" && NameApp.Text != "" && Path.Text != "")
                {
                    IVersionRepo versionRepo = new VersionInDBServerRepo("127.0.0.1", 8005);
                    IList<AppVersion> versions = versionRepo.FindAll();
                    AppVersion version = new AppVersion(NameApp.Text, new VersionNumber(1, 0));
                    if (!versions.Contains(version))
                    {
                        if (service.AddAplicationNew(Path.Text, NameApp.Text, new VersionNumber(Convert.ToInt32(Number.Text), 0)).ToString() == new ServiceResult(true).ToString())
                        {
                            message = "Приложение добавлено";
                        }
                        else
                        {
                            message = "Ошибка обновления приложения";
                        }
                        new Notifications.Notif(message).ShowDialog();
                        Close();
                    }
                    else
                    {
                        if (service.AddAplication(NameApp.Text, new AppBuilder(Path.Text, NameApp.Text, new VersionNumber(Convert.ToInt32(Number.Text), 0))).ToString() == new ServiceResult(true).ToString())
                        {
                            message = "Приложение добавлено";
                            new Notifications.Notif(message).ShowDialog();
                            Close();
                        }
                        else
                        {
                            message = "Введена старая версия";
                        }
                    }
                }
                else
                {
                    string message = "Заполните все поля!";
                    new Notifications.Notif(message).ShowDialog();
                }
            }
            catch (Exception)
            {
                string message = "Ошибка добавления приложения";
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

        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }
        private void NameApp_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
            if (!char.IsSeparator((e.Text).ToCharArray()[e.Text.Length - 1]))
            {
                if (((e.Text).ToCharArray()[e.Text.Length - 1] == '_'))
                {
                    e.Handled = true;
                }
                e.Handled = false;
            }
            
        }
        private void NameApp_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        #endregion
    }
}
