using UnityEngine;
using UnityEngine.UI;
using SFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectScript;

namespace ProjectScript.Network
{
    public class HandleBasicMsg
    {
        public static void MsgDisconnect(Protocol protocol)
        {
            Debug.Log("[客户端] 断开连接");
            NetMgr.srvConn.Close();
        }

        public static void MsgRegister(Protocol protocol)
        {
            if (protocol.bodyStr == "0")
            {
                Debug.Log("[客户端] 注册成功");
            }
            else
            {
                Debug.Log("[客户端] 注册失败");
            }
        }

        public static void MsgLogin(Protocol protocol)
        {
            if (protocol.bodyStr == "0")
            {
                Debug.Log("[客户端] 登录成功");
            }
            else
            {
                Debug.Log("[客户端] 登录失败");
            }
        }
    }
}
