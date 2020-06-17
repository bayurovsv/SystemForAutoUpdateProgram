namespace AppVersionControlServices
{
    /// <summary>Модель отображения работы сервиса</summary>
    public class ServiceResult
    {
        /// <summary>Флаг выполнения</summary>
        public bool IsSuccess { get; set; }
        public ServiceResult(bool flag)
        {
           IsSuccess = flag;
        }
    }
}
