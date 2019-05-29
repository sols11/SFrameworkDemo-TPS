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
        public enum PropType
        {
            Medicine,
            BulletClip,
            Gun,
        }
        public PropType type = PropType.Medicine;

        private void Update()
        {
            transform.Rotate(rotate);
        }

        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == (int)ObjectLayer.Player)
            {
                IPlayerMono playerMono = col.transform.GetComponent<IPlayerMono>();
                if (type == PropType.Medicine)
                {
                    playerMono.PlayerMedi.Player.CurrentHP += 50;
                    GameMainProgram.Instance.eventMgr.InvokeEvent(EventName.PlayerHP_SP);
                }
                else if(type == PropType.BulletClip)
                {
                    ProjectScript.WeaponHandgun weaponHandgun = playerMono.iPlayerWeapon as ProjectScript.WeaponHandgun;
                    weaponHandgun.RemainingBulletCount += 30;
                    GameMainProgram.Instance.eventMgr.InvokeEvent(EventName.BulletCount);
                }
                else
                {
                    IWeaponMono weapon = playerMono.iPlayerWeapon;
                    weapon.GetComponent<MeshFilter>().mesh = transform.GetComponent<MeshFilter>().mesh;
                }

                transform.parent.GetComponent<AudioSource>().Play();
                gameObject.SetActive(false);
                Destroy(transform.parent.gameObject, 3);
            }
        }
    }
}