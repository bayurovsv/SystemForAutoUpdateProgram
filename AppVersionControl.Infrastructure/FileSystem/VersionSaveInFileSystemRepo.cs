using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using System;
using System.IO;
using System.Web.Script.Serialization;

namespace AppVersionControl.Infrastructure
{
    /// <summary>Сохранение метаданных версии приложения</summary>
    public class VersionSaveInFileSystemRepo : IVersionSaveRepo
    {
        private class VersionMeta
        {
            /// <summary>Имя приложения</summary>
            public string AppName { get; set; }
            /// <summary>Номер версии </summary>
            public int Num { get; set; }
            /// <summary>Номер сборки</summary>
            public int Bild { get; set; }
        }

        /// <summary>Путь сохранения</summary>
        private readonly string RootPath;
        public VersionSaveInFileSystemRepo(string root_path)
        {
            RootPath = root_path ?? throw new ArgumentNullException(nameof(root_path));
            RootPath = Path.Combine(RootPath, "Versions");
        }
        /// <summary>Сохранение метаданных</summary><param name="version"></param>
        public bool SaveApplication(AppVersion version)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                VersionMeta versionMeta= new VersionMeta();
                versionMeta.AppName = version.VersionName;
                versionMeta.Num = version.VersionNumber.Number;
                versionMeta.Bild = version.VersionNumber.VersionBuildNumber;
                string json = serializer.Serialize(versionMeta);
                File.WriteAllText(RootPath + "\\" + version.VersionName +"\\"+ version.VersionName + "-" + version.VersionNumber.ToString() + ".xml", json);
                return true;
            }
            catch (SaveApplicationException ex) 
            {
                throw new SaveApplicationException();
            }
        }
        #region Domain exception
        public class SaveApplicationException : Exception
        {
            public SaveApplicationException()
                 : base("Ошибка сохранения")
            {
            }
        }
        #endregion
    }
}
