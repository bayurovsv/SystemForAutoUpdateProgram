using AppVersionControl.Domain.ApplicationVersion.AppSetting;

namespace AppVersionControl.Domain.Interfaces
{
    /// <summary>Нaстройка клиентского приложения</summary>
    public interface ISettingAppClient
    {
        /// <summary>Получение настроек клиентского приложения</summary><param name="path"></param><returns>ClientSetting</returns>
        ClientSetting GetSetting(string path);
        /// <summary>Сохранение настроек приложения</summary><param name="setting"></param><param name="path"></param><returns>bool</returns>
        bool SaveSetting(ClientSetting setting, string path);
    }
}
