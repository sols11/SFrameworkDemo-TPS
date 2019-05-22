using UnityEngine;
using SFramework;
using ProjectScript.Network;
using UnityEngine.UI;

namespace ProjectScript
{
    public class UILogin : ViewBase
    {
        public InputField idInput;
        public InputField pwInput;
        public Button loginBtn;
        public Button regBtn;

        private void Awake()
        {
            base.UIForm_Type = UIFormType.Normal;
            base.UIForm_ShowMode = UIFormShowMode.Normal;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;
        }

        private void Start()
        {
            loginBtn.onClick.AddListener(OnLoginClick);
            regBtn.onClick.AddListener(OnRegClick);
        }

        public void OnRegClick()
        {
            UI_Manager.ShowUIForms("Register");
            CloseUIForm();
        }

        public void OnLoginClick()
        {
            // 若用户名密码为空
            if (idInput.text == "" || pwInput.text == "")
            {
                Debug.Log("用户名密码不能为空!");
                return;
            }

            if (NetMgr.srvConn.status != Connection.Status.Connected)
            {
                //NetMgr.ConnectServ();
            }
            // 发送
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("Login");
            protocol.AddString(idInput.text);
            protocol.AddString(pwInput.text);
            Debug.Log("发送 " + protocol.GetDesc());
            //NetMgr.srvConn.Send(protocol, OnLoginBack);
        }

        // 登录完毕回调
        public void OnLoginBack(ProtocolBase protocol)
        {
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            if (ret == 0)
            {
                Debug.Log("登录成功!");
                //开始游戏
                //Walk.instance.StartGame(idInput.text);
                //CloseUIForm();
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("登录失败!");
            }
        }
    }
}