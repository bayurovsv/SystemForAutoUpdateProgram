using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AppVersionControl.Infrastructure
{
    /// <summary>Получение информации о версии приложения</summary>
    public class VersionInFileSystemRepo : IVersionRepo
    {
        /// <summary>Путь к версии</summary>
        private readonly string RootPath;
        public VersionInFileSystemRepo(string root_path)
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
                FileOfVersion verss = new FileOfVersion(ListFile[x], fileContain(RootPath + "\\" + version_name + "\\" + versionNumber.ToString() + ListFile[x]), File.GetCreationTime(RootPath + "\\" + version_name + "\\" + versionNumber.ToString() + ListFile[x]));
                fileApp.Add(verss);
            }
            AppVersion res = null;
            res = new AppVersion(Path.GetFileNameWithoutExtension(App), versionNumber, fileApp);
            return res;
        } 
        private AppVersion Find_(string name)
        {
            try
            {
                string[] Apps = Directory.GetDirectories(RootPath);// массив приложений(папки с названиями всех приложений) 
                string App = (from app in Apps where (Path.GetFileNameWithoutExtension(app.ToLower()) == name.ToLower()) select app).Single();//Строка содержащая суть к нужному приложению(к версиям)
                string[] VersionsApps = (from vers in Directory.GetDirectories(App) orderby vers descending select vers.ToString()).ToArray();//отсортированный по убыванию массив версий 
                VersionNumber Vers = Directory.GetDirectories(App).Select(x =>
                  {
                      VersionNumber version;
                      VersionNumber.TryParse(Path.GetFileNameWithoutExtension(x.ToString()), out version);
                      return version;
                  }).Where(x => x!= null).OrderByDescending(x=>x).First();
                List<FileOfVersion> fileApp = new List<FileOfVersion>();
                List<string> ListFile = new List<string>();
                ListFile = GetFiles_app(name ,Vers);
                for (int x = 0; x < ListFile.Count; x++)
                {
                    FileOfVersion verss = new FileOfVersion(ListFile[x], fileContain(RootPath + "\\" + name + "\\" + Vers.ToString() + ListFile[x]), File.GetCreationTime(RootPath + "\\" + name + "\\" + Vers.ToString() + ListFile[x]));
                    fileApp.Add(verss);
                }
                AppVersion res = null;
                res = new AppVersion(Path.GetFileNameWithoutExtension(App), Vers, fileApp);
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
        ///<summary>Список всех версий приложения</summary><param name="version_name"></param>
        public IList<AppVersion> FindAll(string version_name)
        {
            List<AppVersion> AppVersions = new List<AppVersion>();
            try
            {
                string[] Apps = Directory.GetDirectories(RootPath);// массив приложений(папки с названиями всех приложений) 
                string App = (from app in Apps where (Path.GetFileNameWithoutExtension(app.ToLower()) == version_name.ToLower()) select app).Single();//Строка содержащая суть к нужному приложению(к версиям)
                string[] VersionsApps = (from vers in Directory.GetDirectories(App) orderby vers select vers.ToString()).ToArray();//отсортированный  массив версий 
                foreach (string Version in VersionsApps)
                {
                    VersionNumber VerNum;
                    VersionNumber.TryParse(Path.GetFileNameWithoutExtension(Version), out VerNum);
                    AppVersion AppVer = new AppVersion(Path.GetFileNameWithoutExtension(App), VerNum);
                    AppVersions.Add(AppVer);
                }
                return AppVersions;
            }
            catch (VersionInFileSystemRepo.AppVersionIsNullException ex)
            {
                throw new AppVersionIsNullException();
            }
        }
        ///<summary>Список всех версий приложений</summary>
        public IList<AppVersion> FindAll()
        {
            List<AppVersion> AppVersions = new List<AppVersion>();
            try
            {
                foreach (string vers in Directory.GetDirectories(RootPath))
                {
                    AppVersion version = new AppVersion(Path.GetFileNameWithoutExtension(vers), new VersionNumber(1, 0));
                    AppVersion NewVersion = new AppVersion(Path.GetFileNameWithoutExtension(vers), new VersionNumber(1, 0));
                    foreach (string fov in Directory.GetDirectories(vers).ToArray())
                    {
                        VersionNumber VerNum;
                        VersionNumber.TryParse(Path.GetFileNameWithoutExtension(fov), out VerNum);
                        if (version.VersionNumber < VerNum)
                        {
                            NewVersion = new AppVersion(version.VersionName, VerNum);
                        }
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
        private List<string> GetFiles_app(string nameApp,VersionNumber version)
        {
            List<string> ls = new List<string>();
            try
            {
                if (version != null)
                {
                    string z = Path.Combine(RootPath, nameApp);
                    z = Path.Combine(z, version.ToString());
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
        #region Domain exception
        public class FilePathIsNullException : Exception
        {
            public FilePathIsNullException()
                 : base("Версии не найдены")
            {
            }
        }
        public class AppVersionIsNullException : Exception
        {
            public AppVersionIsNullException()
                 : base("Версия приложения не найдена")
            {
            }
        }
        public class FileContainIsNullException : Exception
        {
            public FileContainIsNullException()
                 : base("Не удалось получить содержимое файла")
            {
            }
        }
        #endregion
    }
}