using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDBSQL
{
    class Program
    {
        static int port = 8005;
        static string ip = "127.0.0.1";
        static void Main(string[] args)
        {
            ServerControlService serverControl = new ServerControlService(ip, port);
            serverControl.Active();
        }
    }

}
