/*----------------------------------------------------------------------------
Author:
    Anotts
Date:
    2017/08/01
Description:
    简介：登录场景（需要网络）
    作用：登录验证
    使用：
    补充：
History:
----------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace ProjectScript
{
    public class LoginScene : ISceneState
    {
        public LoginScene(SceneStateController controller) : base(controller)
        {
            this.SceneName = "Login";
        }

        public override void StateBegin()
        {
            base.StateBegin();
            // 场景初始化
            //GameMainProgram.Instance.uiManager.ShowUIForms("FadeOut");
            //GameMainProgram.Instance.uiManager.ShowUIForms("ChatRoom");
            //gameMainProgram.audioMgr.PlayMusic(0);
        }

        public override void StateEnd()
        {
            base.StateEnd();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
        }

    }
}
