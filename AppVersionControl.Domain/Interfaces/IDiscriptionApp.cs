using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.ApplicationVersion.ApplicationVersionSettings;

namespace AppVersionControl.Domain.Interfaces
{
    /// <summary>Получении информациио об актуальной версии приложения</summary>
    public interface IDiscriptionApp
    {
        /// <summary>Получение настроек версии приложения</summary><param name="appName"></param><param name="appVersion"></param><returns>AppVersionSetting</returns>
        AppVersionSetting GetSetting(string appName, string appVersion);
        /// <summary>Сохранение настроек</summary><param name="versionSetting"></param><returns>bool</returns>
        bool SaveSetting(AppVersionSetting versionSetting);
        /// <summary>Получение версии приложения</summary><param name="appName"></param><returns>AppVersion</returns>
        AppVersion GetAppVersion(string appName);
        /// <summary>Получение описания версии приложения</summary><param name="appName"></param><param name="appVersion"></param><returns>string</returns>
        string GetDiscription(string appName, string appVersion);
        /// <summary>Получение имени исполняемого файла приложения</summary><param name="appName"></param><param name="appVersion"></param><returns>string</returns>
        string GetNameExe(string appName, string appVersion);
    }
}
