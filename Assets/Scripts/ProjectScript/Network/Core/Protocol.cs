using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace ProjectScript.Network
{
    /// <summary>
    /// 一个Size+Name+Json传输协议，是目前使用的协议
    /// </summary>
    public class Protocol
    {
        // 接受的数据
        public int size;
        public string name;
        public string jsonStr;
        // 传输的字节流
        public byte[] bytes;

        // 将bytes中数据提取出来
        public bool Decode(byte[] dataBuffer)
        {
            // 先拿到headpack中的size
            int end = 0;
            size = GetInt(dataBuffer, 0, ref end);
            if (size <= 0)
                return false;
            // 再拿到body中的name
            name = GetString(dataBuffer, end, ref end);
            if (String.IsNullOrEmpty(name))
                return false;
            string jsonStr = System.Text.Encoding.UTF8.GetString(dataBuffer, end, size);
            object data = JsonMapper.ToObject(jsonStr);
            return true;
        }

        // 将数据打包
        public bool Encode(string name, byte[] dataBuffer)
        {
            if (String.IsNullOrEmpty(name))
                return false;
            Int32 len = name.Length;
            byte[] lenBytes = BitConverter.GetBytes(len);
            byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(name);
            dataBuffer = lenBytes.Concat(strBytes).Concat(dataBuffer).ToArray();
            Int32 size = dataBuffer.Length;
            byte[] sizeBytes = BitConverter.GetBytes(size);
            dataBuffer = sizeBytes.Concat(dataBuffer).ToArray();
            return true;
        }

        // 原理就是将int转为Byte类型然后concat，加入bytes数组，数组引用传递
        public static void AddInt(byte[] dataBuffer, int num)
        {
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
            return BitConverter.ToInt32(dataBuffer, start);
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
                return "";
            if (dataBuffer.Length < start + sizeof(Int32))
                return "";
            Int32 strLen = BitConverter.ToInt32(dataBuffer, start);
            if (dataBuffer.Length < start + sizeof(Int32) + strLen)
                return "";
            string str = System.Text.Encoding.UTF8.GetString(dataBuffer, start + sizeof(Int32), strLen);
            end = start + sizeof(Int32) + strLen;
            return str;
        }

        public static string GetString(byte[] dataBuffer, int start)
        {
            int end = 0;
            return GetString(dataBuffer, start, ref end);
        }


    }
}
