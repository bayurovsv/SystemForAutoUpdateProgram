namespace AppVersionControlServices
{
    /// <summary>Модель для отображения информации о версии приложения</summary>
    public class DisplayApplication
    {
        /// <summary>Название приложения</summary>
        public string ApplicationName { get; set; }
        /// <summary>Номер сборки</summary>
        public string ApplicationNumber { get; set; }
        /// <summary>Флаг актуальности приложения</summary>
        public string FlagUpdate { get; set; }
        /// <summary>Описание версии приложения</summary>
        public string Discription { get; set; }
    }
}
