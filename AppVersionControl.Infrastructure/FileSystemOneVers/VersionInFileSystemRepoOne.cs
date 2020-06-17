using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using static AppVersionControl.Infrastructure.VersionInFileSystemRepo;

namespace AppVersionControl.Infrastructure.FileSystemOneVers
{
    /// <summary>Получение информации о версии приложения</summary>
    public class VersionInFileSystemRepoOne : IVersionRepo
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

        /// <summary>Путь к версии</summary>
        private readonly string RootPath;
        public VersionInFileSystemRepoOne(string root_path)
        {
            RootPath = root_path ?? throw new ArgumentNullException(nameof(root_path));
            RootPath = Path.Combine(RootPath, "Versions");
        }
        #region FindLast
        ///<summary>Поиск последней версии приложения</summary><param name="version_name"></param>
        public AppVersion FindLast(string version_name)
        {
            return Find_(version_name);
        }
        ///<summary>Поиск последней версии приложения</summary><param name="app"></param>
        public AppVersion Find(AppVersion app)
        {
            return Find_(app.VersionName);
        }
        ///<summary>Поиск последней версии приложения</summary><param name="version_name"></param><param name="versionNumber"></param>
        public AppVersion Find(string version_name, VersionNumber versionNumber)
        {
            string[] Apps = Directory.GetDirectories(RootPath);// массив приложений(папки с названиями всех приложений) 
            string App = (from app in Apps where (Path.GetFileNameWithoutExtension(app.ToLower()) == version_name.ToLower()) select app).Single();//Строка содержащая суть к нужному приложению(к версиям)
            List<FileOfVersion> fileApp = new List<FileOfVersion>();
            List<string> ListFile = new List<string>();
            ListFile = GetFiles_app(version_name, versionNumber);
            for (int x = 0; x < ListFile.Count; x++)
            {
                FileOfVersion verss = new FileOfVersion(ListFile[x], fileContain(RootPath + "\\" + version_name + ListFile[x]), File.GetCreationTime(RootPath + "\\" + version_name + ListFile[x]));
                fileApp.Add(verss);
            }
            AppVersion res = new AppVersion(Path.GetFileNameWithoutExtension(App), versionNumber, fileApp);
            return res;
        }
        private AppVersion Find_(string name)
        {
            try
            {
                string[] Apps = Directory.GetDirectories(RootPath);// массив приложений(папки с названиями всех приложений) 
                string App = (from app in Apps where (Path.GetFileNameWithoutExtension(app.ToLower()) == name.ToLower()) select app).Single();//Строка содержащая суть к нужному приложению(к версиям)
                string[] VersionsApps = (from vers in Directory.GetFiles(App) orderby vers descending select vers.ToString()).ToArray();
                string js = File.ReadAllText(VersionsApps[0]);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                VersionMeta meta = serializer.Deserialize<VersionMeta>(js);
                VersionNumber Vers = new VersionNumber(meta.Num, meta.Bild);
                List<FileOfVersion> fileApp = new List<FileOfVersion>();
                List<string> ListFile = new List<string>();
                ListFile = GetFiles_app(name, Vers);
                for (int x = 0; x < ListFile.Count; x++)
                {
                    FileOfVersion verss = new FileOfVersion(ListFile[x], fileContain(RootPath + "\\" + name + ListFile[x]), File.GetCreationTime(RootPath + "\\" + name + ListFile[x]));
                    fileApp.Add(verss);
                }
                AppVersion res = new AppVersion(Path.GetFileNameWithoutExtension(App), Vers, fileApp);
                return res;
            }
            catch (AppVersionIsNullException ex)
            {
                return null;
                throw new AppVersionIsNullException();
            }
        }
        #endregion
        #region FindAll
        ///<summary>Список всех версий приложений</summary>
        public IList<AppVersion> FindAll()
        {
            List<AppVersion> AppVersions = new List<AppVersion>();
            try
            {
                string[] folders = Directory.GetDirectories(RootPath);
                foreach (string App in folders)
                {
                    AppVersion version = new AppVersion(Path.GetFileNameWithoutExtension(App), new VersionNumber(1, 0));
                    AppVersion NewVersion = new AppVersion(Path.GetFileNameWithoutExtension(App), new VersionNumber(1, 0));
                    string[] VersionsApps = (from vers in Directory.GetFiles(App) orderby vers descending select vers.ToString()).ToArray();
                    string js = File.ReadAllText(VersionsApps[0]);
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    VersionMeta meta = serializer.Deserialize<VersionMeta>(js);
                    VersionNumber Vers = new VersionNumber(meta.Num, meta.Bild);
                    if (version.VersionNumber < Vers)
                    {
                        NewVersion = new AppVersion(version.VersionName, Vers);
                    }
                    AppVersions.Add(NewVersion);
                }
                return AppVersions;
            }
            catch (VersionInFileSystemRepo.AppVersionIsNullException ex)
            {
                return null;
                throw new AppVersionIsNullException();
            }
        }
        ///<summary>Список всех версий приложения</summary><param name="version_name"></param>
        public IList<AppVersion> FindAll(string version_name)
        {
            List<AppVersion> AppVersions = new List<AppVersion>();
            try
            {
                AppVersions.Add(FindLast(version_name));
                return AppVersions;
            }
            catch (VersionInFileSystemRepo.AppVersionIsNullException ex)
            {
                return null;
                throw new AppVersionIsNullException();
            }
        }
        #endregion
        #region Tools
        private static byte[] fileContain(string file_path)
        {
            byte[] file_contain = null;
            try
            {
                using (FileStream fstream = File.OpenRead(file_path))
                {
                    file_contain = new byte[fstream.Length];
                    fstream.Read(file_contain, 0, file_contain.Length);
                    fstream.Close();
                }
            }
            catch (FileContainIsNullException ex)
            {
                throw new FileContainIsNullException();
            }
            return file_contain;
        }
        private List<string> GetFiles_app(string nameApp, VersionNumber version)
        {
            List<string> ls = new List<string>();
            try
            {
                if (version != null)
                {
                    string z = Path.Combine(RootPath, nameApp);
                    var dirs = Directory.GetFiles(z, "*", SearchOption.AllDirectories).Select(x => x.Replace(z, ""));
                    return new List<string>(dirs);
                }
            }
            catch (FilePathIsNullException ex)
            {

                throw new FilePathIsNullException();
            }
            return ls;
        }
        #endregion
    }
}
