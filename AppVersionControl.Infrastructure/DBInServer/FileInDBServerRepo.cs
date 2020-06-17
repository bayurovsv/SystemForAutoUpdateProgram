using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using AppVersionControl.Infrastructure.FileSystem;
using ServerSQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace AppVersionControl.Infrastructure.DBInServer
{
    /// <summary>Получение информации о файлах версии</summary>
    public class FileInDBServerRepo : IFileRepo
    {
        private static DB.SQLite.Logger logger = new DB.SQLite.Logger(AppDomain.CurrentDomain.BaseDirectory);
        /// <summary>Порт сервера</summary>
        private static int Port;
        /// <summary>Адрес сервера</summary>
        private static string Address;
        private ClientControlService clientControl;
        public FileInDBServerRepo(string address, int port)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            if (port < 0) throw new Exception(nameof(port));
            Port = port;
        }
        #region IList 
        /// <summary>Список содержимого файлов приложения</summary><param name="ver"></param>
        public IList<byte[]> FindByteFileOfVersions(AppVersion ver)
        {
            try
            {
                clientControl = new ClientControlService(Address, Port);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "SELECT * FROM FilesContains WHERE Applications=@Applications";
                command.Parameters.Add("@Applications", DbType.String).Value = ver.VersionName;
                DataRow[] datarows = clientControl.Select(command);
                if (datarows == null)// проверка что запрос что-то вернул
                {
                    logger.Add("Не удалось подключиться к базе данных");
                    throw new FilePathIsNullException();
                }
                string[] versss = (from vers in datarows orderby vers.ItemArray[0] descending select vers.ItemArray[1].ToString()).ToArray();//поиск наибольшей версии 
                VersionNumber Vers;
                VersionNumber.TryParse(versss[0], out Vers);
                List<byte[]> fileApp = new List<byte[]>();
                List<string> ListFile = new List<string>();
                foreach (var dr in from DataRow dr in datarows where Vers.ToString() == dr["AppVersions"].ToString().Trim() select dr)// Заполнение списка мета данных файлов
                {
                    fileApp.Add((byte[])dr["Contains"]);
                }
                return fileApp;
            }
            catch (VersionInFileSystemRepo.FilePathIsNullException ex)
            {
                logger.Add(ex);
                throw new FilePathIsNullException();
            }
        }
        /// <summary>Список метаданных файлов приложения</summary><param name="ver"></param>
        public IList<FileOfVersion> FindFileOfVersions(AppVersion ver)// попробовать рпеределать не по индексу а по ключу
        {
            try
            {
                clientControl = new ClientControlService(Address, Port);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "SELECT * FROM FilesContains WHERE Applications = @Applications";
                command.Parameters.Add("@Applications", DbType.String).Value = ver.VersionName;
                DataRow[] datarows = clientControl.Select(command);
                if (datarows == null)
                {
                    logger.Add("Не удалось подключиться к базе данных");
                    throw new FilePathIsNullException();
                }
                string[] versss = (from vers in datarows orderby vers.ItemArray[0] descending select vers.ItemArray[1].ToString()).ToArray();//поиск наибольшей версии 
                VersionNumber Vers;
                VersionNumber.TryParse(versss[0], out Vers);
                List<FileOfVersion> fileApp = new List<FileOfVersion>();
                foreach (var dr in from DataRow dr in datarows where Vers.ToString() == dr[1].ToString().Trim() select dr)// Заполнение списка мета данных файлов
                {
                    FileOfVersion verss = new FileOfVersion(dr[2].ToString(),
                    (byte[])dr[4], File.GetCreationTime(dr[2].ToString()));
                    fileApp.Add(verss);
                }
                return fileApp;
            }
            catch (VersionInFileSystemRepo.FilePathIsNullException ex)
            {
                logger.Add(ex);
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
                byte[] contains = null;
                clientControl = new ClientControlService(Address, Port);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "SELECT * FROM FilesContains WHERE Applications = @Applications AND AppVersions = @AppVersions AND FilePath = @FilePath";
                command.Parameters.Add("@Applications", DbType.String).Value = version.VersionName;
                command.Parameters.Add("@AppVersions", DbType.String).Value = version.VersionNumber.ToString();
                command.Parameters.Add("@FilePath", DbType.String).Value = name.FilePath;
                DataRow[] datarows = clientControl.Select(command);
                if (datarows == null)
                {
                    logger.Add("Не удалось подключиться к базе данных");
                    throw new FilePathIsNullException();
                }
                foreach (var dr in from DataRow dr in datarows select dr)
                {
                    contains = (byte[])dr["Contains"];
                }
                return contains;
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
                byte[] contains = null;
                clientControl = new ClientControlService(Address, Port);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "SELECT * FROM FilesContains WHERE Applications = @Applications AND AppVersions = @AppVersions AND FilePath = @FilePath";
                command.Parameters.Add("@Applications", DbType.String).Value = versionName;
                command.Parameters.Add("@AppVersions", DbType.String).Value = versionNumber;
                command.Parameters.Add("@FilePath", DbType.String).Value = relative_file_path;
                DataRow[] datarows = clientControl.Select(command);
                if (datarows == null)
                {
                    logger.Add("Не удалось подключиться к базе данных");
                    throw new FilePathIsNullException();
                }
                foreach (var dr in from DataRow dr in datarows select dr)
                {
                    contains = (byte[])dr["Contains"];
                }
                return contains;
            }
            catch
            {
                throw new FilePathIsNullException(); ;
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
        #region Save
        /// <summary>Сохранение содержимого файлов приложения</summary><param name="ver"></param><param name="file"></param>
        public bool Save(AppVersion ver, IDictionary<FileOfVersion, byte[]> file)
        {
            try
            {
                List<FileOfVersion> fileVersion = (from dir in file.Keys select dir).ToList();
                List<byte[]> fileContains = (from contains in file.Values select contains).ToList();
                for (int i = 0; i < fileVersion.Count; i++)
                {
                    if (fileVersion[i].FileHash == HashCode(fileContains[i]))
                    {
                        clientControl = new ClientControlService(Address, Port);
                        SQLiteCommand command = new SQLiteCommand();
                        command.CommandText = "INSERT INTO FilesContains (Applications,AppVersions,FilePath,DateCreateDB,Contains) VALUES(@Applications,@AppVersions,@FilePath,@DateCreateDB,@Contains)";
                        command.Parameters.Add("@Applications", DbType.String).Value = ver.VersionName;
                        command.Parameters.Add("@AppVersions", DbType.String).Value = ver.VersionNumber.ToString();
                        command.Parameters.Add("@FilePath", DbType.String).Value = fileVersion[i].FilePath;
                        command.Parameters.Add("@DateCreateDB", DbType.String).Value = DateTime.Now.ToString();
                        command.Parameters.Add("@Contains", DbType.Binary).Value = fileContains[i].ToArray();
                        clientControl.Insert(command);
                        logger.Add("Запись добавлена!");
                    }
                    else
                    {
                        throw new Exception("Метаданные не соответствуют содержимому файла!");
                    }
                }
                return true;
            }
            catch (SaveException ex)
            {
                logger.Add(ex);
                throw new SaveException();
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
        #endregion
    }
}
