using DB.SQLite;
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;

namespace ServerSQLite
{
    public class ClientControlService
    {
        /// <summary>Ip сервера</summary>
        private string IPAddres { get; }
        /// <summary>Порт</summary>
        private int Port { get; }
        public ClientControlService(string ip, int port)
        {
            IPAddres = ip ?? throw new Exception(nameof(ip));
            if (port < 0) throw new Exception(nameof(port));
            Port = port;
        }
        public bool Update(SQLiteCommand command)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IPAddres), Port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            CommandSQL commandSend = new CommandSQL();
            commandSend.CommandText = command.CommandText;
            foreach (SQLiteParameter par in command.Parameters)
            {
                commandSend.Parameters += "$" + par.ParameterName + "$" + par.Value;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string json = serializer.Serialize(commandSend);
            byte[] data = Encoding.Unicode.GetBytes(json);
            socket.Send(data);
            data = new byte[256];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);
            string test = builder.ToString();
            if (test == "True")
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return true;
            }
            else
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return false;
            }
        }
        public bool Insert(SQLiteCommand command)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IPAddres), Port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            CommandSQL commandSend = new CommandSQL();
            commandSend.CommandText = command.CommandText;
            foreach (SQLiteParameter par in command.Parameters)
            {
                if (par.Value.GetType() == typeof(byte[]))
                {
                    commandSend.Parameters += "$" + par.ParameterName + "$" + ByteArrayToString((byte[])par.Value);
                }
                else
                {
                    commandSend.Parameters += "$" + par.ParameterName + "$" + par.Value;
                }
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string json = serializer.Serialize(commandSend);
            byte[] data = Encoding.Unicode.GetBytes(json);
            socket.Send(data);
            data = new byte[256];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);
            string test = builder.ToString();
            if (test == "True")
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return true;
            }
            else
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return false;
            }
        }
        public DataRow[] Select(SQLiteCommand command)
        {
            string[] parsingSQL = command.CommandText.Split(' ');
            string table = parsingSQL[3];
            DataRow[] dataRows = null;
            switch (table)
            {
                #region Applications
                case "Applications":
                    dataRows = DtApplications(command);
                    break;
                #endregion

                #region ApplicationInfo
                case "ApplicationInfo":
                    dataRows = DtApplicationInfo(command);
                    break;
                #endregion

                #region FilesContains
                case "FilesContains":
                    dataRows = DtFilesContains(command);
                    break;
                #endregion

                #region FilesVersions
                case "FilesVersions":
                    dataRows = DtFilesVersions(command);
                    break;
                    #endregion
            }
            return dataRows;
        }
        #region DataRows
        private DataRow[] DtApplications(SQLiteCommand command)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IPAddres), Port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            //добавление в модель данных 
            CommandSQL commandSend = new CommandSQL();
            commandSend.CommandText = command.CommandText;
            foreach (SQLiteParameter par in command.Parameters)
            {
                commandSend.Parameters += "$" + par.ParameterName + "$" + par.Value;
            }
            //сериализация для отправки команды
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string json = serializer.Serialize(commandSend);
            //отправка
            byte[] data = Encoding.Unicode.GetBytes(json);
            socket.Send(data);
            // получаем ответ
            data = new byte[256]; // буфер для ответа
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байт
            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);
            string test = builder.ToString();
            string[] mystring = test.Split('$');
            DataTable table = new DataTable();
            table.Columns.Add("ApplicationName");
            for (int i = 1; i < mystring.Length; i++)
            {
                JavaScriptSerializer serializer1 = new JavaScriptSerializer();
                serializer1.MaxJsonLength = int.MaxValue;
                Applications meta = serializer1.Deserialize<Applications>(mystring[i]);
                table.Rows.Add(new object[] { meta.ApplicationName });
            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            DataRow[] dataRows = table.Rows.Cast<DataRow>().ToArray();
            return dataRows;
        }
        private DataRow[] DtApplicationInfo(SQLiteCommand command)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IPAddres), Port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            //добавление в модель данных 
            CommandSQL commandSend = new CommandSQL();
            commandSend.CommandText = command.CommandText;
            foreach (SQLiteParameter par in command.Parameters)
            {
                commandSend.Parameters += "$" + par.ParameterName + "$" + par.Value;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string json = serializer.Serialize(commandSend);
            byte[] data = Encoding.Unicode.GetBytes(json);
            socket.Send(data);
            data = new byte[256];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);
            string test = builder.ToString();
            string[] mystring = test.Split('$');
            DataTable table = new DataTable();
            table.Columns.Add("Application");
            table.Columns.Add("AppVersion");
            table.Columns.Add("AppDiscription");
            table.Columns.Add("Flag");
            table.Columns.Add("Name_EXE");
            for (int i = 1; i < mystring.Length; i++)
            {
                JavaScriptSerializer serializer1 = new JavaScriptSerializer();
                serializer1.MaxJsonLength = int.MaxValue;
                ApplicationInfo meta = serializer1.Deserialize<ApplicationInfo>(mystring[i]);
                table.Rows.Add(new object[] { meta.Application, meta.AppVersion, meta.AppDiscription, meta.Flag, meta.Name_EXE});
            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            DataRow[] dataRows = table.Rows.Cast<DataRow>().ToArray();
            return dataRows;
        }
        private DataRow[] DtFilesVersions(SQLiteCommand command)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IPAddres), Port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            CommandSQL commandSend = new CommandSQL();
            commandSend.CommandText = command.CommandText;
            foreach (SQLiteParameter par in command.Parameters)
            {
                commandSend.Parameters += "$" + par.ParameterName + "$" + par.Value;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string json = serializer.Serialize(commandSend);
            byte[] data = Encoding.Unicode.GetBytes(json);
            socket.Send(data);
            data = new byte[256];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);
            string test = builder.ToString();
            string[] mystring = test.Split('$');
            DataTable table = new DataTable();
            table.Columns.Add("AppVersion");
            table.Columns.Add("FileHash");
            table.Columns.Add("CreationDate");
            table.Columns.Add("FileSize");
            table.Columns.Add("FilePath");
            table.Columns.Add("DateCreateDB");
            table.Columns.Add("Applications");
            for (int i = 1; i < mystring.Length; i++)
            {
                JavaScriptSerializer serializer1 = new JavaScriptSerializer();
                serializer1.MaxJsonLength = int.MaxValue;
                FilesVersions meta = serializer1.Deserialize<FilesVersions>(mystring[i]);
                table.Rows.Add(new object[] { meta.AppVersions, meta.FileHash, meta.CreationDate, meta.FileSize, meta.FilePath, meta.DateCreateDB, meta.Applications });
            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            DataRow[] dataRows = table.Rows.Cast<DataRow>().ToArray();
            return dataRows;
        }
        private DataRow[] DtFilesContains(SQLiteCommand command)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IPAddres), Port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);
            //добавление в модель данных 
            CommandSQL commandSend = new CommandSQL();
            commandSend.CommandText = command.CommandText;
            foreach (SQLiteParameter par in command.Parameters)
            {
                commandSend.Parameters += "$" + par.ParameterName + "$" + par.Value;
            }
            //сериализация для отправки команды
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string json = serializer.Serialize(commandSend);
            //отправка
            byte[] data = Encoding.Unicode.GetBytes(json);
            socket.Send(data);
            // получаем ответ
            data = new byte[256]; // буфер для ответа
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байт
            do
            {
                bytes = socket.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);
            string test = builder.ToString();
            string[] mystring = test.Split('$');
            DataTable table = new DataTable();
            table.Columns.Add("Applications");
            table.Columns.Add("AppVersions");
            table.Columns.Add("FilePath");
            table.Columns.Add("DateCreateDB");
            table.Columns.Add("Contains");
            table.Columns[4].DataType = typeof(byte[]);
            for (int i = 1; i < mystring.Length; i++)
            {
                JavaScriptSerializer serializer1 = new JavaScriptSerializer();
                serializer1.MaxJsonLength = int.MaxValue;
                FilesContains meta = serializer1.Deserialize<FilesContains>(mystring[i]);
                table.Rows.Add(new object[] { meta.Applications, meta.AppVersions, meta.FilePath, meta.DateCreateDB, StringToByteArray(meta.Contains) });
            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            DataRow[] dataRows = table.Rows.Cast<DataRow>().ToArray();
            return dataRows;
        }
        #endregion
        #region Tools
        private string ByteArrayToString(byte[] cont)
        {
            StringBuilder contains = new StringBuilder(cont.Length * 2);
            foreach (byte b in cont)
                contains.AppendFormat("{0:x2}", b);
            return contains.ToString();
        }
        private byte[] StringToByteArray(string contains)
        {
            int Num = contains.Length;
            byte[] bytes = new byte[Num / 2];
            for (int i = 0; i < Num; i += 2)
                bytes[i / 2] = Convert.ToByte(contains.Substring(i, 2), 16);
            return bytes;
        }
        #endregion
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
    }
}
