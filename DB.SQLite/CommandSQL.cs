namespace DB.SQLite
{
    /// <summary>Вспомагательная модель для формирования команды SQLite </summary>
    public class CommandSQL
    {
        /// <summary>Текст команды(sql запрос)</summary>
        public string CommandText { set; get; }
        /// <summary>Строка содержащая имя параметра и его значение</summary>
        public string Parameters { set; get; }
    }
}
