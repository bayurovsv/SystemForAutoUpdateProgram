using AppVersionControl.Domain.ApplicationVersion;
using DB.SQLite;
using System;
using System.Data;
using System.Data.SQLite;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;

namespace ServerDBSQL
{
    class ServerControlService
    {
        /// <summary>Ip сервера</summary>
        private string IPAddres { get; }
        /// <summary>Порт</summary>
        private int Port { get; }

        private static SQLite mydb = null;
        #region Tables
        public struct ApplicationInfo
        {
            public string Application { set; get; }
            public string AppVersion { set; get; }
            public string AppDiscription { set; get; }
            public string Flag { set; get; }
            public string Name_EXE { set; get; }
        }
        public struct Applications
        {
            public string ApplicationName { set; get; }
        }
        public struct FilesContains
        {
            public string Applications { set; get; }
            public string AppVersions { set; get; }
            public string FilePath { set; get; }
            public string DateCreateDB { set; get; }
            public string Contains { set; get; }
        }
        public struct FilesVersions
        {
            public string AppVersions { set; get; }
            public string FileHash { set; get; }
            public string CreationDate { set; get; }
            public int FileSize { set; get; }
            public string FilePath { set; get; }
            public string DateCreateDB { set; get; }
            public string Applications { set; get; }
        }
        #endregion
        public ServerControlService(string ip, int port)
        {
            IPAddres = ip ?? throw new Exception(nameof(ip));
            if (port < 0) throw new Exception(nameof(port));
            Port = port;
        }
        public void Active()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IPAddres), Port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(ipPoint);
            listenSocket.Listen(10);
            Console.WriteLine("Сервер запущен");
            while (true)
            {
                Socket handler = listenSocket.Accept();
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                byte[] data = new byte[256];
                do
                {
                    bytes = handler.Receive(data);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (handler.Available > 0);
                string sql = builder.ToString();
                JavaScriptSerializer serializer1 = new JavaScriptSerializer();
                serializer1.MaxJsonLength = int.MaxValue;
                CommandSQL commandSQL = serializer1.Deserialize<CommandSQL>(sql);
                string[] parsingSQL = commandSQL.CommandText.Split(' ');
                string command = "";
                string table = "";
                if (parsingSQL[0] == "SELECT")
                {
                    command = parsingSQL[0];
                    table = parsingSQL[3];
                }
                else
                {
                    if (parsingSQL[0] == "INSERT")
                    {
                        command = parsingSQL[0];
                        table = parsingSQL[2];
                    }
                    else
                    {
                        command = parsingSQL[0];
                        table = parsingSQL[1];
                    }
                }
                switch (command)
                {
                    case "SELECT":
                        switch (table)
                        {
                            #region Applications
                            case "Applications":
                                string resultApplications = "";
                                Applications applications = new Applications();
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandApplications = new SQLiteCommand();
                                commandApplications.CommandText = commandSQL.CommandText;
                                string[] paramApplications = commandSQL.Parameters.Split('$');
                                for (int i = 1; i < paramApplications.Length; i += 2)
                                {
                                    commandApplications.Parameters.AddWithValue(paramApplications[i], paramApplications[i + 1]);
                                }
                                DataRow[] datarowsApp = mydb.Select(commandApplications);
                                foreach (DataRow row in datarowsApp)
                                {
                                    applications.ApplicationName = row[0].ToString();
                                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                                    serializer.MaxJsonLength = int.MaxValue;
                                    string json = serializer.Serialize(applications);
                                    resultApplications += "$" + json;
                                }
                                data = Encoding.Unicode.GetBytes(resultApplications);
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                            #endregion
                            #region ApplicationInfo
                            case "ApplicationInfo":
                                string resultApplicationInfo = "";
                                ApplicationInfo applicationInfo = new ApplicationInfo();
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandApplicationInfo = new SQLiteCommand();
                                commandApplicationInfo.CommandText = commandSQL.CommandText;
                                string[] paramApplicationInfo = commandSQL.Parameters.Split('$');
                                for (int i = 1; i < paramApplicationInfo.Length; i += 2)
                                {
                                    commandApplicationInfo.Parameters.AddWithValue(paramApplicationInfo[i], paramApplicationInfo[i + 1]);
                                }
                                DataRow[] datarowsApplicationInfo = mydb.Select(commandApplicationInfo);
                                foreach (DataRow row in datarowsApplicationInfo)
                                {
                                    applicationInfo.Application = row[0].ToString();
                                    applicationInfo.AppVersion = row[1].ToString();
                                    applicationInfo.AppDiscription = row[2].ToString();
                                    applicationInfo.Flag = row[3].ToString();
                                    applicationInfo.Name_EXE = row[4].ToString();
                                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                                    serializer.MaxJsonLength = int.MaxValue;
                                    string json = serializer.Serialize(applicationInfo);
                                    resultApplicationInfo += "$" + json;
                                }
                                data = Encoding.Unicode.GetBytes(resultApplicationInfo);
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                            #endregion
                            #region FilesContains
                            case "FilesContains":
                                string resultFilesContains = "";
                                FilesContains filesContains = new FilesContains();
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandFilesContains = new SQLiteCommand();
                                commandFilesContains.CommandText = commandSQL.CommandText;
                                string[] paramFilesContains = null;
                                if (commandSQL.Parameters != null)
                                {
                                    paramFilesContains = commandSQL.Parameters.Split('$');
                                    for (int i = 1; i < paramFilesContains.Length; i += 2)
                                    {
                                        commandFilesContains.Parameters.AddWithValue(paramFilesContains[i], paramFilesContains[i + 1]);
                                    }
                                }
                                DataRow[] datarowsFilesContains = mydb.Select(commandFilesContains);
                                foreach (DataRow row in datarowsFilesContains)
                                {
                                    filesContains.Applications = row[0].ToString();
                                    filesContains.AppVersions = row[1].ToString();
                                    filesContains.FilePath = row[2].ToString();
                                    filesContains.DateCreateDB = row[3].ToString();
                                    filesContains.Contains = row[4].ToString();
                                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                                    serializer.MaxJsonLength = int.MaxValue;
                                    string json = serializer.Serialize(filesContains);
                                    resultFilesContains += "$" + json;
                                }
                                data = Encoding.Unicode.GetBytes(resultFilesContains);
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                            #endregion
                            #region FilesVersions
                            case "FilesVersions":
                                string resultFilesVersions = "";
                                FilesVersions filesVersions = new FilesVersions();
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandFilesVersions = new SQLiteCommand();
                                commandFilesVersions.CommandText = commandSQL.CommandText;
                                if (commandSQL.Parameters != null)
                                {
                                    string[] paramFilesVersions = commandSQL.Parameters.Split('$');
                                    for (int i = 1; i < paramFilesVersions.Length; i += 2)
                                    {
                                        commandFilesVersions.Parameters.AddWithValue(paramFilesVersions[i], paramFilesVersions[i + 1]);
                                    }
                                }
                                DataRow[] datarowsFilesVersions = mydb.Select(commandFilesVersions);
                                foreach (DataRow row in datarowsFilesVersions)
                                {
                                    filesVersions.AppVersions = row[0].ToString();
                                    filesVersions.FileHash = row[1].ToString();
                                    filesVersions.CreationDate = row[2].ToString();
                                    filesVersions.FileSize = Convert.ToInt32(row[3].ToString());
                                    filesVersions.FilePath = row[4].ToString();
                                    filesVersions.DateCreateDB = row[5].ToString();
                                    filesVersions.Applications = row[6].ToString();
                                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                                    serializer.MaxJsonLength = int.MaxValue;
                                    string json = serializer.Serialize(filesVersions);
                                    resultFilesVersions += "$" + json;
                                }
                                data = Encoding.Unicode.GetBytes(resultFilesVersions);
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                                #endregion
                        }
                        break;
                    case "INSERT":
                        switch (table)
                        {
                            #region Applications
                            case "Applications":
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandApplications = new SQLiteCommand();
                                commandApplications.CommandText = commandSQL.CommandText;
                                string[] paramApplications = commandSQL.Parameters.Split('$');
                                for (int i = 1; i < paramApplications.Length; i += 2)
                                {
                                    commandApplications.Parameters.AddWithValue(paramApplications[i], paramApplications[i + 1]);
                                }
                                mydb.Insert(commandApplications);
                                data = Encoding.Unicode.GetBytes("True");
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                            #endregion
                            #region ApplicationInfo
                            case "ApplicationInfo":
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandApplicationInfo = new SQLiteCommand();
                                commandApplicationInfo.CommandText = commandSQL.CommandText;
                                string[] paramApplicationInfo = commandSQL.Parameters.Split('$');
                                for (int i = 1; i < paramApplicationInfo.Length; i += 2)
                                {
                                    commandApplicationInfo.Parameters.AddWithValue(paramApplicationInfo[i], paramApplicationInfo[i + 1]);
                                }
                                mydb.Insert(commandApplicationInfo);
                                data = Encoding.Unicode.GetBytes("True");
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                            #endregion
                            #region FilesContains
                            case "FilesContains":
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandFilesContains = new SQLiteCommand();
                                commandFilesContains.CommandText = commandSQL.CommandText;
                                string[] paramFilesContains = commandSQL.Parameters.Split('$');
                                for (int i = 1; i < paramFilesContains.Length; i += 2)
                                {
                                    commandFilesContains.Parameters.AddWithValue(paramFilesContains[i], paramFilesContains[i + 1]);
                                }
                                mydb.Insert(commandFilesContains);
                                data = Encoding.Unicode.GetBytes("True");
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                            #endregion
                            #region FilesVersions
                            case "FilesVersions":
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandFilesVersions = new SQLiteCommand();
                                commandFilesVersions.CommandText = commandSQL.CommandText;
                                string[] paramFilesVersions = commandSQL.Parameters.Split('$');
                                for (int i = 1; i < paramFilesVersions.Length; i += 2)
                                {
                                    commandFilesVersions.Parameters.AddWithValue(paramFilesVersions[i], paramFilesVersions[i + 1]);
                                }
                                mydb.Insert(commandFilesVersions);
                                data = Encoding.Unicode.GetBytes("True");
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                                #endregion
                        }
                        break;
                    case "Update":
                        switch (table)
                        {
                            #region Applications
                            case "Applications":
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandApplications = new SQLiteCommand();
                                commandApplications.CommandText = commandSQL.CommandText;
                                string[] paramApplications = commandSQL.Parameters.Split('$');
                                for (int i = 1; i < paramApplications.Length; i += 2)
                                {
                                    commandApplications.Parameters.AddWithValue(paramApplications[i], paramApplications[i + 1]);
                                }
                                mydb.Insert(commandApplications);
                                data = Encoding.Unicode.GetBytes("True");
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                            #endregion
                            #region ApplicationInfo
                            case "ApplicationInfo":
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandApplicationInfo = new SQLiteCommand();
                                commandApplicationInfo.CommandText = commandSQL.CommandText;
                                string[] paramApplicationInfo = commandSQL.Parameters.Split('$');
                                for (int i = 1; i < paramApplicationInfo.Length; i += 2)
                                {
                                    commandApplicationInfo.Parameters.AddWithValue(paramApplicationInfo[i], paramApplicationInfo[i + 1]);
                                }
                                mydb.Insert(commandApplicationInfo);
                                data = Encoding.Unicode.GetBytes("True");
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                            #endregion
                            #region FilesContains
                            case "FilesContains":
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandFilesContains = new SQLiteCommand();
                                commandFilesContains.CommandText = commandSQL.CommandText;
                                string[] paramFilesContains = commandSQL.Parameters.Split('$');
                                for (int i = 1; i < paramFilesContains.Length; i += 2)
                                {
                                    commandFilesContains.Parameters.AddWithValue(paramFilesContains[i], paramFilesContains[i + 1]);
                                }
                                mydb.Insert(commandFilesContains);
                                data = Encoding.Unicode.GetBytes("True");
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                            #endregion
                            #region FilesVersions
                            case "FilesVersions":
                                mydb = new SQLite(AppDomain.CurrentDomain.BaseDirectory);
                                SQLiteCommand commandFilesVersions = new SQLiteCommand();
                                commandFilesVersions.CommandText = commandSQL.CommandText;
                                string[] paramFilesVersions = commandSQL.Parameters.Split('$');
                                for (int i = 1; i < paramFilesVersions.Length; i += 2)
                                {
                                    commandFilesVersions.Parameters.AddWithValue(paramFilesVersions[i], paramFilesVersions[i + 1]);
                                }
                                mydb.Insert(commandFilesVersions);
                                data = Encoding.Unicode.GetBytes("True");
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                                #endregion
                        }
                        break;
                }
            }
        }
    }
}

