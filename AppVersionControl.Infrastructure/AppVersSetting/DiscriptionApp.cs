using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using System;
using AppVersionControl.Domain.ApplicationVersion.ApplicationVersionSettings;
using System.Data;
using System.Data.SQLite;

namespace AppVersionControl.Infrastructure.AppVersSetting
{
    /// <summary>Получении информациио об актуальной версии приложения</summary>
    public class DiscriptionApp : IDiscriptionApp
    {
        private DB.SQLite.SQLite mydb = null;
        private readonly string BDPath;
        public DiscriptionApp(string bd_path)
        {
            BDPath = bd_path ?? throw new ArgumentNullException(nameof(bd_path));
        }
        /// <summary>Получение версии приложения</summary><param name="appName"></param><returns></returns>
        public AppVersion GetAppVersion(string appName)
        {
            AppVersion AppVersions = null;
            mydb = new DB.SQLite.SQLite(BDPath);
            string flag = "True";
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ApplicationInfo WHERE Flag = @Flag AND Application = @Application";
            command.Parameters.Add("@Flag", DbType.String).Value = flag;
            command.Parameters.Add("@Application", DbType.String).Value = appName;
            DataRow[] datarows = mydb.Select(command);
            if (datarows == null)
            {
                throw new Exception();
            }
            foreach (DataRow dr in datarows)
            {
                VersionNumber VerNum;
                VersionNumber.TryParse(dr["AppVersion"].ToString().Trim(), out VerNum);
                AppVersion AppVer = new AppVersion(dr["Application"].ToString(), VerNum);
                AppVersions = (AppVer);
            }
            return AppVersions;
        }
        /// <summary>Получение имени исполняемого файла приложения</summary><param name="appName"></param><param name="appVersion"></param><returns></returns>
        public string GetNameExe(string appName, string appVersion)
        {
            mydb = new DB.SQLite.SQLite(BDPath);
            string flag = "True";
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ApplicationInfo WHERE Flag = @Flag AND Application = @Application AND AppVersion = @AppVersion";
            command.Parameters.Add("@Flag", DbType.String).Value = flag;
            command.Parameters.Add("@Application", DbType.String).Value = appName;
            command.Parameters.Add("@AppVersion", DbType.String).Value = appVersion;
            DataRow[] datarows = mydb.Select(command);
            if (datarows == null)
            {
                throw new Exception();
            }
            string name = "";
            foreach (var info in datarows)
            {
                name = info["Name_EXE"].ToString();
            }
            return name;
        }
        /// <summary>Получение описания приложения</summary><param name="appName"></param><param name="appVersion"></param><returns>discription</returns>
        public string GetDiscription(string appName, string appVersion)
        {
            mydb = new DB.SQLite.SQLite(BDPath);
            string flag = "True";
            string discription = "";
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ApplicationInfo WHERE Flag = @Flag AND Application = @Application AND AppVersion = @AppVersion";
            command.Parameters.Add("@Flag", DbType.String).Value = flag;
            command.Parameters.Add("@Application", DbType.String).Value = appName;
            command.Parameters.Add("@Application", DbType.String).Value = appVersion;
            DataRow[] datarows = mydb.Select(command);
            if (datarows == null)
            {
                throw new Exception();
            }
            foreach (DataRow dr in datarows)
            {
                discription = dr["AppDiscription"].ToString();
            }
            return discription;
        }
        /// <summary>Получение настроек сообщения</summary><param name="appName"></param><param name="appVersion"></param><returns>result</returns>
        public AppVersionSetting GetSetting(string appName, string appVersion)
        {
            try
            {
                mydb = new DB.SQLite.SQLite(BDPath);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "SELECT * FROM ApplicationInfo WHERE Application = @Application AND AppVersion = @AppVersion ";
                command.Parameters.Add("@Application", DbType.String).Value = appName;
                command.Parameters.Add("@AppVersion", DbType.String).Value = appVersion;
                DataRow[] datarows = mydb.Select(command);
                if (datarows == null)
                {
                    throw new Exception();
                }
                AppVersionSetting result = new AppVersionSetting();
                foreach (var info in datarows)
                {
                    result.ActialAppName = info["Aplication"].ToString();
                    result.ActialAppVersion = info["AppVersion"].ToString();
                    result.ActialFlag = info["Flag"].ToString();
                    result.AppDiscription = info["AppDiscription"].ToString();
                    result.NameExe = info["Name_EXE"].ToString();
                }
                return result;
            }
            catch
            {
                throw new Exception();
            }
        }
        /// <summary>Сохранение настроек приложения</summary><param name="versionSetting"></param><returns></returns>
        public bool SaveSetting(AppVersionSetting versionSetting)
        {
            try
            {
                mydb = new DB.SQLite.SQLite(BDPath);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "Update ApplicationInfo set AppDiscription = @AppDiscription, Flag=@Flag, Name_EXE = @EXE WHERE Application = @Application AND AppVersion = @AppVersion";
                command.Parameters.Add("@AppDiscription", DbType.String).Value = versionSetting.AppDiscription;
                command.Parameters.Add("@Flag", DbType.String).Value = versionSetting.ActialFlag;
                command.Parameters.Add("@Application", DbType.String).Value = versionSetting.ActialAppName;
                command.Parameters.Add("@AppVersion", DbType.String).Value = versionSetting.ActialAppVersion;
                command.Parameters.Add("@EXE", DbType.String).Value = versionSetting.NameExe;
                mydb.Insert(command);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
    }
}
