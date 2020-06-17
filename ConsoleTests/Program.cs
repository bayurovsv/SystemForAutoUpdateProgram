using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.ApplicationVersion.ApplicationVersionSettings;
using AppVersionControl.Domain.Interfaces;
using AppVersionControl.Infrastructure;
using AppVersionControl.Infrastructure.DBInServer;
using AppVersionControl.Infrastructure.FileSystem;
using AppVersionControl.Infrastructure.FileSystemOneVers;
using AppVersionControlServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleTests
{
    class Program
    {        /// <summary>Путь сохранения приложений </summary>
        private static string Path = AppDomain.CurrentDomain.BaseDirectory;
      
        static void Main(string[] args)
        {

        //поиск и запуск исполняемого файла приложения
            IVersionRepo repo = new VersionInFileSystemRepoOne(AppDomain.CurrentDomain.BaseDirectory); 
            IList<AppVersion> versions = repo.FindAll();
            List<AppVersion> versionsSort = new List<AppVersion>();
            string[] VersionsApps = (from v in versions select v.VersionName.ToString()).Distinct().ToArray();
            versionsSort.AddRange(from string v1 in VersionsApps select repo.FindLast(v1));
            AppVersion test = versions[0];
            IFileRepo fileRepo = new FileInSystemRepoOne(AppDomain.CurrentDomain.BaseDirectory);
            IList<FileOfVersion> testfile = fileRepo.FindFileOfVersions(test);
            string testname = "";
            foreach (var v2 in from FileOfVersion v2 in testfile where v2.FileExtension == ".exe" select v2)
            {
                testname = Path + "\\Versions" + "\\" + test.VersionName + "\\" + v2.FilePath;
                break;
            }
            System.Diagnostics.Process.Start(testname);
            Console.ReadKey();
        }
    }
}
