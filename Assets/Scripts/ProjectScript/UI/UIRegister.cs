using UnityEngine;
using SFramework;
using ProjectScript.Network;
using UnityEngine.UI;

namespace ProjectScript
{
    public class UIRegister : ViewBase
    {
        public InputField idInput;
        public InputField pwInput;
        public Button regBtn;
        public Button closeBtn;

        private void Awake()
        {
            base.UIForm_Type = UIFormType.PopUp;
            base.UIForm_ShowMode = UIFormShowMode.ReverseChange;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;
        }

        public void OnCloseClick()
        {
            CloseUIForm();
        }

        public void OnRegClick()
        {
            // 用户名密码为空
            if (idInput.text == "" || pwInput.text == "")
            {
                Debug.Log("用户名密码不能为空!");
                return;
            }

            if (NetMgr.srvConn.status != Connection.Status.Connected)
            {
                NetMgr.ConnectServ();
            }
            // 发送
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("Register");
            protocol.AddString(idInput.text);
            protocol.AddString(pwInput.text);
            Debug.Log("发送 " + protocol.GetDesc());
            NetMgr.srvConn.Send(protocol, OnRegBack);
        }

        public void OnRegBack(ProtocolBase protocol)
        {
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            if (ret == 0)
            {
                Debug.Log("注册成功!");
                CloseUIForm();
            }
            else
            {
                Debug.Log("注册失败!");
            }
        }
    }
}
