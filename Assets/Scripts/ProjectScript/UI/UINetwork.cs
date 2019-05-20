using UnityEngine;
using UnityEngine.UI;
using SFramework;
using System;
using System.Net.Sockets;
using System.Linq;


namespace ProjectScript
{
    public class UINetwork : ViewBase
    {
        // 服务器IP和端口
        public InputField hostInput;
        public InputField portInput;
        // 客户端地址信息
        public Text ClientInfo;
        public Text ChatText;
        [Header("面版")]
        public GameObject PanelLink;
        public GameObject PanelLogin;
        public GameObject PanelChat;
        // 登录信息
        public InputField InputUsername;
        public InputField InputPassword;
        public InputField InputText;
        // Button
        public Button ButtonLink;
        public Button ButtonLogin;
        public Button ButtonRegister;
        public Button ButtonSend;
        // Socket和接收缓冲区
        [NonSerialized]
        public byte[] readBuff = new byte[BUFFER_SIZE];
        private Socket socket;
        private const int BUFFER_SIZE = 1024;
        // 粘包分包用
        private int buffCount = 0;
        byte[] lenBytes = new byte[sizeof(UInt32)];
        Int32 msgLength = 0;
        // Json协议


        private void Start()
        {
            ButtonLink.onClick.AddListener(OnLinkButton);
            ButtonLogin.onClick.AddListener(OnLoginButton);
            ButtonSend.onClick.AddListener(OnSendButton);
        }

        // Connect连接
        private void OnLinkButton()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string host = hostInput.text;
            int port = int.Parse(portInput.text);
            socket.Connect(host, port);
            // 客户端地址更新
            ClientInfo.text = socket.LocalEndPoint.ToString();
            PanelLogin.SetActive(true);
            PanelLink.SetActive(false);
            //Recv
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }

        // 接收回调
        private void ReceiveCb(IAsyncResult ar)
        {
            try
            {
                // count是接收数据的大小
                int count = socket.EndReceive(ar);
                // 数据处理
                buffCount += count;
                ProcessData();
                // 继续接收	
                socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
            }
            catch (Exception e)
            {
                socket.Close();
                //ClientInfo.text += "链接已断开";
            }
        }

        /// <summary>
        /// 处理信息
        /// </summary>
        private void ProcessData()
        {
            // 小于长度字节
            if (buffCount < sizeof(Int32))
                return;
            // 消息长度
            Array.Copy(readBuff, lenBytes, sizeof(Int32));
            msgLength = BitConverter.ToInt32(lenBytes, 0);
            if (buffCount < msgLength + sizeof(Int32))
                return;
            // 处理消息
            ProtocolBase protocol = proto.Decode(readBuff, sizeof(Int32), msgLength);
            HandleMsg(protocol);
            // 清除已处理的消息
            int count = buffCount - msgLength - sizeof(Int32);
            Array.Copy(readBuff, msgLength, readBuff, 0, count);
            buffCount = count;
            if (buffCount > 0)
            {
                ProcessData();
            }
        }

        private void HandleMsg(ProtocolBase protoBase)
        {
            ProtocolBytes proto = (ProtocolBytes)protoBase;
            //获取数值
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            //显示
            Debug.Log("接收 " + proto.GetDesc());
            //ClientInfo.text = "接收 " + proto.GetName() + " " + ret.ToString();
        }

        /// <summary>
        /// 根据协议发送
        /// </summary>
        /// <param name="protocol"></param>
        public void Send(ProtocolBase protocol)
        {
            byte[] bytes = protocol.Encode();
            byte[] length = BitConverter.GetBytes(bytes.Length);
            byte[] sendbuff = length.Concat(bytes).ToArray();
            socket.Send(sendbuff);
        }


    }
}