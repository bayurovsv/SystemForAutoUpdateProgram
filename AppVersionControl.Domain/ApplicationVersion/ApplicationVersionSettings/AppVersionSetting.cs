namespace AppVersionControl.Domain.ApplicationVersion.ApplicationVersionSettings
{
    /// <summary>Модель настроек версии приложения</summary>
    public class AppVersionSetting
    {
        /// <summary>Информация о приложении</summary>
        public string AppDiscription { get; set; }
        /// <summary>Имя приложения</summary>
        public string ActialAppName { get; set; }
        /// <summary>Актуальная версия приложения</summary>
        public string ActialAppVersion { get; set; }
        /// <summary>Флаг актуальности версии приложения</summary>
        public string ActialFlag { get; set; }
        /// <summary>Имя исполняемого файла </summary>
        public string NameExe { get; set; }
    }
}
