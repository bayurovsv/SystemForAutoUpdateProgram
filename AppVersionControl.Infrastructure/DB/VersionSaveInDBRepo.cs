using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using AppVersionControl.Infrastructure.FileSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace AppVersionControl.Infrastructure
{
    /// <summary>Сохранение метаданных версии приложения</summary>
    public class VersionSaveInDBRepo : IVersionSaveRepo
    {
        private static DB.SQLite.Logger logger = new DB.SQLite.Logger(AppDomain.CurrentDomain.BaseDirectory);
        /// <summary>Путь к базе данных</summary>
        private readonly string BDPath;
        private DB.SQLite.SQLite mydb = null;
        public VersionSaveInDBRepo(string bd_path)
        {
            BDPath = bd_path ?? throw new ArgumentNullException(nameof(bd_path));
        }
        /// <summary>Сохранение метаданных</summary><param name="version"></param>
        public bool SaveApplication(AppVersion version)
        {
            try
            {
                mydb = new DB.SQLite.SQLite(BDPath);
                IVersionRepo repo = new VersionInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
                IList<AppVersion> apps = repo.FindAll(version.VersionName);
                if (apps.Count == 0)
                {
                    SQLiteCommand command = new SQLiteCommand();
                    command.CommandText = "INSERT INTO Applications(ApplicationName) VALUES(@ApplicationName)";
                    command.Parameters.Add("@ApplicationName", DbType.String).Value = version.VersionName;
                    mydb.Insert(command);
                }
                for (int i = 0; i < version.Files.Count; i++)
                {
                    mydb = new DB.SQLite.SQLite(BDPath);
                    SQLiteCommand command1 = new SQLiteCommand();
                    command1.CommandText = "INSERT INTO FilesVersions(AppVersions,FileHash,CreationDate,FileSize,FilePath,DateCreateDB,Applications)" +
                    "VALUES(@AppVersions,@FileHash,@CreationDate,@FileSize,@FilePath,@DateCreateDB,@Applications)";
                    command1.Parameters.Add("@AppVersions", DbType.String).Value = version.VersionNumber.ToString();
                    command1.Parameters.Add("@FileHash", DbType.String).Value = version.Files[i].FileHash;
                    command1.Parameters.Add("@CreationDate", DbType.String).Value = DateTime.Now.ToString();
                    command1.Parameters.Add("@FileSize", DbType.Int32).Value = version.Files[i].FileSize.FileSize;
                    command1.Parameters.Add("@FilePath", DbType.String).Value = version.Files[i].FilePath;
                    command1.Parameters.Add("@DateCreateDB", DbType.String).Value = DateTime.Now.ToString();
                    command1.Parameters.Add("@Applications", DbType.String).Value = version.VersionName;
                    mydb.Insert(command1);
                }
                mydb = new DB.SQLite.SQLite(BDPath);
                SQLiteCommand command2 = new SQLiteCommand();
                command2.CommandText = "INSERT INTO ApplicationInfo(Application,AppVersion)VALUES(@Applications,@AppVersions)";
                command2.Parameters.Add("@Applications", DbType.String).Value = version.VersionName;
                command2.Parameters.Add("@AppVersions", DbType.String).Value = version.VersionNumber.ToString();
                mydb.Insert(command2);
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
