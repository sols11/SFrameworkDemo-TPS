using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectScript
{
    public class ChatRoom : MonoBehaviour
    {
        public string connectIP = "127.0.0.1";
        public int port = 8888;
        public bool isConnected = false;
        public InputField inputField;
        public Text charText;

        private Socket clientSocket;
        private Thread thread;
        /// <summary>
        /// 数据容器
        /// </summary>
        private byte[] data = new byte[1024];
        /// <summary>
        /// 消息容器
        /// </summary>
        private string message = string.Empty;

        private void Start()
        {
            ConnectToServer();
        }

        private void Update()
        {
            if (message != string.Empty)
            {
                charText.text += "\n" + message;
                message = string.Empty;
            }
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
                OnSendButtonClick();
        }

        private void OnServerInitialized()
        {
            isConnected = true;
        }

        private void OnConnectedToServer()
        {
            isConnected = false;
        }

        private void OnDestroy()
        {
            // 关闭连接
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        private void ConnectToServer()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(connectIP), port));
            thread = new Thread(Receive);
            thread.Start();
        }

        /// <summary>
        /// 发送给服务器
        /// </summary>
        /// <param name="message"></param>
        private void Send(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(data);
        }

        /// <summary>
        /// 接受自服务器。服务器会将其收到的消息发送过来
        /// </summary>
        private void Receive()
        {
            // 循环接受消息
            while (true)
            {
                // 判断是否断开
                if (clientSocket.Poll(10, SelectMode.SelectRead))
                {
                    clientSocket.Close();
                    // 终止线程
                    return;
                }
                int result = clientSocket.Receive(data);
                string message = Encoding.UTF8.GetString(data, 0, result);
                // 因为无法在线程中使用UnityAPI，所以通过Update检测修改实现
                this.message = message;
            }
        }

        /// <summary>
        /// 将信息发送给Server，然后服务器会转发回来更新UI
        /// </summary>
        public void OnSendButtonClick()
        {
            string text = inputField.text;
            Send(text);
            inputField.text = string.Empty;
        }
    }
}