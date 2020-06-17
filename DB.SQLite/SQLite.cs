using System;
using System.Data;
using System.Data.SQLite;

namespace DB.SQLite
{
    /// <summary>Класс для работы с БД</summary>
    public class SQLite
    {
        static string cs = "Data Source=:memory:";
        private static Logger logger = new Logger(AppDomain.CurrentDomain.BaseDirectory);
        private readonly string BDPath;
        public SQLite(string bd_path)
        {
            BDPath = bd_path ?? throw new ArgumentNullException(nameof(bd_path));
            BDPath = string.Concat(BDPath, @"\DB\updatefile.db");
        }
        public bool Update(SQLiteCommand command)
        {
            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.ConnectionString = @"Data Source=" + BDPath + ";New=False;Version=3";
                con.Open();
                try
                {
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        sqlCommand.CommandText = command.CommandText;
                        foreach (SQLiteParameter par in command.Parameters)
                        {
                            sqlCommand.Parameters.Add(par);
                        }
                        sqlCommand.ExecuteNonQuery();
                    }
                    con.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Add(ex);
                    throw new Exception();
                }
            }
        }
        public bool Insert(SQLiteCommand command)
        {
            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.ConnectionString = @"Data Source=" + BDPath + ";New=False;Version=3";
                con.Open();
                try
                {
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        sqlCommand.CommandText=command.CommandText;
                        foreach(SQLiteParameter par in command.Parameters)
                        {
                            sqlCommand.Parameters.Add(par);
                        }
                        sqlCommand.ExecuteNonQuery();
                    }
                    con.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Add(ex);
                    throw new Exception();
                }
            }
        }
        public DataRow[] Select(SQLiteCommand command)
        {
            DataRow[] datarows = null;
            SQLiteDataAdapter dataadapter = null;
            DataSet dataset = new DataSet();
            DataTable datatable = new DataTable();
            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.ConnectionString = @"Data Source=" + BDPath +
               ";New=False;Version=3";
                con.Open();
                try
                {
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        sqlCommand.CommandText = command.CommandText;
                        foreach (SQLiteParameter par in command.Parameters)
                        {
                            sqlCommand.Parameters.Add(par);
                        }
                        dataadapter = new SQLiteDataAdapter(sqlCommand);
                        dataset.Reset();
                        dataadapter.Fill(dataset);
                        datatable = dataset.Tables[0];
                        datarows = datatable.Select();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    logger.Add(ex);
                    throw new Exception();
                }
                return datarows;
            }
        }
    }
}

