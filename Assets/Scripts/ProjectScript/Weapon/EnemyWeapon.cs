using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace ProjectScript
{
    /// <summary>
    /// 用于固定范围的Enemy攻击，负责战斗和碰撞检测，攻击相关属性
    /// 适用于带追踪功能的Enemy
    /// Trigger的开关交由Animation处理
    /// </summary>
    public class EnemyWeapon : IEnemyWeapon
    {
        public GameObject muzzleFlashEffect;
        private Transform player;
        private string bloodEffect = @"Particles\Blood";

        public override void Initialize()
        {
            base.Initialize();
            player = GameMainProgram.Instance.playerMgr.CurrentPlayer.GameObjectInScene.transform;
        }

        public override void Attack()
        {
            // 特效
            muzzleFlashEffect.SetActive(true);
            // 击中概率
            if (Random.Range(0,1) < 0.5f)
            {
                RealAttack = BasicAttack;
                // 传参给enemy告知受伤
                TransformForward = EnemyMedi.EnemyMono.transform.forward;
                PlayerHurtAttr.ModifyAttr((int)RealAttack, VelocityForward, VelocityVertical, TransformForward);
                player.GetComponent<IPlayerMono>().Hurt(PlayerHurtAttr);
                // 特效
                resourcesMgr.LoadAsset(bloodEffect, true, player.position + Vector3.up, Quaternion.identity);
            }
        }  // end_function
    }
}