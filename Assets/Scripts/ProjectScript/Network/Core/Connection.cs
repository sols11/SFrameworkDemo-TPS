using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ProjectScript.Network
{
    /// 网络链接
    public class Connection
    {
        // 常量
        const int BUFFER_SIZE = 1024;
        // Socket
        private Socket socket;
        // Buffer
        private byte[] readBuff = new byte[BUFFER_SIZE];
        private int buffCount = 0;
        // 沾包分包
        private Int32 msgLength = 0;
        private byte[] lenBytes = new byte[sizeof(Int32)];
        // 协议
        public ProtocolBase proto;
        // 心跳时间
        public float lastTickTime = -10000;
        public float heartBeatTime = 30;
        // 消息分发
        public MsgDistribution msgDist = new MsgDistribution();
        /// 状态
        public enum Status
        {
            None,
            Connected,
        };
        public Status status = Status.None;

        // 连接服务端
        public bool Connect(string host, int port)
        {
            try
            {
                // Socket
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Connect
                socket.Connect(host, port);
                // BeginReceive
                socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None,
                          ReceiveCb, readBuff);
                Debug.Log("[客户端] 连接服务器成功");
                // 状态
                status = Status.Connected;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("[客户端] 连接服务器失败:" + e.Message);
                return false;
            }
        }

        // 关闭连接
        public bool Close()
        {
            try
            {
                socket.Close();
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("[客户端] 关闭失败:" + e.Message);
                return false;
            }
        }

        // 接收回调
        private void ReceiveCb(IAsyncResult ar)
        {
            try
            {
                int count = socket.EndReceive(ar);
                buffCount = buffCount + count;
                ProcessData();
                socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None,
                         ReceiveCb, readBuff);
            }
            catch (Exception e)
            {
                Debug.Log("[客户端] ReceiveCb失败:" + e.Message);
                status = Status.None;
            }
        }

        // 消息处理
        private void ProcessData()
        {
            // 小于长度字节
            if (buffCount < sizeof(Int32))
                return;
            // 消息长度（先拿到headpack中的size）
            int end = 0;
            msgLength = Protocol.GetInt(readBuff, 0, ref end);
            Debug.Log("[系统] 数据包长度：" + msgLength);
            if (msgLength <= 0)
            {
                Debug.LogWarning("接受了错误的数据，无法GetInt，丢弃数据");
                // 清除不符合协议的消息
                Array.Clear(readBuff, 0, buffCount);
                buffCount = 0;
                return;
            }
            if (buffCount < msgLength + sizeof(Int32))
                return;
            // 创建Protocol，并存放body内容
            Protocol protocol = new Protocol(readBuff, end, msgLength);
            // 处理消息（交给MsgDistribution处理）
            lock (msgDist.msgList)
            {
                msgDist.msgList.Add(protocol);
            }
            // 清除已处理的消息
            int remainSize = buffCount - msgLength - sizeof(Int32);
            Array.Copy(readBuff, sizeof(Int32) + msgLength, readBuff, 0, remainSize);
            buffCount = remainSize;
            if (buffCount > 0)
            {
                ProcessData();
            }
        }

        // 原先是发送Protocol，我们改成直接发byte[]，该接口会修改body数据
        public bool Send(string name, byte[] body = null)
        {
            if (status != Status.Connected)
            {
                Debug.LogError("[Connection] 发送消息前需要先连接");
                return false;
            }
            body = Protocol.Encode(name, body);
            if (body == null)
                return false;
            socket.Send(body);
            Debug.Log("[客户端] 发送消息 " + name);
            return true;
        }

        public bool Send(string name, string str)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            return Send(name, bytes);
        }

        // 发送消息的同时绑定回调函数，并指定key名
        public bool Send(string name, byte[] body, string cbName, MsgDistribution.Delegate cb)
        {
            if (!Send(name, body))
                return false;
            msgDist.AddOnceListener(cbName, cb);
            return true;
        }

        // 发送消息的同时绑定回调函数，使用Msg+name作为key名
        public bool Send(string name, byte[] body, MsgDistribution.Delegate cb)
        {
            string cbName = "Msg" + name;
            return Send(name, body, cbName, cb);
        }

        // Update中进行消息接受后处理，以及发送心跳包
        public void Update()
        {
            // 消息
            msgDist.Update();
            // 心跳
            if (status == Status.Connected)
            {
                if (Time.time - lastTickTime > heartBeatTime)
                {
                    Send("HeartBeat");
                    lastTickTime = Time.time;
                }
            }
        }

        // 显示地址和端口信息
        public override string ToString()
        {
            return socket.LocalEndPoint.ToString();
        }
    }
}