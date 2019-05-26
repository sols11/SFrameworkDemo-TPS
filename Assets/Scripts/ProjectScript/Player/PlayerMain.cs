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
                horizontal = 0; vertical = 0;
                canMove = value;
                Speed = 0;
                if (!canMove)
                    animator.SetFloat(animSpeed, 0);
            }
        }
        // Fields
        private float moveSpeed;
        private float jumpPower = 6;
        private bool atGround = true;
        private bool isCombatState = true;
        private float fireRate = 0.5f;      // 开火需要间隔的时间
        private float fireTimer = 0;
        private int groundLayerIndex;
        // Animator States
        private string staCombatMove = "HandgunCombatMove";
        private string staCombatWalk = "handgun_combat_walk";
        private string staCombatRun = "handgun_combat_run";
        private string staCombatShoot = "handgun_combat_shoot";
        private string staCombatRunShoot = "handgun_combat_run_shooting";
        private string staJumpStart = "handgun_jump_1_start";
        private string staJumpAir = "handgun_jump_2_air";
        private string staJumpLand = "handgun_jump_3_land";
        private string staReload = "handgun_combat_reload";
        // Parameters
        private string animHurt = "Hurt";
        private string animDead = "Dead";
        private string animSpeed = "Speed";
        private string animSpeedX = "SpeedX";
        private string animSpeedY = "SpeedY";
        private string animJump = "Jump";
        private string animShoot = "Shoot";
        private string animRunShoot = "RunShoot";
        private string animReload = "Reload";
        private string animAtGround = "AtGround";
        // Directions
        private Vector3 targetDirection;        // 输入的方向
        private Vector3 forwardDirection;       // 存储输入后的朝向
        private Transform mainCamera;
        private float zoomInFov = 28;           // 瞄准时的fov
        private float zoomTime = 0.15f;         // changeFov所需时间
        private float originalFov;              // 初始fov
        private float zoomLevel = 0;            // 插值
        private float zoomLevelVelocity;

        /// <summary>
        /// 创建时的初始化
        /// </summary>
        /// <param name="gameObject"></param>
        public PlayerMain(GameObject gameObject) : base(gameObject)
        {
            // 以下是其他属性
            CanMove = true;
            CanRotate = true;
            Speed = 4;
        }

        /// <summary>
        /// 每次切换场景时执行
        /// </summary>
        public override void Initialize()
        {
            if (Camera.main != null)
            {
                mainCamera = Camera.main.transform;
                originalFov = Camera.main.fieldOfView;
            }
            else
                Debug.LogWarning("No Main Camera Found! 将无法朝向相机正向移动");
            moveSpeed = Speed;
            groundLayerIndex = LayerMask.GetMask("Ground");
        }

        public override void Release()
        {
            base.Release();
        }

        public override void FixedUpdate()
        {
            atGround = Physics.Raycast(GameObjectInScene.transform.position, Vector3.down, 0.1f, groundLayerIndex);

            // 物理移动和旋转（设计在落地动画前段不可移动）
            if (stateInfo.IsName(staCombatMove) || stateInfo.IsName(staCombatRunShoot) || stateInfo.IsName(staJumpStart) || stateInfo.IsName(staJumpAir) || (stateInfo.IsName(staJumpLand) && stateInfo.normalizedTime > 0.8f))
            {
                if (CanMove)
                    GroundMove();
                if (CanRotate)
                    Rotating();
            }
        }

        public override void Update()
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            animator.SetBool(animAtGround, atGround);
            // 关于对话时禁止移动的办法，我们有做过InputMgr来实现，但暂时不添加这个功能
            MoveInput();
            AttackInput();
            BasicInput();
        }

        /// <summary>
        /// 接受并计算方向移动输入
        /// </summary>
        private void MoveInput()
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            Vector3 inputDir = new Vector3(horizontal, 0, vertical).normalized;

            if (mainCamera != null)
            {
                // 获得剔除y轴影响后的相机朝向（即mainCamera.forward在XZ平面上的投影）
                forwardDirection = Vector3.Scale(mainCamera.forward, new Vector3(1, 0, 1)).normalized;
                targetDirection = vertical * forwardDirection + horizontal * mainCamera.right;
            }
            else
            {   
                // 用四元数绕Y轴旋转向量，使其和y朝向一致
                targetDirection = Quaternion.AngleAxis(0, Vector3.up) * inputDir;
            }
            // 是否处于移动状态
            if (Mathf.Abs(horizontal) > 0.1 || Mathf.Abs(vertical) > 0.1)
                moveSpeed = Speed;
            else
                moveSpeed = 0;
            animator.SetFloat(animSpeedX, horizontal);
            animator.SetFloat(animSpeedY, vertical);

            // 跳跃
            if (atGround && Input.GetButtonDown("Jump"))
            {
                Rgbd.velocity = new Vector3(Rgbd.velocity.x, jumpPower, Rgbd.velocity.z);
                animator.SetTrigger(animJump);
            }
        }

        /// <summary>
        /// 平滑旋转
        /// </summary>
        private void Rotating()
        {
            // Combat状态下，角色朝向随相机朝向变化而变化
            if (isCombatState && forwardDirection != Vector3.zero)
            {
                // 目标方向的旋转角度
                Quaternion targetRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);     
                // 不需要平滑插值
                Rgbd.MoveRotation(targetRotation);
            }
            // Normal状态下，角色朝向随输入方向变化而变化
            else if (targetDirection != Vector3.zero)
            {
                // 目标方向的旋转角度
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                // 平滑插值
                Quaternion newRotation = Quaternion.Slerp(Rgbd.rotation, targetRotation, 5 * Time.deltaTime);
                Rgbd.MoveRotation(targetRotation);
            }
        }

        /// <summary>
        /// 地面移动
        /// </summary>
        /// <param name="h"></param>
        /// <param name="v"></param>
        private void GroundMove()
        {
            // Speed = 4
            if (moveSpeed > 1e-2)
                Rgbd.MovePosition(GameObjectInScene.transform.position + targetDirection * 4 * Time.deltaTime);
        }

        private void BasicInput()
        {
            // 拔枪，切换状态
            if (Input.GetButtonDown("Draw"))
                isCombatState = !isCombatState;
            else if (Input.GetButtonDown("Reload"))
            {
                PlayerMedi.PlayerWeapon.Reload();
                Reloading();
            }
            if (Input.GetButton("Fire2"))    // 鼠标右键瞄准
            {
                zoomLevel = Mathf.SmoothDamp(zoomLevel, 1, ref zoomLevelVelocity, zoomTime);    // 随着时间的推移逐渐将值改变为期望的目标
            }
            else if(Camera.main.fieldOfView < originalFov - 0.01f)
            {
                zoomLevel = Mathf.SmoothDamp(zoomLevel, 0, ref zoomLevelVelocity, zoomTime);
            }
            Camera.main.fieldOfView = Mathf.Lerp(originalFov, zoomInFov, zoomLevel);
        }

        private void AttackInput()
        {
            fireTimer -= Time.deltaTime;

            if (stateInfo.IsName(staCombatMove) || stateInfo.IsName(staCombatShoot) || stateInfo.IsName(staCombatRunShoot))
            {
                if (Input.GetButtonDown("Fire1") && fireTimer <= 0)
                {
                    ShootFire();
                    fireTimer = fireRate;
                }
            }
        }

        private void ShootFire()
        {
            if(moveSpeed > 1e-2)
                animator.SetTrigger(animRunShoot);
            else
                animator.SetTrigger(animShoot);
            PlayerMedi.PlayerWeapon.Attack();
        }

        public void Reloading()
        {
            animator.SetTrigger(animReload);
        }

        public override void Hurt(PlayerHurtAttr playerHurtAttr)
        {
            if (stateInfo.IsName("Dead"))
                return;
            animator.SetTrigger(animHurt);
            CurrentHP -= playerHurtAttr.Attack;
            Debug.Log("PlayerHurt:" + playerHurtAttr.Attack);
        }

        public override void Dead()
        {
            base.Dead();
            animator.SetTrigger(animDead);
        }
    }
}
