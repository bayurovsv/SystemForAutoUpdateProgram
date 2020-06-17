using AppVersionControl.Domain.ApplicationVersion;
using System.Collections.Generic;

namespace AppVersionControl.Domain.Interfaces
{
    /// <summary>Сборка версии приложения из файлов по указанному пути</summary>
    public interface IApplicationVersionBuilder
    {
        /// <summary> Сборка версии приложения</summary><returns>AppVersion</returns>
        AppVersion Build();
        /// <summary>Получение коллекция содержимого файлов версии </summary><returns>IDictionary<FileOfVersion, byte[]></returns>
        IDictionary<FileOfVersion, byte[]> FindFileOfVersionsWithData();
    }
}
