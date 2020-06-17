using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AppVersionControl.Infrastructure.FileSystem
{
    /// <summary>Получение информации о файлах версии</summary>
    public class FileInSystemRepo : IFileRepo
    {
        /// <summary>Путь к приложению</summary>
        private readonly string RootPath;
        public FileInSystemRepo(string root_path)
        {
            RootPath = root_path ?? throw new ArgumentNullException(nameof(root_path));
            RootPath = Path.Combine(RootPath, "Versions");
        }
        #region IList
        /// <summary>Список метаданных файлов приложения</summary><param name="ver"></param>
        public IList<FileOfVersion> FindFileOfVersions(AppVersion ver)
        {
            try
            {
                string[] Apps = Directory.GetDirectories(RootPath);// массив приложений(папки с названиями всех приложений) 
                string App = (from app in Apps where (Path.GetFileNameWithoutExtension(app.ToLower()) == ver.VersionName.ToLower()) select app).Single();//Строка содержащая путь к нужному приложению(к версиям)
                string[] VersionsApps = (from vers in Directory.GetDirectories(App) orderby vers descending select vers.ToString()).ToArray();//отсортированный по убыванию массив версий 
                VersionNumber Vers = Directory.GetDirectories(App).Select(x =>
                {
                    VersionNumber version;
                    VersionNumber.TryParse(Path.GetFileNameWithoutExtension(x.ToString()), out version);
                    return version;
                }).Where(x => x != null).OrderByDescending(x => x).First();
                List<FileOfVersion> fileApp = new List<FileOfVersion>();
                List<string> ListFile = new List<string>();
                ListFile = GetFiles_app(ver.VersionName, Vers);
                fileApp.AddRange(from string v in ListFile
                                 let verss = new FileOfVersion(v, fileContain(RootPath + "\\" + ver.VersionName + "\\" + Vers.ToString() + v), File.GetCreationTime(RootPath + "\\" + ver.VersionName + "\\" + Vers.ToString() + v))
                                 select verss);
                return fileApp;
            }
            catch (FilePathIsNullException ex)
            {
                throw new FilePathIsNullException();
            }
        }
        /// <summary>Список содержимого файлов приложения</summary><param name="ver"></param>
        public IList<byte[]> FindByteFileOfVersions(AppVersion ver)
        {
            try
            {
                string[] Apps = Directory.GetDirectories(RootPath);// массив приложений(папки с названиями всех приложений) 
                string App = (from app in Apps where (Path.GetFileNameWithoutExtension(app.ToLower()) == ver.VersionName.ToLower()) select app).Single();//Строка содержащая суть к нужному приложению(к версиям)
                string[] VersionsApps = (from vers in Directory.GetDirectories(App) orderby vers descending select vers.ToString()).ToArray();//отсортированный по убыванию массив версий 
                VersionNumber Vers = Directory.GetDirectories(App).Select(x =>
                {
                    VersionNumber version;
                    VersionNumber.TryParse(Path.GetFileNameWithoutExtension(x.ToString()), out version);
                    return version;
                }).Where(x => x != null).OrderByDescending(x => x).First(); 
                List<byte[]> fileApp = new List<byte[]>();
                List<string> ListFile = new List<string>();
                ListFile = GetFiles_app(ver.VersionName, Vers);
                fileApp.AddRange(from string v in ListFile
                                 select fileContain(RootPath + "\\" + ver.VersionName + "\\" + Vers.ToString() + v));
                return fileApp;
            }
            catch (FilePathIsNullException ex)
            {
                throw new FilePathIsNullException();
            }
        }
        #endregion
        #region GetFile
        /// <summary>Получение содержимого файла версии приложения</summary><param name="version"></param><param name="name"></param><returns>byte[]</returns>
        public byte[] GetFile(AppVersion version, FileOfVersion name)
        {
            try
            {
                return fileContain(RootPath + "\\" + version.VersionName + "\\" + version.VersionNumber.ToString() + "\\" + name.FilePath);
            }
            catch
            {
                throw new FilePathIsNullException(); ;
            }
        }
        /// <summary>Получение содержимого файла версии приложения</summary><param name="versionName"></param><param name="versionNumber"></param><param name="relative_file_path"></param><returns>byte[]</returns>
        public byte[] GetFile(string versionName, string versionNumber, string relative_file_path)
        {
            try
            {
                return fileContain(RootPath + "\\" + versionName + "\\" + versionNumber + "\\" + relative_file_path);
            }
            catch
            {
                throw new FilePathIsNullException(); ;
            }
        }
        #endregion
        #region Save
        /// <summary>Сохранение содержимого файлов приложения</summary><param name="ver"></param><param name="file"></param>
        public bool Save(AppVersion version, IDictionary<FileOfVersion, byte[]> file)
        {
            try
            {
                Directory.CreateDirectory(RootPath + "\\" + version.VersionName + "\\" + version.VersionNumber.ToString());
                List<FileOfVersion> fileVersion = (from dir in file.Keys select dir).ToList();
                List<byte[]> fileContains = (from contains in file.Values select contains).ToList();
                for (int i = 0; i < fileVersion.Count; i++)
                {
                    if (fileVersion[i].FileHash == HashCode(fileContains[i]))
                    {
                        Directory.CreateDirectory(RootPath + "\\" + version.VersionName + "\\" + version.VersionNumber.ToString() + "\\" + Path.GetDirectoryName(fileVersion[i].FilePath));
                        using (FileStream fileStream = new FileStream(RootPath + "\\" + version.VersionName + "\\" + version.VersionNumber.ToString() + "\\" + fileVersion[i].FilePath, FileMode.Create))
                        {
                            fileStream.Write(file[fileVersion[i]], 0, fileContains[i].Length);
                        }
                    }
                    else
                    {
                        throw new Exception("Метаданные не соответствуют содержимому файла!");
                    }
                }
                return true;
            }
            catch (SaveException)
            {
                throw new SaveException();
            }
        }
        #endregion
        #region IDictionary
        /// <summary>Коллекция метаданных и содержимого файлов</summary><param name="ver"></param>
        public IDictionary<FileOfVersion, byte[]> FindFileOfVersionsWithData(AppVersion ver)
        {
            IDictionary<FileOfVersion, byte[]> result = new Dictionary<FileOfVersion, byte[]>();
            try
            {
                IList<FileOfVersion> FileVersion = FindFileOfVersions(ver);
                IList<byte[]> ByteFile = FindByteFileOfVersions(ver);
                for (int i = 0; i < FileVersion.Count; i++)
                {
                    if (FileVersion[i].FileHash == HashCode(ByteFile[i]))
                        result.Add(FileVersion[i], ByteFile[i]);
                }
                return result;
            }
            catch (FileOrByteIsNullException)
            {
                throw new FileOrByteIsNullException();
            }
        }
        #endregion
        #region Tools
        ///<summary>Hashcode</summary><param name="text"></param>
        private static string HashCode(byte[] text)
        {
            string hash;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                hash = BitConverter.ToString(md5.ComputeHash(text)).Replace("-", string.Empty);
            }
            return hash;
        }
        ///<summary>Содержимое файла</summary><param name="file_path"></param>
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
            catch (VersionInFileSystemRepo.FilePathIsNullException ex)
            {
                throw new NotImplementedException();
            }
            return file_contain;
        }
        ///<summary>Список всех файлов версии(пути к файлам)</summary><param name="nameApp"></param><param name="version"></param>
        private List<string> GetFiles_app(string nameApp, VersionNumber version)
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
    }
    #region Domain exception
    public class FilePathIsNullException : Exception
    {
        public FilePathIsNullException()
             : base("Версии не найдены")
        {
        }
    }
    public class FileOrByteIsNullException : Exception
    {
        public FileOrByteIsNullException()
             : base("Не удалось получить набор байт и метаданные файла")
        {
        }
    }
    public class SaveException : Exception
    {
        public SaveException()
             : base("Ошибка сохранения")
        {
        }
    }
    public class SaveApplicationException : Exception
    {
        public SaveApplicationException()
             : base("Ошибка сохранения дынных о приложении")
        {
        }
    }
    #endregion
}

