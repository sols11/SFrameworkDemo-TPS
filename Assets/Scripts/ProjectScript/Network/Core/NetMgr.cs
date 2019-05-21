using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProjectScript.Network
{
    /// 网络管理
    public class NetMgr
    {
        public static Connection srvConn = new Connection();
        //public static Connection platformConn = new Connection();

        private static string host = "127.0.0.1";
        private static int    port = 8888;

        public static void Update()
        {
            srvConn.Update();
            //platformConn.Update();
        }

        public static void ConnectServ()
        {
            srvConn.Connect(host, port);
        }

    }
}