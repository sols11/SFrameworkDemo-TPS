using System.Collections;

namespace ProjectScript.Network
{
    // 协议基类
    public class ProtocolBase
    {
        // 解码器（转protocol），解码readbuff中从start开始的length字节
        public virtual ProtocolBase Decode(byte[] readbuff, int start, int length)
        {
            return new ProtocolBase();
        }

        // 编码器（返回字节流）
        public virtual byte[] Encode()
        {
            return new byte[] { };
        }

        // 协议名称，用于消息分发
        public virtual string GetName()
        {
            return "";
        }

        // 描述
        public virtual string GetDesc()
        {
            return "";
        }
    }
}