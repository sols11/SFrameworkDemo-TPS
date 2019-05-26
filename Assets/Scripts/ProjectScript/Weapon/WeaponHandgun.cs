using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace ProjectScript
{
    public class WeaponHandgun : IPlayerWeapon
    {
        public Transform bulletPosition;
        public Transform shellPosition;
        public GameObject muzzleFlashEffect;
        private string shootEffect = @"Particles\BulletTrail";
        private string shellEffect = @"Particles\EmptyShell";
        private string holeEffect = @"Particles\StoneBulletHole";
        private string hitEffect = @"Particles\StoneHitFx";
        private string bloodEffect = @"Particles\Blood";
        private Vector3 emptyShellMinForce = new Vector3(0.8f, -0.5f, 0.2f);
        private Vector3 emptyShellMaxForce = new Vector3(-1.2f, 0.8f, 0.4f);

        public override void Attack()
        {
            // 子弹数计算
            if (CurrentBulletCount <= 0)
            {
                Reload();
                return;
            }
            --CurrentBulletCount;
            GameMainProgram.Instance.eventMgr.InvokeEvent(EventName.BulletCount);

            // 特效
            muzzleFlashEffect.SetActive(true);
            resourcesMgr.LoadAsset(shootEffect, true, bulletPosition.position, Quaternion.identity);
            // 子弹壳
            Vector3 randomEmptyShellForce = new Vector3(Random.Range(emptyShellMinForce.x,  emptyShellMaxForce.x), Random.Range( emptyShellMinForce.y,  emptyShellMaxForce.y), Random.Range( emptyShellMinForce.z,  emptyShellMaxForce.z));
            GameObject emptyShell = resourcesMgr.LoadAsset(shellEffect, true, shellPosition.position, shellPosition.rotation);
            emptyShell.GetComponent<Rigidbody>().AddForce(shellPosition.rotation * randomEmptyShellForce, ForceMode.Impulse);

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
                    resourcesMgr.LoadAsset(bloodEffect, true, hit.point + (hit.normal * .04f), Quaternion.LookRotation(hit.normal));
                }
                else  // 打在物体上
                {
                    resourcesMgr.LoadAsset(holeEffect, true, hit.point + (hit.normal * .04f), Quaternion.LookRotation(hit.normal) * Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
                    resourcesMgr.LoadAsset(hitEffect, true, hit.point + (hit.normal * .07f), Quaternion.LookRotation(hit.normal) * Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
                }
            }
        }  // end_function
    }
}