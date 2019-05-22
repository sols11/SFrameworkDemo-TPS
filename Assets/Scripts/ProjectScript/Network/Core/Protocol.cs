using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEngine;

namespace ProjectScript.Network
{
    /// <summary>
    /// 一个Size+Name+Json传输协议，是目前使用的协议。
    /// Protocol用于维护Name，JsonStr，Bytes部分，不包含Size。
    /// Protocol由Connection创建，将body部分内容交给MsgDistribution处理。
    /// </summary>
    public class Protocol
    {
        // 未经处理的bytes字节流
        public byte[] body;
        // 接受的数据。TODO：以后换成属性
        public string name;
        public string bodyStr;

        public Protocol(byte[] dataBuffer, int startIndex, int size)
        {
            // 拷贝body（分片）
            body = new byte[size];
            Array.Copy(dataBuffer, startIndex, body, 0, size);
            // 再拿到body中的name
            int end = 0;
            name = GetString(body, 0, ref end);
            if (String.IsNullOrEmpty(name))
                return;
            // 余下部分是JsonStr
            bodyStr = System.Text.Encoding.UTF8.GetString(body, end, size - end);
        }

        // 以下是接口方法

        // 将bytes中数据提取出来（用于Recv)
        public static bool Decode(byte[] dataBuffer)
        {
            // 先拿到headpack中的size
            int end = 0;
            int size = GetInt(dataBuffer, 0, ref end);
            if (size <= 0)
                return false;
            // 再拿到body中的name
            string name = GetString(dataBuffer, end, ref end);
            if (String.IsNullOrEmpty(name))
                return false;
            string jsonStr = System.Text.Encoding.UTF8.GetString(dataBuffer, end, size);
            object data = JsonMapper.ToObject(jsonStr);
            return true;
        }

        // 将数据打包（用于Send)，返回打包结果，会修改dataBuffer，失败返回null
        public static byte[] Encode(string name, byte[] dataBuffer)
        {
            if (String.IsNullOrEmpty(name))
                return null;
            // 注意int要做大端转换
            Int32 len = System.Net.IPAddress.HostToNetworkOrder(name.Length);
            byte[] lenBytes = BitConverter.GetBytes(len);
            byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(name);
            if(dataBuffer == null)
                dataBuffer = lenBytes.Concat(strBytes).ToArray();
            else
                dataBuffer = lenBytes.Concat(strBytes).Concat(dataBuffer).ToArray();
            Int32 size = System.Net.IPAddress.HostToNetworkOrder(dataBuffer.Length);
            byte[] sizeBytes = BitConverter.GetBytes(size);
            dataBuffer = sizeBytes.Concat(dataBuffer).ToArray();
            return dataBuffer;
        }

        // 原理就是将int转为Byte类型然后concat，加入bytes数组，数组引用传递
        public static void AddInt(byte[] dataBuffer, int num)
        {
            num = System.Net.IPAddress.HostToNetworkOrder(num);
            byte[] numBytes = BitConverter.GetBytes(num);
            if (dataBuffer == null)
                dataBuffer = numBytes;
            else
                dataBuffer = dataBuffer.Concat(numBytes).ToArray();
        }

        // 安全方式获取，失败返回0
        public static int GetInt(byte[] dataBuffer, int start, ref int end)
        {
            if (dataBuffer == null)
                return 0;
            if (dataBuffer.Length < start + sizeof(Int32))
                return 0;
            end = start + sizeof(Int32);
            int num = BitConverter.ToInt32(dataBuffer, start);
            num = System.Net.IPAddress.NetworkToHostOrder(num);
            return num;
        }

        public static int GetInt(byte[] dataBuffer, int start)
        {
            int end = 0;
            return GetInt(dataBuffer, start, ref end);
        }

        // 添加字符串
        public static void AddString(byte[] dataBuffer, string str)
        {
            Int32 len = str.Length;
            byte[] lenBytes = BitConverter.GetBytes(len);
            byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(str);
            if (dataBuffer == null)
                dataBuffer = lenBytes.Concat(strBytes).ToArray();
            else
                dataBuffer = dataBuffer.Concat(lenBytes).Concat(strBytes).ToArray();
        }

        // 从字节数组的start处开始读取字符串
        public static string GetString(byte[] dataBuffer, int start, ref int end)
        {
            if (dataBuffer == null)
            {
                Debug.LogWarning("[GetString失败] dataBuffer为null");
                return "";
            }
            if (dataBuffer.Length < start + sizeof(Int32))
            {
                Debug.LogWarning("[GetString失败] dataBuffer长度不足获取Lengt");
                return "";
            }
            int strLen = GetInt(dataBuffer, start);
            if (strLen <= 0)
            {
                Debug.LogWarning("[GetString失败] 获取strLen出错：" + strLen);
                return "";
            }
            if (dataBuffer.Length < start + sizeof(Int32) + strLen)
            {
                Debug.LogWarning("[GetString失败] dataBuffer长度不足获取String");
                return "";
            }
            string str = System.Text.Encoding.UTF8.GetString(dataBuffer, start + sizeof(Int32), strLen);
            end = start + sizeof(Int32) + strLen;
            if (string.IsNullOrEmpty(str))
            {
                Debug.LogWarning("[GetString失败] 空字符串");
                return "";
            }
            return str;
        }

        public static string GetString(byte[] dataBuffer, int start)
        {
            int end = 0;
            return GetString(dataBuffer, start, ref end);
        }


    }
}
