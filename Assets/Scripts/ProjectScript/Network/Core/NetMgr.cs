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
            srvConn.proto = new ProtocolBytes();
            srvConn.Connect(host, port);
        }

        // 心跳
        public static ProtocolBase GetHeatBeatProtocol()
        {
            // 具体的发送内容根据服务端设定改动
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("HeatBeat");
            return protocol;
        }
    }
}