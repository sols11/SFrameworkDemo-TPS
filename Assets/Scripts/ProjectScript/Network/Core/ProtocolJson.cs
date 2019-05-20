using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace ProjectScript.Network
{
    /// <summary>
    /// Json协议模型
    /// 形式：包头（int），消息名（msg），参数（args*）
    /// </summary>
    public class ProtocolJson
    {
        // 传输的对象
        public object data;
        public Dictionary<string, object> dict;
        // 还有一种方法是，针对要传输的json协议，每一种写一个对应的类型，decode时不反序列化，拿到对应的协议去反序列化

        // 解码器（将字节流前半部分的headPack提取出来，再将后半部分的Json字节流反序列化出来）
        public Dictionary<string, object> Decode(byte[] readbuff)
        {
            // 先拿到headpack中的size
            byte[] temp = new byte[sizeof(Int32)];
            Array.Copy(readbuff, temp, sizeof(Int32));
            Int32 size = BitConverter.ToInt32(temp, 0);
            // 然后反序列化Json字符串
            string jsonStr = System.Text.Encoding.UTF8.GetString(readbuff, sizeof(Int32), size);
            dict = JsonMapper.ToObject<Dictionary<string, object>>(jsonStr);
            //data = JsonMapper.ToObject(jsonStr);
            return dict;
        }

        // 编码器（将数据序列化为Json字节流，再添加上headPack）
        public byte[] Encode(object data)
        {
            string jsonStr = JsonMapper.ToJson(data);
            jsonStr = jsonStr.Length.ToString() + jsonStr; // 添加size
            return System.Text.Encoding.UTF8.GetBytes(jsonStr);
        }

        // 协议名称
        public string GetName()
        {
            object name = null;
            if (dict != null)
                dict.TryGetValue("msg", out name);
            return name.ToString();
        }

    }
}