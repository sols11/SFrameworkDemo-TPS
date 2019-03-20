/*----------------------------------------------------------------------------
Author:
    Anotts
Date:
    2017/08/01
Description:
    简介：
    作用：
    使用：
    补充：
History:
----------------------------------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;

namespace ProjectScript
{
    public class TpsCamera : MonoBehaviour
    {
        public Transform target;
        public float distanceH = 7f;
        public float distanceV = 4f;

        void LateUpdate()
        {
            Vector3 nextpos = target.forward * -1 * distanceH + Vector3.up * distanceV + target.position;
            this.transform.position = nextpos;
            this.transform.LookAt(target);
        }

    }
}
