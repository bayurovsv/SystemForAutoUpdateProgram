using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using AppVersionControl.Infrastructure.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AppVersionControl.Infrastructure.Bilders
{
    /// <summary>Сборщик версии приложения по задонному пути</summary>
    public class AppBuilder : IApplicationVersionBuilder
    {
        ///<summary>Путь к приложению</summary>
        private readonly string RootPath;
        ///<summary>Имя приложения</summary>
        private string ApplicationName;
        ///<summary>Номер версии</summary>
        private VersionNumber AppVersionNumber;
        public AppBuilder(string rootPath, string appName, VersionNumber appVerNumber)
        {
            RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            ApplicationName = appName ?? throw new ArgumentNullException(nameof(appName));
            AppVersionNumber = appVerNumber ?? throw new ArgumentNullException(nameof(appVerNumber));
        }
        ///<summary>Сборка приложения</summary>
        public AppVersion Build()
        {
            return new AppVersion(ApplicationName, AppVersionNumber, FindFileOfVersions());
        }
        /// <summary>Получение списка метаданных файлов приложения</summary>
        private List<FileOfVersion> FindFileOfVersions()
        {
            List<FileOfVersion> fileApp = new List<FileOfVersion>();
            List<string> ListFile = new List<string>();
            ListFile = GetFiles_app(RootPath);
            fileApp.AddRange(from string v in ListFile
                             let verss = new FileOfVersion(v, fileContain(RootPath + v), File.GetCreationTime(RootPath + v))
                             select verss);
            return fileApp;
        }
        /// <summary>Получение списка содержимого файлов приложения</summary>
        private List<byte[]> FindByteFileOfVersions()
        {
            List<byte[]> fileApp = new List<byte[]>();
            List<string> ListFile = new List<string>();
            ListFile = GetFiles_app(RootPath);
            fileApp.AddRange(from string v in ListFile
                             select fileContain(RootPath + v));
            return fileApp;
        }
        /// <summary>Получение колекции метаданных и содержимого файлов приложения</summary>
        public IDictionary<FileOfVersion, byte[]> FindFileOfVersionsWithData()
        {
            IDictionary<FileOfVersion, byte[]> result = new Dictionary<FileOfVersion, byte[]>();
            try
            {
                IList<FileOfVersion> FileVersion = FindFileOfVersions();
                IList<byte[]> ByteFile = FindByteFileOfVersions();
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
        private List<string> GetFiles_app(string path)
        {
            List<string> ls = new List<string>();
            try
            {
                var dirs = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Select(x => x.Replace(path, ""));
                return new List<string>(dirs);
            }
            catch (FilePathIsNullException ex)
            {

                throw new FilePathIsNullException();
            }
        }
        private byte[] fileContain(string file_path)
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
        private static string HashCode(byte[] text)
        {
            string hash;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                hash = BitConverter.ToString(md5.ComputeHash(text)).Replace("-", string.Empty);
            }
            return hash;
        }
        #endregion
    }
}
