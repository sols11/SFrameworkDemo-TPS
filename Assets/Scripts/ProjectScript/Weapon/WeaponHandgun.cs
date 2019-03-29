using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace ProjectScript
{
    public class WeaponHandgun : IPlayerWeapon
    {
        public Transform bulletPosition;
        public GameObject muzzleFlashEffect;
        private string shootEffect = @"Particles\BulletTrail";

        public override void Attack()
        {
            // 特效
            muzzleFlashEffect.SetActive(true);
            resourcesMgr.LoadAsset(shootEffect, true, bulletPosition.position, Quaternion.identity);

            // 碰撞检测
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.layer == (int)ObjectLayer.Enemy)
                {
                    RealAttack = BasicAttack * AttackFactor;
                    // 传参给enemy告知受伤
                    TransformForward = PlayerMedi.PlayerMono.transform.forward;
                    EnemyHurtAttribute.ModifyAttr((int)RealAttack, VelocityForward, VelocityVertical, TransformForward);
                    EnemyReturn = hit.transform.GetComponent<IEnemyMono>().Hurt(EnemyHurtAttribute);
                    // 特效
                }
            }
        }  // end_function
    }
}