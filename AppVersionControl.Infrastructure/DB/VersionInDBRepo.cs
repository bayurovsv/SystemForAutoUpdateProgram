using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace AppVersionControl.Infrastructure
{
    /// <summary>Получение информации о версии приложения</summary>
    public class VersionInDBRepo : IVersionRepo
    {
        private static DB.SQLite.Logger logger = new DB.SQLite.Logger(AppDomain.CurrentDomain.BaseDirectory);
        /// <summary>Путь к базе данных</summary>
        private readonly string BDPath;
        private DB.SQLite.SQLite mydb = null;
        public VersionInDBRepo(string bd_path)
        {
            BDPath = bd_path ?? throw new ArgumentNullException(nameof(bd_path));
        }
        #region Find
        ///<summary>Поиск последней версии приложения</summary><param name="version_name"></param>
        public AppVersion FindLast(string version_name)
        {
            try
            {
                mydb = new DB.SQLite.SQLite(BDPath);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "SELECT * FROM FilesVersions WHERE Applications = @Applications";
                command.Parameters.Add("@Applications", DbType.String).Value = version_name;
                logger.Add("Версия найдена");
                return Result(command);
            }
            catch (FilePathIsNullException ex)
            {
                logger.Add(ex);
                throw new FilePathIsNullException();
            }
        }
        ///<summary>Поиск последней версии приложения</summary><param name="app"></param>
        public AppVersion Find(AppVersion app)
        {
            try
            {
                mydb = new DB.SQLite.SQLite(BDPath);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "SELECT * FROM FilesVersions WHERE Applications = @Applications";
                command.Parameters.Add("@Applications", DbType.String).Value = app.VersionName;
                logger.Add("Версия найдена");
                return Result(command);
            }
            catch (FilePathIsNullException ex)
            {
                logger.Add(ex);
                throw new FilePathIsNullException();
            }
        }
        ///<summary>Поиск последней версии приложения</summary><param name="version_name"></param><param name="versionNumber"></param>
        public AppVersion Find(string version_name, VersionNumber versionNumber)
        {
            try
            {
                mydb = new DB.SQLite.SQLite(BDPath);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "SELECT * FROM FilesVersions WHERE Applications = @Applications and AppVersions = @AppVersions";
                command.Parameters.Add("@Applications", DbType.String).Value = version_name;
                command.Parameters.Add("@AppVersions", DbType.String).Value = versionNumber.ToString();
                logger.Add("Версия найдена");
                return Result(command);
            }
            catch (FilePathIsNullException ex)
            {
                logger.Add(ex);
                throw new FilePathIsNullException();
            }
        }
        #endregion
        #region FindAll
        ///<summary>Всех версий приложения</summary><param name="version_name"></param>
        public IList<AppVersion> FindAll(string version_name)
        {
            try
            {
                mydb = new DB.SQLite.SQLite(BDPath);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "SELECT * FROM FilesVersions WHERE Applications = " + "'" + version_name + "'";
                command.Parameters.Add("@Applications", DbType.String).Value = version_name;
                return AppVersions(command);
            }
            catch (FilePathIsNullException ex)
            {
                logger.Add(ex);
                throw new FilePathIsNullException();
            }
        }
        ///<summary>Всех версий приложений</summary>
        public IList<AppVersion> FindAll()
        {
            try
            {
                mydb = new DB.SQLite.SQLite(BDPath); 
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "SELECT * FROM FilesVersions";
                return AppVersions(command);
            }
            catch (FilePathIsNullException ex)
            {
                logger.Add(ex);
                throw new FilePathIsNullException();
            }
        }
        #endregion
        #region Tools
        private AppVersion Result(SQLiteCommand command)
        {
            string appVersion = "";
            string app_name = "";
            DataRow[] datarows = mydb.Select(command);
            if (datarows == null)
            {
                logger.Add("Не удалось подключиться к базе данных");
                throw new FilePathIsNullException();
            }
            string[] versss = (from vers in datarows orderby vers.ItemArray[0] descending select vers.ItemArray[0].ToString()).ToArray();//поиск наибольшей версии 
            VersionNumber Vers;
            VersionNumber.TryParse(versss[0], out Vers);
            List<FileOfVersion> fileApp = new List<FileOfVersion>();
            foreach (var dr in from DataRow dr in datarows where Vers.ToString() == dr.ItemArray[0].ToString().Trim() select dr)
            {
                app_name = dr["Applications"].ToString();
                appVersion = dr["AppVersions"].ToString().Trim();
                FileOfVersion verss = new FileOfVersion(dr["FileHash"].ToString().Trim(), Convert.ToDateTime(dr["CreationDate"].ToString().Trim()), new AppFileSize(Convert.ToInt32(dr["FileSize"].ToString().Trim())), dr["FilePath"].ToString().Trim());
                fileApp.Add(verss);
            }
            VersionNumber version;
            VersionNumber.TryParse(appVersion, out version);
            AppVersion res = new AppVersion(app_name, version, fileApp);
            return res;
        }
        private List<AppVersion> AppVersions(SQLiteCommand command)
        {
            List<AppVersion> AppVersions = new List<AppVersion>();
            DataRow[] datarows = mydb.Select(command);
            if (datarows == null)
            {
                logger.Add("Не удалось подключиться к базе данных");
                throw new FilePathIsNullException();
            }
            foreach (DataRow dr in datarows)
            {
                VersionNumber VerNum;
                VersionNumber.TryParse(dr[0].ToString().Trim(), out VerNum);
                AppVersion AppVer = new AppVersion(dr.ItemArray[6].ToString(), VerNum);
                if (!AppVersions.Contains(AppVer))
                    AppVersions.Add(AppVer);
            }
            return AppVersions;
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
        #endregion
    }
}
