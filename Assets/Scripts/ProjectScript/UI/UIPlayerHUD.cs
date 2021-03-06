﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;
using UnityEngine.UI;

namespace ProjectScript
{
    public class UIPlayerHUD : ViewBase
    {
        public Image uiHP;
        private IPlayer player;

        void Awake()
        {
            //定义本窗体的性质(默认数值，可以不写)
            base.UIForm_Type = UIFormType.Normal;
            base.UIForm_ShowMode = UIFormShowMode.Normal;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;
            // 观察者注册
            player = GameMainProgram.Instance.playerMgr.CurrentPlayer;
            GameMainProgram.Instance.eventMgr.StartListening(EventName.PlayerHP_SP, this.UpdateUI);
            UpdateUI(); // 进行初始化
        }

        void OnDestroy()
        {
            GameMainProgram.Instance.eventMgr.StopListening(EventName.PlayerHP_SP, this.UpdateUI);
        }

        public override void UpdateUI()
        {
            // 计算出fillAmount
            if (uiHP != null)
            {
                uiHP.fillAmount = player.HPpercent;
            }
        }
    }
}
