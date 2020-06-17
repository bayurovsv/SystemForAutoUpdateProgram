using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using AppVersionControl.Infrastructure.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace AppVersionControl.Infrastructure.FileSystemOneVers
{
    /// <summary>Получение информации о файлах версии</summary>
    public class FileInSystemRepoOne : IFileRepo
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

        /// <summary>Путь к приложению</summary>
        private readonly string RootPath;
        public FileInSystemRepoOne(string root_path)
        {
            RootPath = root_path ?? throw new ArgumentNullException(nameof(root_path));
            RootPath = Path.Combine(RootPath, "Versions");
        }
        #region IList
        /// <summary>Список содержимого файлов приложения</summary><param name="ver"></param>
        public IList<byte[]> FindByteFileOfVersions(AppVersion ver)
        {
            string[] Apps = Directory.GetDirectories(RootPath);// массив приложений(папки с названиями всех приложений) 
            string App = (from app in Apps where (Path.GetFileNameWithoutExtension(app.ToLower()) == ver.VersionName.ToLower()) select app).Single();//Строка содержащая суть к нужному приложению(к версиям)
            string VersionsApps = (from vers in Directory.GetFiles(App) where (Path.GetFileNameWithoutExtension(vers) == ver.VersionName + "-" + ver.VersionNumber.ToString()) select vers).Single();//отсортированный по убыванию массив версий 
            string js = File.ReadAllText(VersionsApps);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            VersionMeta meta = serializer.Deserialize<VersionMeta>(js);
            VersionNumber Vers = new VersionNumber(meta.Num, meta.Bild);
            List<byte[]> fileApp = new List<byte[]>();
            List<string> ListFile = new List<string>();
            ListFile = GetFiles_app(ver.VersionName, Vers);
            for (int x = 0; x < ListFile.Count; x++)
            {
                fileApp.Add(fileContain(RootPath + "\\" + ver.VersionName + ListFile[x]));
            }
            return fileApp;
        }
        /// <summary>Список метаданных файлов приложения</summary><param name="ver"></param>
        public IList<FileOfVersion> FindFileOfVersions(AppVersion ver)
        {
            try
            {
                string[] Apps = Directory.GetDirectories(RootPath);// массив приложений(папки с названиями всех приложений) 
                string App = (from app in Apps where (Path.GetFileNameWithoutExtension(app.ToLower()) == ver.VersionName.ToLower()) select app).Single();//Строка содержащая суть к нужному приложению(к версиям)
                string VersionsApps = (from vers in Directory.GetFiles(App) where (Path.GetFileNameWithoutExtension(vers) == ver.VersionName + "-" + ver.VersionNumber.ToString()) select vers).Single();//отсортированный по убыванию массив версий 
                string js = File.ReadAllText(VersionsApps);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                VersionMeta meta = serializer.Deserialize<VersionMeta>(js);
                VersionNumber Vers = new VersionNumber(meta.Num, meta.Bild);
                List<FileOfVersion> fileApp = new List<FileOfVersion>();
                List<string> ListFile = new List<string>();
                ListFile = GetFiles_app(ver.VersionName, Vers);
                for (int x = 0; x < ListFile.Count; x++)
                {
                    FileOfVersion verss = new FileOfVersion(ListFile[x], fileContain(RootPath + "\\" + ver.VersionName + ListFile[x]), File.GetCreationTime(RootPath + "\\" + ver.VersionName + ListFile[x]));
                    fileApp.Add(verss);
                }
                return fileApp;
            }
            catch (FilePathIsNullException ex)
            {
                throw new FilePathIsNullException();
            }

        }
        #endregion
        #region GetFile
        public byte[] GetFile(AppVersion version, FileOfVersion name)
        {
            try
            {
                return fileContain(RootPath + "\\" + version.VersionName + "\\" + name.FilePath);
            }
            catch
            {
                throw new FilePathIsNullException(); ;
            }

        }

        public byte[] GetFile(string versionName, string versionNumber, string relative_file_path)
        {
            try
            {
                return fileContain(RootPath + "\\" + versionName + "\\" + relative_file_path);
            }
            catch
            {
                throw new FilePathIsNullException(); ;
            }
        }
        #endregion
        #region Save
        /// <summary>Сохранение содержимого файлов приложения</summary><param name="ver"></param><param name="file"></param>
        public bool Save(AppVersion ver, IDictionary<FileOfVersion, byte[]> file)
        {
            try
            {
                if (Directory.Exists(RootPath + "\\" + ver.VersionName))
                {
                    Directory.Delete(RootPath + "\\" + ver.VersionName,true);
                }
                Directory.CreateDirectory(RootPath + "\\" + ver.VersionName);
                List<FileOfVersion> fileVersion = (from dir in file.Keys select dir).ToList();
                List<byte[]> fileContains = (from contains in file.Values select contains).ToList();
                for (int i = 0; i < fileVersion.Count; i++)
                {
                    if (fileVersion[i].FileHash == HashCode(fileContains[i]))
                    {
                        Directory.CreateDirectory(RootPath + "\\" + ver.VersionName + "\\" + Path.GetDirectoryName(fileVersion[i].FilePath));
                        using (FileStream fileStream = new FileStream(RootPath + "\\" + ver.VersionName + "\\" + fileVersion[i].FilePath, FileMode.Create))
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
        #region Tools
        private static string HashCode(byte[] text)//HashCode
        {
            string hash;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                hash = BitConverter.ToString(md5.ComputeHash(text)).Replace("-", string.Empty);
            }
            return hash;
        }
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
