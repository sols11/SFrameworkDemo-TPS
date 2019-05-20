using System;
using System.Collections;

namespace ProjectScript.Network
{
    /// <summary>
    /// �ַ���Э��ģ��
    /// ��ʽ ����,����1,����2,����3
    /// </summary>
    public class ProtocolStr : ProtocolBase
    {
        // ������ַ���
        public string str;

        // ������
        public override ProtocolBase Decode(byte[] readbuff, int start, int length)
        {
            ProtocolStr protocol = new ProtocolStr();
            protocol.str = System.Text.Encoding.UTF8.GetString(readbuff, start, length);
            return (ProtocolBase)protocol;
        }

        // ������
        public override byte[] Encode()
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(str);
            return b;
        }

        // Э������
        public override string GetName()
        {
            if (str.Length == 0) return "";
            return str.Split(',')[0];
        }

        // Э������
        public override string GetDesc()
        {
            return str;
        }
    }
}