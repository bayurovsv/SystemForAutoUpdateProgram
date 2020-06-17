using AppVersionControl.Domain.ApplicationVersion;
using AppVersionControl.Domain.Interfaces;
using AppVersionControl.Infrastructure;
using AppVersionControl.Infrastructure.FileSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NUnitTestProject
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }
        //#region Test DB
        //#region FileInDBRepo
        //[Test]
        //public void FindByteFileOfVersionsInDB()
        //{
        //    IFileRepo fileRepo = new FileInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IList<byte[]> findByteFileOfVersions = fileRepo.FindByteFileOfVersions(testApp);
        //    Assert.AreEqual(26,findByteFileOfVersions.Count);
        //}
        //[Test]
        //public void FindFileOfVersionsInDB()
        //{
        //    IFileRepo fileRepo = new FileInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IList<FileOfVersion> findByteFileOfVersions = fileRepo.FindFileOfVersions(testApp);
        //    Assert.AreEqual(26, findByteFileOfVersions.Count);
        //}
        //[Test]
        //public void FindFileOfVersionsWithDataInDB()
        //{
        //    IFileRepo fileRepo = new FileInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IDictionary<FileOfVersion, byte[]> findByteFileOfVersions = fileRepo.FindFileOfVersionsWithData(testApp);
        //    Assert.AreEqual(26, findByteFileOfVersions.Count);
        //}
        //[Test]
        //public void SaveInDB()
        //{
        //    IFileRepo fileRepo = new FileInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IDictionary<FileOfVersion, byte[]> findByteFileOfVersions = fileRepo.FindFileOfVersionsWithData(testApp);
        //    bool flagSave = fileRepo.Save(testApp, findByteFileOfVersions);
        //    Assert.AreEqual("True", flagSave.ToString());
        //}
        //#endregion
        //#region VersionInDBRepo
        //[Test]
        //public void FindLastInDB_NameApp()
        //{
        //    IVersionRepo fileRepo = new VersionInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.FindLast("TestApp");
        //    Assert.AreEqual(new AppVersion("TestApp",new VersionNumber(1,0)), version);
        //}
        //[Test]
        //public void FindLastInDB_AppVersion()
        //{
        //    IVersionRepo fileRepo = new VersionInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.Find(new AppVersion("TestApp"));
        //    Assert.AreEqual(new AppVersion("TestApp",new VersionNumber(1, 0)), version);
        //}
        //[Test]
        //public void FindLastInDB_NameApp_AppVersion()
        //{
        //    IVersionRepo fileRepo = new VersionInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.Find("TestApp", new VersionNumber(1,0));
        //    Assert.AreEqual(new AppVersion("TestApp", new VersionNumber(1, 0)), version);
        //}
        //[Test]
        //public void FindAll_NameApp()
        //{
        //    IVersionRepo fileRepo = new VersionInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    IList <AppVersion> version = fileRepo.FindAll("TestApp");
        //    Assert.AreEqual(1, version.Count);
        //}
        //[Test]
        //public void FindAll()
        //{
        //    IVersionRepo fileRepo = new VersionInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    IList<AppVersion> version = fileRepo.FindAll();
        //    Assert.AreEqual(1, version.Count);
        //}
        //#endregion
        //#region VersionSaveInDBRepo
        //[Test]
        //public void SaveApplicationInDB()
        //{
        //    IVersionSaveRepo saveRepo = new VersionSaveInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    IVersionRepo fileRepo = new VersionInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.Find(new AppVersion("TestApp"));
        //    bool flagSave = saveRepo.SaveApplication(version);
        //    Assert.AreEqual("True", flagSave.ToString());
        //}
        //#endregion
        //#endregion
        //#region Test FileSystem
        //#region FileInFileSystem
        //[Test]
        //public void FindByteFileOfVersionsInFileSystem()
        //{
        //    IFileRepo fileRepo = new FileInSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IList<byte[]> findByteFileOfVersions = fileRepo.FindByteFileOfVersions(testApp);
        //    Assert.AreEqual(26, findByteFileOfVersions.Count);
        //}
        //[Test]
        //public void FindFileOfVersionsInFileSystem()
        //{
        //    IFileRepo fileRepo = new FileInSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IList<FileOfVersion> findByteFileOfVersions = fileRepo.FindFileOfVersions(testApp);
        //    Assert.AreEqual(26, findByteFileOfVersions.Count);
        //}
        //[Test]
        //public void FindFileOfVersionsWithDataInFileSystem()
        //{
        //    IFileRepo fileRepo = new FileInSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IDictionary<FileOfVersion, byte[]> findByteFileOfVersions = fileRepo.FindFileOfVersionsWithData(testApp);
        //    Assert.AreEqual(26, findByteFileOfVersions.Count);
        //}
        //[Test]
        //public void SaveInFileSystem()
        //{
        //    IFileRepo fileRepo = new FileInSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IDictionary<FileOfVersion, byte[]> findByteFileOfVersions = fileRepo.FindFileOfVersionsWithData(testApp);
        //    bool flagSave = fileRepo.Save(testApp, findByteFileOfVersions);
        //    Assert.AreEqual("True", flagSave.ToString());
        //}
        //#endregion
        //#region VersionInFileSystemRepo
        //[Test]
        //public void FindLastInFileSystem_NameApp()
        //{
        //    IVersionRepo fileRepo = new VersionInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.FindLast("TestApp");
        //    Assert.AreEqual(new AppVersion("TestApp", new VersionNumber(1, 0)), version);
        //}
        //[Test]
        //public void FindLastInFileSystem_AppVersion()
        //{
        //    IVersionRepo fileRepo = new VersionInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.Find(new AppVersion("TestApp"));
        //    Assert.AreEqual(new AppVersion("TestApp", new VersionNumber(1, 0)), version);
        //}
        //[Test]
        //public void FindLastInFileSystem_NameApp_AppVersion()
        //{
        //    IVersionRepo fileRepo = new VersionInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.Find("TestApp", new VersionNumber(1, 0));
        //    Assert.AreEqual(new AppVersion("TestApp", new VersionNumber(1, 0)), version);
        //}
        //[Test]
        //public void FindAllInFileSystem_NameApp()
        //{
        //    IVersionRepo fileRepo = new VersionInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    IList<AppVersion> version = fileRepo.FindAll("TestApp");
        //    Assert.AreEqual(1, version.Count);
        //}
        //[Test]
        //public void FindAllInFileSystem()
        //{
        //    IVersionRepo fileRepo = new VersionInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    IList<AppVersion> version = fileRepo.FindAll();
        //    Assert.AreEqual(1, version.Count);
        //}
        //#endregion
        //#region VersionSaveInFileSystemRepo
        //[Test]
        //public void SaveApplicationInFileSystem()
        //{
        //    IVersionSaveRepo saveRepo = new VersionSaveInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    IVersionRepo fileRepo = new VersionInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.Find(new AppVersion("TestApp"));
        //    bool flagSave = saveRepo.SaveApplication(version);
        //    Assert.AreEqual("True", flagSave.ToString());
        //}
        //#endregion
        //#endregion
        //#region Test FileSystemOne
        //#region FileInFileSystemOne
        //[Test]
        //public void FindByteFileOfVersionsInFileSystemOne()
        //{
        //    IFileRepo fileRepo = new FileInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IList<byte[]> findByteFileOfVersions = fileRepo.FindByteFileOfVersions(testApp);
        //    Assert.AreEqual(26, findByteFileOfVersions.Count);
        //}
        //[Test]
        //public void FindFileOfVersionsInFileSystemOne()
        //{
        //    IFileRepo fileRepo = new FileInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IList<FileOfVersion> findByteFileOfVersions = fileRepo.FindFileOfVersions(testApp);
        //    Assert.AreEqual(26, findByteFileOfVersions.Count);
        //}
        //[Test]
        //public void FindFileOfVersionsWithDataInFileSystemOne()
        //{
        //    IFileRepo fileRepo = new FileInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IDictionary<FileOfVersion, byte[]> findByteFileOfVersions = fileRepo.FindFileOfVersionsWithData(testApp);
        //    Assert.AreEqual(26, findByteFileOfVersions.Count);
        //}
        //[Test]
        //public void SaveInFileSystemOne()
        //{
        //    IFileRepo fileRepo = new FileInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion testApp = new AppVersion("TestApp");
        //    IDictionary<FileOfVersion, byte[]> findByteFileOfVersions = fileRepo.FindFileOfVersionsWithData(testApp);
        //    bool flagSave = fileRepo.Save(testApp, findByteFileOfVersions);
        //    Assert.AreEqual("True", flagSave.ToString());
        //}
        //#endregion
        //#region VersionInFileSystemRepoOne
        //[Test]
        //public void FindLastInFileSystem_NameAppOne()
        //{
        //    IVersionRepo fileRepo = new VersionInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.FindLast("TestApp");
        //    Assert.AreEqual(new AppVersion("TestApp", new VersionNumber(1, 0)), version);
        //}
        //[Test]
        //public void FindLastInFileSystem_AppVersionOne()
        //{
        //    IVersionRepo fileRepo = new VersionInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.Find(new AppVersion("TestApp"));
        //    Assert.AreEqual(new AppVersion("TestApp", new VersionNumber(1, 0)), version);
        //}
        //[Test]
        //public void FindLastInFileSystem_NameApp_AppVersionOne()
        //{
        //    IVersionRepo fileRepo = new VersionInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.Find("TestApp", new VersionNumber(1, 0));
        //    Assert.AreEqual(new AppVersion("TestApp", new VersionNumber(1, 0)), version);
        //}
        //[Test]
        //public void FindAllInFileSystem_NameAppOne()
        //{
        //    IVersionRepo fileRepo = new VersionInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    IList<AppVersion> version = fileRepo.FindAll("TestApp");
        //    Assert.AreEqual(1, version.Count);
        //}
        //[Test]
        //public void FindAllInFileSystemOne()
        //{
        //    IVersionRepo fileRepo = new VersionInFileSystemRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    IList<AppVersion> version = fileRepo.FindAll();
        //    Assert.AreEqual(1, version.Count);
        //}
        //#endregion
        //#region VersionSaveInFileSystemRepoOne
        //[Test]
        //public void SaveApplicationInFileSystemOne()
        //{
        //    IVersionSaveRepo saveRepo = new VersionSaveInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    IVersionRepo fileRepo = new VersionInDBRepo(AppDomain.CurrentDomain.BaseDirectory);
        //    AppVersion version = fileRepo.Find(new AppVersion("TestApp"));
        //    bool flagSave = saveRepo.SaveApplication(version);
        //    Assert.AreEqual("True", flagSave.ToString());
        //}
        //#endregion
        //#endregion
    }
}