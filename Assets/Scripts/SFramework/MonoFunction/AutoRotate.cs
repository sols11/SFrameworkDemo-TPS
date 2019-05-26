/*----------------------------------------------------------------------------
Author:
    Anotts
Date:
    2017/08/01
Description:
    简介：
    作用：自动旋转
    使用：
    补充：
History:
----------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SFramework
{
    /// <summary>
    /// 自动旋转
    /// </summary>
	public class AutoRotate : MonoBehaviour
    {
        public Vector3 rotate;

        private void Update()
        {
            transform.Rotate(rotate);
        }
    }
}