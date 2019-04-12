/*----------------------------------------------------------------------------
Author:
    Anotts
Date:
    2017/08/01
Description:
    简介：Player的MonoBehaviour脚本，用于武器碰撞检测和动画事件等Mono相关的功能。
    作用：挂载到Player的GameObject上以控制游戏对象
    使用：继承并添加代码。
    补充：由PlayerMediator初始化，不使用Mono生命周期
History:
----------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class IPlayerMono : ICharacterMono
	{
		public IPlayerWeapon iPlayerWeapon;      //在预制时赋好的变量
        protected AnimatorStateInfo stateInfo;
        public PlayerMediator PlayerMedi { get; set; } 

        /// <summary>
        /// Release也不会删除GO。
        /// </summary>
        public override void Release()
        {
            iPlayerWeapon.Release();
        }

        /// <summary>
        /// 提供给EnemyAttack调用
        /// </summary>
        /// <param name="damage"></param>
        public virtual void Hurt(PlayerHurtAttr _playerHurtAttr)
		{
            PlayerMedi.Player.Hurt(_playerHurtAttr);
		}

        /// <summary>
        /// 因为Animator不能在AnimationCurve设定所以只好用帧事件
        /// </summary>
	    public virtual void AnimatorRootMotion(int open)
	    {
            if(open==1)
	            AnimatorComponent.applyRootMotion=true;
            else
	            AnimatorComponent.applyRootMotion =false;
	    }
    }
}