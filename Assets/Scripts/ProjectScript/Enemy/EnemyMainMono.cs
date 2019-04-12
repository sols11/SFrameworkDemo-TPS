/*----------------------------------------------------------------------------
Author:
    Anotts
Date:
    2019/03/29
Description:
    简介：默认Enemy的Mono脚本
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
    public class EnemyMainMono : IEnemyMono
    {
        private EnemyMain enemyMain;
        
        public override void Initialize()
        {
            base.Initialize();
            enemyMain = EnemyMedi.Enemy as EnemyMain;
        }

    }
}