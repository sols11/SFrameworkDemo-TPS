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
    public class UIPauseMenu : ViewBase
    {
        public Button btnExit;

        private void Start()
        {
            btnExit.onClick.AddListener(OnExitButton);
        }

        // 客户端主动断开连接
        private void OnExitButton()
        {
            NetMgr.srvConn.Send("Disconnect");
            Debug.Log("[客户端] 断开连接");
            NetMgr.srvConn.Close();
            GameLoop.Instance.sceneStateController.ExitApplication();
        }
    }
}
