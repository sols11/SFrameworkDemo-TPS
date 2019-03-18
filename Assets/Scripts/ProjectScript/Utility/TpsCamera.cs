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
        #region Animation Variables
        private readonly int _upPeek = Animator.StringToHash("UpPeek");
        private readonly int _edgePeek = Animator.StringToHash("EdgePeek");
        private readonly int _coverLocomotion = Animator.StringToHash("CoverLocomotion");
        private readonly int _coverEnter = Animator.StringToHash("CoverEnter");
        private readonly int _locomotionState = Animator.StringToHash("Locomotion");
        private readonly int _crouchlocomotionState = Animator.StringToHash("CrouchLocomotion");
        #endregion

    }
}
