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
        public override void Attack()
        {
            base.Attack();
        }
    }
}