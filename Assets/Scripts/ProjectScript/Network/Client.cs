using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProjectScript
{
    class Client
    {
        public static void Awake()
        {
            Socket tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            EndPoint endPoint = new IPEndPoint(ipAddress, 8888);
            // Client连接EP
            tcpClient.Connect(endPoint);
            // 定义一个数组接收数据
            byte[] data = new byte[1024];
            // 接收到数据的长度
            int length = tcpClient.Receive(data);
            // 字节序列解码为string并显示出来
            Debug.Log(Encoding.UTF8.GetString(data, 0, length));
            // 向服务器端发送消息
            tcpClient.Send(Encoding.UTF8.GetBytes("客户端：服务器你好"));

            // 防止关闭
        }
    }
}
