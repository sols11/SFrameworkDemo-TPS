/*----------------------------------------------------------------------------
Author:
    Anotts
Date:
    2019/03/12
Description:
    简介：主玩家控制器，即默认的PlayerController
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
    public class PlayerMain : IPlayer
    {
        public override bool CanMove
        {
            get { return canMove; }
            set
            {
                h = 0; v = 0;
                canMove = value;
                Speed = 0;
                if (!canMove)
                    animator.SetFloat(aniSpeed, 0);
            }
        }

        // Animator States
        private string staCombatIdle = "handgun_combat_idle";
        private string staCombatWalk = "handgun_combat_walk";
        private string staCombatRun = "handgun_combat_run";
        private string staCombatShoot = "handgun_combat_shoot";
        private string staCombatRunShoot = "handgun_combat_run_shooting";
        // Parameters
        private string aniSpeed = "Speed";
        private string aniShoot = "Shoot";
        // Directions
        private Vector3 targetDirection;        // 输入的方向
        private Vector3 forwardDirection;       // 存储输入后的朝向
        private Transform mainCamera;

        /// <summary>
        /// 创建时的初始化
        /// </summary>
        /// <param name="gameObject"></param>
        public PlayerMain(GameObject gameObject) : base(gameObject)
        {
            // 以下是其他属性
            CanMove = true;
            CanRotate = true;
        }

        /// <summary>
        /// 每次切换场景时执行
        /// </summary>
        public override void Initialize()
        {
            if (Camera.main != null)
                mainCamera = Camera.main.transform;
            else
                Debug.LogWarning("No Main Camera Found! 将无法朝向相机正向移动");

        }

        public override void Release()
        {
            base.Release();
        }

        public override void FixedUpdate()
        {
            // 物理移动
            if (CanMove)
                if (stateInfo.IsName(staCombatIdle) || stateInfo.IsName(staCombatRun) || stateInfo.IsName(staCombatRunShoot))
                    GroundMove(h, v);
        }

        public override void Update()
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // 关于对话时禁止移动的办法，我们有做过InputMgr来实现，但暂时不添加这个功能
            MoveInput();
            AttackInput();
        }

        private void MoveInput()
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
            Vector3 inputDir = new Vector3(h, 0, v).normalized;

            if (mainCamera != null)
            {
                // 获得剔除y轴影响后的相机朝向（即mainCamera.forward在XZ平面上的投影）
                forwardDirection = Vector3.Scale(mainCamera.forward, new Vector3(1, 0, 1)).normalized;
                targetDirection = v * forwardDirection + h * mainCamera.right;
            }
            else
            {   
                // 用四元数绕Y轴旋转向量，使其和y朝向一致
                targetDirection = Quaternion.AngleAxis(0, Vector3.up) * inputDir;
            }
            // 移动状态时使用平滑旋转
            if (stateInfo.IsName(staCombatIdle) || stateInfo.IsName(staCombatRun) || stateInfo.IsName(staCombatRunShoot))
            {
                if (CanRotate)
                    Rotating();
            }
            // 是否切换为Run
            if (Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1)
                Speed = 1;
            else
                Speed = 0;
            animator.SetFloat(aniSpeed, Speed);
        }

        /// <summary>
        /// 平滑旋转
        /// </summary>
        private void Rotating()
        {
            //计算出旋转
            if (targetDirection != Vector3.zero)
            {
                // 目标方向的旋转角度
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                // 平滑插值
                Quaternion newRotation = Quaternion.Slerp(Rgbd.rotation, targetRotation, 5 * Time.deltaTime);
                Rgbd.MoveRotation(newRotation);
            }
        }

        /// <summary>
        /// 地面移动
        /// </summary>
        /// <param name="h"></param>
        /// <param name="v"></param>
        private void GroundMove(float h, float v)
        {
            // Speed = 4 村子里移动速度慢
            if (Speed != 0)
                Rgbd.MovePosition(GameObjectInScene.transform.position + targetDirection * 4 * Time.deltaTime);
        }

        private void AttackInput()
        {
            if (stateInfo.IsName(staCombatIdle) || stateInfo.IsName(staCombatShoot) || stateInfo.IsName(staCombatRun))
            {
                if(Input.GetButtonDown("Fire1"))
                    animator.SetTrigger(aniShoot);
            }
        }
    }
}
