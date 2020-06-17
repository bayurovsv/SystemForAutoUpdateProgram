using AppVersionControl.Domain.ApplicationVersion;

namespace AppVersionControl.Domain.Interfaces
{
    /// <summary>Сохранение метаданных версии приложения</summary>
    public interface IVersionSaveRepo 
    {
        /// <summary>Сохранение метаданных файлов приложения</summary><param name="version"></param>
        bool SaveApplication(AppVersion version);
    }
}