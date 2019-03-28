using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace ProjectScript
{
    public class WeaponHandgun : IPlayerWeapon
    {
        public override void Attack()
        {
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
                    Debug.Log("击中");
                }
            }
        }  // end_function
    }
}