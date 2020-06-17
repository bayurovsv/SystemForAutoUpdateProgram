using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using AppVersionControl.Infrastructure.Bilders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppVersionControlServices
{
    /// <summary>Класс работы приложения</summary>
    public class AppVersionControlService
    {
        /// <summary>Реализация интерфейса(откуда брать метаданные)</summary>
        private IVersionRepo Src_meta { get; }
        /// <summary>Куда записывать метаданные</summary>
        private IVersionSaveRepo Dst_meta { get; }
        /// <summary>Откуда брать файлы</summary>
        private IFileRepo Src_content { get; }
        /// <summary>Куда сохранять</summary>
        private IFileRepo Dst_content { get; }
        /// <summary>Информация о версии приожения</summary>
        private IDiscriptionApp Discription { get; }
        public AppVersionControlService(IVersionRepo src_meta, IVersionSaveRepo dst_meta, IFileRepo src_content, IFileRepo dst_content, IDiscriptionApp discription)
        {
            Src_meta = src_meta ?? throw new Exception(nameof(src_meta));
            Dst_meta = dst_meta ?? throw new Exception(nameof(dst_meta));
            Src_content = src_content ?? throw new Exception(nameof(src_content));
            Dst_content = dst_content ?? throw new Exception(nameof(dst_content));
            Discription = discription ?? throw new Exception(nameof(discription));
        }
        ///<summary>Обновление приложения, а также добавление у клиента</summary>
        public ServiceResult UpdateApp(string app_name)
        {
            AppVersion ActialVers = Discription.GetAppVersion(app_name);
            IDictionary<FileOfVersion, byte[]> fileContents = Src_content.FindFileOfVersionsWithData(ActialVers);
            ServiceResult service = new ServiceResult(Dst_content.Save(ActialVers, fileContents));
            Dst_meta.SaveApplication(ActialVers);
            return service;
        }
        ///<summary>Добавление приложения нового приложения</summary>
        public ServiceResult AddAplicationNew(string rootPath, string appName, VersionNumber appVerNumber)
        {
            IApplicationVersionBuilder application = new AppBuilder(rootPath, appName, appVerNumber);
            AppVersion srcV = application.Build();
            IDictionary<FileOfVersion, byte[]> fileContents = application.FindFileOfVersionsWithData();
            ServiceResult service = new ServiceResult(Dst_content.Save(srcV, fileContents));
            Dst_meta.SaveApplication(srcV);
            return service;
        }
        ///<summary>Добавление приложения</summary>
        public ServiceResult AddAplication(string appName, IApplicationVersionBuilder application)
        {
            AppVersion srcV = application.Build();
            AppVersion Version = Src_meta.FindLast(appName);
            AppVersion newVersion = new AppVersion(srcV.VersionName,
                new VersionNumber(srcV.VersionNumber.Number,
                Version.VersionNumber.VersionBuildNumber + 1), srcV.Files);
            if (Version.VersionNumber < newVersion.VersionNumber)
            {
                IDictionary<FileOfVersion, byte[]> fileContents = application.FindFileOfVersionsWithData();
                ServiceResult service = new ServiceResult(Dst_content.Save(newVersion, fileContents));
                Dst_meta.SaveApplication(newVersion);
                return service;
            }
            else
            {
                ServiceResult service = new ServiceResult(false);
                return service;
            }
        }
        ///<summary>Список последних версий</summary>
        public List<AppVersion> GetVersionFileSystem()
        {
            IList<AppVersion> versions = Src_meta.FindAll();
            List<AppVersion> versionsSort = new List<AppVersion>();
            string[] VersionsApps = (from v in versions select v.VersionName.ToString()).Distinct().ToArray();
            versionsSort.AddRange(from string v1 in VersionsApps select Src_meta.FindLast(v1));
            return versionsSort;
        }
        /// <summary> Формирование списка отображаемых данных оприложении</summary><param name="versionsSort"></param>
        public List<DisplayApplication> GetApplications(List<AppVersion> versionsSort)
        {
            List<DisplayApplication> Applications = new List<DisplayApplication>();
            foreach (var (version, versionDB) in from AppVersion ver in versionsSort
                                                 let version = Src_meta.Find(ver)
                                                 let versionDB = Discription.GetAppVersion(ver.VersionName)
                                                 select (version, versionDB))
            {
                if (version.VersionNumber == versionDB.VersionNumber)
                {
                    DisplayApplication application = new DisplayApplication();
                    application.ApplicationName = version.VersionName;
                    application.ApplicationNumber = version.VersionNumber.ToString();
                    application.Discription = Discription.GetDiscription(version.VersionName, version.VersionNumber.ToString());
                    application.FlagUpdate = "Установленна актуальная версия";
                    Applications.Add(application);
                }
                else
                {
                    DisplayApplication application = new DisplayApplication();
                    application.ApplicationName = version.VersionName;
                    application.ApplicationNumber = version.VersionNumber.ToString();
                    application.Discription = Discription.GetDiscription(version.VersionName, version.VersionNumber.ToString());
                    application.FlagUpdate = "Доступно обновление";
                    Applications.Add(application);
                }
            }
            return Applications;
        }
        /// <summary> Запуск выбранного приложения</summary><param name="rootPath"></param><param name="test"></param>
        public void StartApp(string rootPath, AppVersion test)
        {
            string testname = rootPath + "\\Versions" + "\\" + test.VersionName + "\\" + Discription.GetNameExe(test.VersionName, test.VersionNumber.ToString());
            System.Diagnostics.Process.Start(testname);
        }
    }
}
