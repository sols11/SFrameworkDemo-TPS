/*----------------------------------------------------------------------------
Author:
    Anotts
Date:
    2019/03/12
Description:
    简介：默认敌人
    作用：
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
    public class EnemyMain : IEnemy
    {
        private string aniHurt = "Hurt";
        private string aniDead = "Dead";

        public EnemyMain(GameObject gameObject) : base(gameObject)
        {
            MoveSpeed = 1;
            RotSpeed = 1;
            Name = "Enemy";
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Update()
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 需要使用
        }

        public override EnemyAction Hurt(EnemyHurtAttr enemyHurtAttr)
        {
            if (!IsDead)
            {
                // 伤害计算
                int damage = enemyHurtAttr.Attack;
                CurrentHP -= damage;
                animator.SetTrigger(aniHurt);
                Debug.Log("EnemyHurt:" + damage);
            }
            return EnemyAction.Hurt;
        }

        public override void Dead()
        {
            base.Dead();
            animator.SetTrigger(aniDead);
            // GameObjectInScene.GetComponent<Collider>().enabled = false;
            GameMainProgram.Instance.eventMgr.InvokeEvent(EventName.BossDead);
        }

    }
}