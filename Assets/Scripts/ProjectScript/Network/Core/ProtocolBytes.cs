using System;
using System.Collections;
using System.Linq;

namespace ProjectScript.Network
{
    /// <summary>
    /// 字节流协议模型
    /// </summary>
    public class ProtocolBytes : ProtocolBase
    {
        // 传输的字节流
        public byte[] bytes;

        // 解码器
        public override ProtocolBase Decode(byte[] readbuff, int start, int length)
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.bytes = new byte[length];
            Array.Copy(readbuff, start, protocol.bytes, 0, length);
            return protocol;
        }

        // 编码器
        public override byte[] Encode()
        {
            return bytes;
        }

        // 协议名称
        public override string GetName()
        {
            return GetString(0);
        }

        // 描述（由于是字节流，我们并不能确定各部分是什么类型）
        public override string GetDesc()
        {
            string str = "";
            if (bytes == null) return str;
            // 输出为数字形式
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = (int)bytes[i];
                str += b.ToString() + " ";
            }
            return str;
        }

        // 添加字符串
        public void AddString(string str)
        {
            Int32 len = str.Length;
            byte[] lenBytes = BitConverter.GetBytes(len);
            byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(str);
            if (bytes == null)
                bytes = lenBytes.Concat(strBytes).ToArray();
            else
                bytes = bytes.Concat(lenBytes).Concat(strBytes).ToArray();
        }

        // 从字节数组的start处开始读取字符串
        public string GetString(int start, ref int end)
        {
            if (bytes == null)
                return "";
            if (bytes.Length < start + sizeof(Int32))
                return "";
            Int32 strLen = BitConverter.ToInt32(bytes, start);
            if (bytes.Length < start + sizeof(Int32) + strLen)
                return "";
            string str = System.Text.Encoding.UTF8.GetString(bytes, start + sizeof(Int32), strLen);
            end = start + sizeof(Int32) + strLen;
            return str;
        }

        public string GetString(int start)
        {
            int end = 0;
            return GetString(start, ref end);
        }

        // 原理就是将转为Byte类型然后concat，加入bytes数组
        public void AddInt(int num)
        {
            byte[] numBytes = BitConverter.GetBytes(num);
            if (bytes == null)
                bytes = numBytes;
            else
                bytes = bytes.Concat(numBytes).ToArray();
        }

        // 安全方式获取，失败返回0
        public int GetInt(int start, ref int end)
        {
            if (bytes == null)
                return 0;
            if (bytes.Length < start + sizeof(Int32))
                return 0;
            end = start + sizeof(Int32);
            return BitConverter.ToInt32(bytes, start);
        }

        public int GetInt(int start)
        {
            int end = 0;
            return GetInt(start, ref end);
        }

        public void AddFloat(float num)
        {
            byte[] numBytes = BitConverter.GetBytes(num);
            if (bytes == null)
                bytes = numBytes;
            else
                bytes = bytes.Concat(numBytes).ToArray();
        }

        public float GetFloat(int start, ref int end)
        {
            if (bytes == null)
                return 0;
            if (bytes.Length < start + sizeof(float))
                return 0;
            end = start + sizeof(float);
            return BitConverter.ToSingle(bytes, start);
        }

        public float GetFloat(int start)
        {
            int end = 0;
            return GetFloat(start, ref end);
        }
    }
}