using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.ApplicationVersion.ApplicationVersionSettings;
using AppVersionControl.Domain.Interfaces;
using AppVersionControl.Infrastructure.AppVersSetting;
using AppVersionControl.Infrastructure.DBInServer;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;


namespace AdminClient.SettingApp
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public string AppName;
        public string AppVers;
        public Settings(AppVersion appVersion)
        {
            InitializeComponent();
            AppName = appVersion.VersionName;
            AppVers = appVersion.VersionNumber.ToString();
            Loaded += Settings_Loaded;
        }
        ///<summary>Загрука настроек из файла</summary>
        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            IDiscriptionApp discription = new DiscriptionAppInServer("127.0.0.1", 8005);
            AppVersionSetting appSetting = discription.GetSetting(AppName, AppVers);
            Discription.Text = appSetting.AppDiscription;
            switch (appSetting.ActialFlag)
            {
                case "True":
                    Check.IsChecked = true;
                    break;
                case "False":
                    Check.IsChecked = false;
                    break;
            }
            FileInDBServerRepo repo = new FileInDBServerRepo("127.0.0.1", 8005);
            VersionNumber.TryParse(AppVers, out VersionNumber number);
            IList<FileOfVersion> testfile = repo.FindFileOfVersions(new AppVersion(AppName, number));
            List<string> fileName = new List<string>();
            for (int i = 0; i < testfile.Count; i++)
            {
                fileName.Add(testfile[i].FilePath);
            }
            exeFile.ItemsSource = (fileName);
            exeFile.Text = appSetting.NameExe;
            AppText.Text = "Настройки приложения: "+ AppName + " " + number.ToString();
        }
        ///<summary>Сохранение настроек в файл</summary>
        private void SaveSetting_Click(object sender, RoutedEventArgs e)
        {
            IDiscriptionApp discription = new DiscriptionAppInServer("127.0.0.1", 8005);
            AppVersionSetting appSetting = discription.GetSetting(AppName, AppVers);
            appSetting.ActialAppName = AppName;
            appSetting.ActialAppVersion = AppVers;
            appSetting.AppDiscription = Discription.Text;
            appSetting.ActialFlag = Check.IsChecked.Value.ToString();
            appSetting.NameExe = exeFile.Text;
            discription.SaveSetting(appSetting);
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
        #endregion
    }
}