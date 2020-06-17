using AppVersionControl.Domain.ApplicationVersion;
using System.Collections.Generic;

namespace AppVersionControl.Domain.Interfaces
{
    /// <summary>Получение информации о файлах версии</summary>
    public interface IFileRepo
    {
        /// <summary>Получение списка метаданных файлов версии приложения</summary><param name="ver"></param><returns></returns>
        IList<FileOfVersion> FindFileOfVersions(AppVersion ver);
        /// <summary>Получение списка содержимого файлов версии приложения</summary><param name="ver"></param><returns></returns>
        IList<byte[]> FindByteFileOfVersions(AppVersion ver);
        /// <summary>Коллекция файлов версии приложения</summary><param name="ver"></param><returns></returns>
        IDictionary<FileOfVersion, byte[]> FindFileOfVersionsWithData(AppVersion ver);
        /// <summary>Получение содержимого файла версии приложения</summary><param name="version"></param><param name="name"></param><returns>byte[]</returns>
        byte[] GetFile(AppVersion version, FileOfVersion name);
        /// <summary>Получение содержимого файла версии приложения</summary><param name="versionName"></param><param name="versionNumber"></param><param name="relative_file_path"></param><returns>byte[]</returns>
        byte[] GetFile(string versionName, string versionNumber, string relative_file_path);
        /// <summary>Сохранение версиии приложения</summary><param name="ver"></param><param name="file"></param><returns></returns>
        bool Save(AppVersion ver, IDictionary<FileOfVersion, byte[]> file);
    }
}
