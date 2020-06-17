namespace AppVersionControl.Domain.ApplicationVersion.AppSetting
{
    /// <summary>Модель настроек клиентского приложения</summary>
    public class ClientSetting
    {
        /// <summary>Путь сохранения приложений</summary>
        public string PathSetting { get; }
        /// <summary>Флаг выбора реализации сохранения версий приложений</summary>
        public bool FlagSettings { get; }
        public ClientSetting(string pathSet, bool flagSet)
        {
            PathSetting = pathSet;
            FlagSettings = flagSet;
        }
    }
}
