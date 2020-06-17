using AppVersionControl.Domain.ApplicationVersion.AppSetting;
using AppVersionControl.Domain.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AppVersionControl.Infrastructure.ClientAppSetting
{
    /// <summary>Настройки клиентского приложения</summary>
    public class SettingAppClient : ISettingAppClient
    {
        /// <summary>Получение настроек</summary><param name="path"></param>
        public ClientSetting GetSetting(string path)
        {
            ClientSetting clientSetting=null;
            if (File.Exists("Setting.Config"))
            {
                StreamReader fs = new StreamReader(path);
                List<string> setting = new List<string>();
                while (true)
                {
                    string temp = fs.ReadLine();
                    if (temp == null) break;
                    setting.Add(temp);
                }
                switch (setting[1])
                {
                    case "True":
                        clientSetting = new ClientSetting(setting[0], true);
                        break;
                    case "False":
                        clientSetting = new ClientSetting(setting[0], false);
                        break;
                }
            }
            return clientSetting;
        }
        /// <summary>Сохранение настроек приложения</summary><param name="setting"></param><param name="path"></param>
        public bool SaveSetting(ClientSetting setting ,string path)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default))
                {
                    sw.WriteLine(setting.PathSetting);
                    sw.WriteLine(setting.FlagSettings.ToString());
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
