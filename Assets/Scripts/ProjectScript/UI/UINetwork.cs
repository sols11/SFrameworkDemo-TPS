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
        }

        private void OnLoginButton()
        {

        }

        private void OnSendButton()
        {

        }

    }
}