using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ProjectScript.Network
{
    /// <summary>
    /// 网络管理。维护Connection连接，封装Connection接口，提供辅助函数
    /// </summary>
    public class NetMgr
    {
        public static Connection srvConn = new Connection();
        //public static Connection platformConn = new Connection();

        public static void Update()
        {
            srvConn.Update();
            //platformConn.Update();
        }

        // 判定安全字符串（不得包含不安全符号）
        public static bool IsSafeStr(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }
    }
}