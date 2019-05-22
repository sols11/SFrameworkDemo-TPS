using UnityEngine;
using UnityEngine.UI;
using SFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectScript.Network;

namespace ProjectScript
{
    public class UINetwork : ViewBase
    {
        // 服务器IP和端口
        public InputField inputHost;
        public InputField inputPort;
        // 客户端地址信息
        public Text clientInfo;
        public Text chatText;
        [Header("面版")]
        public GameObject panelLink;
        public GameObject panelLogin;
        public GameObject panelChat;
        // 登录信息
        public InputField inputUsername;
        public InputField inputPassword;
        public InputField inputText;
        // Button
        public Button btnDisconnect;
        public Button btnLink;
        public Button btnLogin;
        public Button btnRegister;
        public Button btnSend;

        private void Start()
        {
            btnDisconnect.onClick.AddListener(OnDisconnectButton);
            btnLink.onClick.AddListener(OnLinkButton);
            btnRegister.onClick.AddListener(OnRegisterButton);
            btnLogin.onClick.AddListener(OnLoginButton);
            btnSend.onClick.AddListener(OnSendButton);
        }

        // Connect连接
        private void OnLinkButton()
        {
            // 检查为空
            if (inputHost.text == "" || inputPort.text == "")
            {
                Debug.LogWarning("IP地址和端口号不能为空!");
                return;
            }

            string host = inputHost.text;
            int port = int.Parse(inputPort.text);
            if (!NetMgr.srvConn.Connect(host, port))
                return;
            // 客户端地址更新
            clientInfo.text = NetMgr.srvConn.ToString();
            panelLogin.SetActive(true);
            panelLink.SetActive(false);
        }

        // 客户端主动断开连接
        private void OnDisconnectButton()
        {
            NetMgr.srvConn.Send("Disconnect");
            Debug.Log("[客户端] 断开连接");
            NetMgr.srvConn.Close();
        }

        private void OnRegisterButton()
        {
            // 用户名密码为空
            if (inputUsername.text == "" || inputPassword.text == "")
            {
                Debug.LogWarning("用户名密码不能为空!");
                return;
            }
            Dictionary<string, string> reg = new Dictionary<string, string>();
            // 客户端也做一遍检查
            if (!NetMgr.IsSafeStr(inputUsername.text) || !NetMgr.IsSafeStr(inputPassword.text))
                return;
            reg["id"] = inputUsername.text;
            reg["pw"] = inputPassword.text;

            string jsonStr = LitJson.JsonMapper.ToJson(reg);
            NetMgr.srvConn.Send("Register", jsonStr);
        }

        private void OnLoginButton()
        {
            // 用户名密码为空
            if (inputUsername.text == "" || inputPassword.text == "")
            {
                Debug.LogWarning("用户名密码不能为空!");
                return;
            }
            Dictionary<string, string> login = new Dictionary<string, string>();
            // 客户端也做一遍检查
            if (!NetMgr.IsSafeStr(inputUsername.text) || !NetMgr.IsSafeStr(inputPassword.text))
                return;
            login["id"] = inputUsername.text;
            login["pw"] = inputPassword.text;

            string jsonStr = LitJson.JsonMapper.ToJson(login);
            NetMgr.srvConn.Send("Login", jsonStr);
        }

        private void OnSendButton()
        {

        }

    }
}