using AppVersionControl.Domain.ApplicationVersion;
using System.Collections.Generic;

namespace AppVersionControl.Domain.Interfaces
{
    /// <summary>Получение информации о версии приложения</summary>
    public interface IVersionRepo
    {
        /// <summary>Поиск последней версии приложения</summary><param name="version_name"></param><returns>AppVersion</returns>
        AppVersion FindLast(string version_name);
        /// <summary>Поиск последней версии приложения</summary><param name="app"></param><returns>AppVersion</returns>
        AppVersion Find(AppVersion app);
        /// <summary>Поиск последней версии приложения</summary><param name="version_name"></param><param name="versionNumber"></param><returns></returns>
        AppVersion Find(string version_name, VersionNumber versionNumber);
        /// <summary>Получение списка всех версий приложений</summary><returns>List</returns>
        IList<AppVersion> FindAll();
        /// <summary>Получение списка всех версий оперделённого приложения</summary><param name="version_name"></param><returns>List</returns>
        IList<AppVersion> FindAll(string version_name);
    }
}
