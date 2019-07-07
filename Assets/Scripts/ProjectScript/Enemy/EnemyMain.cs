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
using LitJson;
using ProjectScript.Network;
using System;

namespace ProjectScript
{
    public class EnemyMain : IEnemy
    {
        // Parameters
        private string animHurt = "Hurt";
        private string animDead = "Dead";
        private string animSpeed = "Speed";
        private string animSpeedX = "SpeedX";
        private string animSpeedY = "SpeedY";
        private string animJump = "Jump";
        private string animShoot = "RunShoot";

        private Transform player;
        private Vector3 target = Vector3.up;  // 设置一个默认值
        private List<List<double>> pathList;
        private float timer = 0;

        public EnemyMain(GameObject gameObject) : base(gameObject)
        {
            MoveSpeed = 3;
            RotSpeed = 1;
            Name = "Enemy";
            // 更换装备
            EnemyMedi.Equip(GameMainProgram.Instance.dataBaseMgr.enemyEquipDict["步枪"]);
            EnemyMedi.Equip(GameMainProgram.Instance.dataBaseMgr.enemyEquipDict["防具"]);
            CurrentHP = MaxHP;
        }

        public override void Initialize()
        {
            player = GameMainProgram.Instance.playerMgr.CurrentPlayer.GameObjectInScene.transform;
            NetMgr.srvConn.msgDist.AddListener("Navigate", MsgNavigate);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Update()
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 需要使用
            // Find Player，Run，Shoot
            timer += Time.deltaTime;
            if (timer > 2f)
            {
                timer = 0;
                PlayerInfo playerInfo = new PlayerInfo("player", player.position);
                PlayerInfo enemyInfo = new PlayerInfo("enemy", GameObjectInScene.transform.position);
                List<PlayerInfo> players = new List<PlayerInfo>();
                players.Add(playerInfo);
                players.Add(enemyInfo);
                PositionSync.SendPosList(players);
                if (target != Vector3.up)
                {    
                    // 射击
                    animator.SetTrigger(animShoot);
                    CoroutineMgr.Instance.StartCoroutine(Attack());
                }
            }
        }

        private IEnumerator Attack()
        {
            yield return new WaitForSeconds(0.4f);
            EnemyMedi.EnemyWeapon.Attack();
        }

        public override void FixedUpdate()
        {
            if (target != Vector3.up)
            {
                GameObjectInScene.transform.LookAt(player); // 面向player
                GameObjectInScene.transform.eulerAngles = new Vector3(0.0f, GameObjectInScene.transform.eulerAngles.y, 0.0f);  // 设置Rotation属性，确保敌人只在y轴旋转
                GameObjectInScene.transform.position = Vector3.MoveTowards(GameObjectInScene.transform.position, target, Time.deltaTime * MoveSpeed); // 移动到指定位置
                // 动画方向计算（因为旋转了180度所以要反向）
                Vector3 dir = (target - GameObjectInScene.transform.position).normalized;
                animator.SetFloat(animSpeedX, -dir.x);
                animator.SetFloat(animSpeedY, -dir.z);

                if (Vector3.Distance(GameObjectInScene.transform.position, target) < 0.5f)
                {
                    // 到达
                    pathList.RemoveAt(0);
                    if (pathList.Count > 0)
                        target = new Vector3((float)pathList[0][0], (float)pathList[0][1], (float)pathList[0][2]);
                    else
                    {
                        // 终点
                        target = Vector3.up;
                        animator.SetFloat(animSpeedX, 0);
                        animator.SetFloat(animSpeedY, 0);
                    }
                }
            }
        }

        public override EnemyAction Hurt(EnemyHurtAttr enemyHurtAttr)
        {
            if (!IsDead)
            {
                // 伤害计算
                int damage = enemyHurtAttr.Attack;
                CurrentHP -= damage;
                animator.SetTrigger(animHurt);
                Debug.Log("EnemyHurt:" + damage);
            }
            return EnemyAction.Hurt;
        }

        public override void Dead()
        {
            base.Dead();
            animator.SetTrigger(animDead);
            target = Vector3.up; // 结束AI
            GameObjectInScene.layer = (int)ObjectLayer.Without;
            // GameObjectInScene.GetComponent<Collider>().enabled = false;
            GameMainProgram.Instance.eventMgr.InvokeEvent(EventName.BossDead);
            GameMainProgram.Instance.enemyMgr.enemysInScene.Remove(this);
            GameObject.Destroy(GameObjectInScene, 10);
        }

        public override void WhenPlayerDead()
        {
            target = Vector3.up; // 结束AI
        }

        private void MsgNavigate(Protocol protocol)
        {
            pathList = JsonMapper.ToObject<List<List<double>>>(protocol.bodyStr);
            target = new Vector3((float)pathList[0][0], (float)pathList[0][1], (float)pathList[0][2]);
        }

    }
}