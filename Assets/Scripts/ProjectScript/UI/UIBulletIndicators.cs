using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;
using UnityEngine.UI;

namespace ProjectScript
{
    public class UIBulletIndicators : ViewBase
    {
        public Text currentBulletCount;
        public Text remainingBulletCount;
        public Text clipBulletCount;
        public Image weaponImage;
        private IPlayerWeapon playerWeapon;

        void Awake()
        {
            //定义本窗体的性质(默认数值，可以不写)
            base.UIForm_Type = UIFormType.Normal;
            base.UIForm_ShowMode = UIFormShowMode.Normal;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;
            // 观察者注册
            playerWeapon = GameMainProgram.Instance.playerMgr.CurrentPlayer.PlayerMedi.PlayerWeapon;
            currentBulletCount.text = playerWeapon.CurrentBulletCount.ToString();
            remainingBulletCount.text = playerWeapon.RemainingBulletCount.ToString();
            clipBulletCount.text = playerWeapon.CilpBulletCount.ToString();

            // 进行初始化
            GameMainProgram.Instance.eventMgr.StartListening(EventName.BulletCount, this.UpdateUI);
            UpdateUI(); 
        }

        void OnDestroy()
        {
            GameMainProgram.Instance.eventMgr.StopListening(EventName.BulletCount, this.UpdateUI);
        }

        public override void UpdateUI()
        {
            currentBulletCount.text = playerWeapon.CurrentBulletCount.ToString();
            remainingBulletCount.text = playerWeapon.RemainingBulletCount.ToString();
            clipBulletCount.text = playerWeapon.CilpBulletCount.ToString();
        }
    }
}
