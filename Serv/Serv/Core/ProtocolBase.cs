using System.Collections;

namespace Network
{
    // Э�����
    public class ProtocolBase
    {
        // ������������readbuff�д�start��ʼ��length�ֽ�
        public virtual ProtocolBase Decode(byte[] readbuff, int start, int length)
        {
            return new ProtocolBase();
        }

        // ������
        public virtual byte[] Encode()
        {
            return new byte[] { };
        }

        // Э�����ƣ�������Ϣ�ַ�
        public virtual string GetName()
        {
            return "";
        }

        // ����
        public virtual string GetDesc()
        {
            return "";
        }
    }
}