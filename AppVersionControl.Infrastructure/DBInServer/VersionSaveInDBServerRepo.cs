using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using AppVersionControl.Infrastructure.FileSystem;
using ServerSQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace AppVersionControl.Infrastructure.DBInServer
{
    /// <summary>Сохранение метаданных версии приложения</summary>
    public class VersionSaveInDBServerRepo : IVersionSaveRepo
    {

        private static DB.SQLite.Logger logger = new DB.SQLite.Logger(AppDomain.CurrentDomain.BaseDirectory);
        /// <summary>Порт сервера</summary>
        private static int Port;
        /// <summary>Адрес сервера</summary>
        private static string Address;
        private ClientControlService clientControl;
        public VersionSaveInDBServerRepo(string address, int port)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            if (port < 0) throw new Exception(nameof(port));
            Port = port;
        }
        /// <summary>Сохранение метаданных</summary><param name="version"></param>
        public bool SaveApplication(AppVersion version)
        {
            try
            {
                clientControl = new ClientControlService(Address, Port);
                IVersionRepo repo = new VersionInDBServerRepo("127.0.0.1", 8005);
                IList<AppVersion> apps = repo.FindAll(version.VersionName);
                if (apps.Count == 0)
                {
                    SQLiteCommand command = new SQLiteCommand();
                    command.CommandText = "INSERT INTO Applications (ApplicationName) VALUES(@ApplicationName)";
                    command.Parameters.Add("@ApplicationName", DbType.String).Value = version.VersionName;
                    clientControl.Insert(command);
                }
                for (int i = 0; i < version.Files.Count; i++)
                {
                    clientControl = new ClientControlService(Address, Port);
                    SQLiteCommand command1 = new SQLiteCommand();
                    command1.CommandText = "INSERT INTO FilesVersions (AppVersions,FileHash,CreationDate,FileSize,FilePath,DateCreateDB,Applications)" +
                   "VALUES(@AppVersions,@FileHash,@CreationDate,@FileSize,@FilePath,@DateCreateDB,@Applications)";
                    command1.Parameters.Add("@AppVersions", DbType.String).Value = version.VersionNumber.ToString();
                    command1.Parameters.Add("@FileHash", DbType.String).Value = version.Files[i].FileHash;
                    command1.Parameters.Add("@CreationDate", DbType.String).Value = DateTime.Now.ToString();
                    command1.Parameters.Add("@FileSize", DbType.Int32).Value = version.Files[i].FileSize.FileSize;
                    command1.Parameters.Add("@FilePath", DbType.String).Value = version.Files[i].FilePath;
                    command1.Parameters.Add("@DateCreateDB", DbType.String).Value = DateTime.Now.ToString();
                    command1.Parameters.Add("@Applications", DbType.String).Value = version.VersionName;
                    clientControl.Insert(command1);
                }
                clientControl = new ClientControlService(Address, Port);
                SQLiteCommand command2 = new SQLiteCommand();
                command2.CommandText = "INSERT INTO ApplicationInfo (Application,AppVersion)VALUES(@Applications,@AppVersions)";
                command2.Parameters.Add("@Applications", DbType.String).Value = version.VersionName;
                command2.Parameters.Add("@AppVersions", DbType.String).Value = version.VersionNumber.ToString();
                clientControl.Insert(command2);
                logger.Add("Запись добавлена!");
                return true;
            }
            catch (SaveException ex)
            {
                logger.Add(ex);
                throw new SaveException();
            }
        }
    }
}
