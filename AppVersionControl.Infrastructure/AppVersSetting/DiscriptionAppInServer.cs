using AppVersionControl.Domain.ApplicationVersion.ApplicationVersionSettings;
using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using ServerSQLite;
using System;
using System.Data;
using System.Data.SQLite;

namespace AppVersionControl.Infrastructure.AppVersSetting
{
    /// <summary>Получении информациио об актуальной версии приложения</summary>
    public class DiscriptionAppInServer : IDiscriptionApp
    {
        private static int Port;
        private static string Address;
        private ClientControlService clientControl;
        public DiscriptionAppInServer(string address, int port)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            if (port < 0) throw new Exception(nameof(port));
            Port = port;
        }
        /// <summary>Получение версии приложения</summary><param name="appName"></param><returns></returns>
        public AppVersion GetAppVersion(string appName)
        {
            AppVersion AppVersions = null;
            clientControl = new ClientControlService(Address, Port);
            string flag = "True";
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ApplicationInfo WHERE Flag = @Flag AND Application = @Application";
            command.Parameters.Add("@Flag", DbType.String).Value = flag;
            command.Parameters.Add("@Application", DbType.String).Value = appName;
            DataRow[] datarows = clientControl.Select(command);
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
            clientControl = new ClientControlService(Address, Port);
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ApplicationInfo WHERE Flag = @Flag AND Application = @Application AND AppVersion = @AppVersion";
            string flag = "True";
            command.Parameters.Add("@Flag", DbType.String).Value = flag;
            command.Parameters.Add("@Application", DbType.String).Value = appName;
            command.Parameters.Add("@AppVersion", DbType.String).Value = appVersion;
            DataRow[] datarows = clientControl.Select(command);
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
        /// <summary>Получение описания приложения</summary><param name="appName"></param><param name="appVersion"></param><returns></returns>
        public string GetDiscription(string appName, string appVersion)
        {
            clientControl = new ClientControlService(Address, Port);
            SQLiteCommand command = new SQLiteCommand();
            command.CommandText = "SELECT * FROM ApplicationInfo WHERE Flag = @Flag AND Application = @Application AND AppVersion = @AppVersion";
            string flag = "True";
            command.Parameters.Add("@Flag", DbType.String).Value = flag;
            command.Parameters.Add("@Application", DbType.String).Value = appName;
            command.Parameters.Add("@AppVersion", DbType.String).Value = appVersion;
            DataRow[] datarows = clientControl.Select(command);
            if (datarows == null)
            {
                throw new Exception();
            }
            string discription = "";
            foreach (var info in datarows)
            {
                discription = info["AppDiscription"].ToString();
            }
            return discription;
        }
        /// <summary>Получение настроек сообщения</summary><param name="appName"></param><param name="appVersion"></param><returns></returns>
        public AppVersionSetting GetSetting(string appName, string appVersion)
        {
            try
            {
                clientControl = new ClientControlService(Address, Port);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "SELECT * FROM ApplicationInfo WHERE Application = @Application AND AppVersion = @AppVersion ";
                command.Parameters.Add("@Application", DbType.String).Value = appName;
                command.Parameters.Add("@AppVersion", DbType.String).Value = appVersion;
                DataRow[] datarows = clientControl.Select(command);
                if (datarows == null)
                {
                    throw new Exception();
                }
                AppVersionSetting result = new AppVersionSetting();
                foreach (var info in datarows)
                {
                    result.ActialAppName = info["Application"].ToString();
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
                clientControl = new ClientControlService(Address, Port);
                SQLiteCommand command = new SQLiteCommand();
                command.CommandText = "Update ApplicationInfo set AppDiscription = @AppDiscription, Flag = @Flag, Name_EXE = @EXE WHERE Application = @Application AND AppVersion = @AppVersion";
                command.Parameters.Add("@AppDiscription", DbType.String).Value = versionSetting.AppDiscription;
                command.Parameters.Add("@Flag", DbType.String).Value = versionSetting.ActialFlag;
                command.Parameters.Add("@Application", DbType.String).Value = versionSetting.ActialAppName;
                command.Parameters.Add("@AppVersion", DbType.String).Value = versionSetting.ActialAppVersion;
                command.Parameters.Add("@EXE", DbType.String).Value = versionSetting.NameExe;
                clientControl.Insert(command);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
    }
}
