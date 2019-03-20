/*----------------------------------------------------------------------------
Author:
    Anotts
Date:
    2017/08/01
Description:
    ��飺
    ���ã�
    ʹ�ã�
    ���䣺
History:
----------------------------------------------------------------------------*/

using System;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// ���ڵļ�ʵ�֣�camera����������λ�õ��е㣬�������offsetZ
    /// �������ʱ��2����PosZΪmainCamera��ǰ����ֵ��-8.3)
    /// Ȼ����������ӣ�PosZ+=0.5*distance
    /// ��Ϊ���offsetY����Ȼ��ҪoffsetRotX��������offsetZ�����þͲ����ˣ�������ת��������ͷ��
    /// ����ĸ�����Ч�����������Կ��Ǽ���distanceMinMax���ƣ���offsetXY
    /// ����������ʱ����Player��Enemy������Գƣ�LevelCamera����Ϊ��ʱ��ƫ�Ƶ�,��������Ϊԭ�������յ��������塿
    /// 
    /// ���ǵ���Ļ��16:9�ı���
    /// ʵ�ʾ����ܹ��ĵ��ķ�Χ������Ļ����Ϊԭ�㣬distanceΪֱ����Բ
    /// ���������ʱ������л�����һ���Ƕȣ��Ա�֤���ĵ�������
    /// ��þ�����Բ�γ�������Ҫ�÷��ģ����䳤����
    /// 
    /// �ڲ�ͬ�ĳ����������ò�ͬ��ϵ��
    /// �������ڳ����аڷź�boss��playerʹ���������������ǵ���Сdistance
    /// Ȼ���޸������λ�ã�Ϊ���������distanceʱ����ѹ۲�λ�ã��豣֤���������е�Ϊԭ�㣩
    /// Ȼ��������distance����ٴ�������ѹ۲�λ��
    /// �������λ�õĲ�ֵ/distance֮�� �͵õ�����Ҫ��ϵ��
    /// ��ȻҲ��������distanceС��һ��ֵʱ�����offset����
    /// </summary>
    //[ExecuteInEditMode]
    public class AutoCam : MonoBehaviour
    {
        public bool EnableAreaLimit { get; set; }
        [SerializeField]
        private float moveSpeed = 10;
        [SerializeField]
        private float rotSpeed = 3;
        [SerializeField]
        private bool autoTarget = true;        // Whether the rig should automatically target the player.
        [SerializeField]
        private bool targetEnemy = true;
        [SerializeField]
        private UpdateType updateType = UpdateType.FixedUpdate;         // stores the selected update type
        // ׷��
        private float xMaxLimit;
        private float xMinLimit;
        private float zMaxLimit;
        private float zMinLimit;
        private Vector3 clampPos = Vector3.zero;
        // �ڵ���͸
        public TransparentCam transparentCam;

        private string playerTag = "Player";
        private string enemyTag = "Enemy";
        public Transform playerTransform;
        public Transform enemyTransform;
        public float distance = 0;
        public float distanceMin = 12;

        // ���·ֱ���PosY,PosZ��RotX��Ĭ��ϵ��
        //public float offsetY = 0.4f;
        public float offsetZ = -0.5f;
        //public float offsetX = 0.9f;

        public enum UpdateType // The available methods of updating are:
        {
            FixedUpdate, // Update in FixedUpdate (for tracking rigidbodies).
            LateUpdate, // Update in LateUpdate. (for tracking objects that are moved in Update)
            ManualUpdate, // user must call to update camera
        }

        /// <summary>
        /// ����Camera�ƶ���Χ
        /// </summary>
        /// <param name="xMax"></param>
        /// <param name="xMin"></param>
        /// <param name="zMax"></param>
        /// <param name="zMin"></param>
        public void SetAreaLimit(float xMax, float xMin, float zMax, float zMin)
        {
            EnableAreaLimit = true;
            xMaxLimit = xMax;
            xMinLimit = xMin;
            zMaxLimit = zMax;
            zMinLimit = zMin;
        }

        private void Start()
        {
            if (autoTarget)
            {
                FindAndTargetPlayer();
            }
            if (targetEnemy)
            {
                FindAndTargetEnemy();
            }
            if (playerTransform == null)
                return;
        }

        private void FindAndTargetPlayer()
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag(playerTag);
            if (targetObj)
            {
                playerTransform = targetObj.transform;
                if (transparentCam != null)
                    transparentCam.targetObject = playerTransform;
            }
        }

        private void FindAndTargetEnemy()
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag(enemyTag);
            if (targetObj)
            {
                enemyTransform = targetObj.transform;
            }
        }

        private void FollowTarget(float deltaTime)
        {
            // �˳��ж�
            if (deltaTime <= 0 || playerTransform == null)
                return;
            // TargetEnemy
            if(targetEnemy&& enemyTransform!=null && playerTransform.gameObject.activeSelf)
            {
                distance = Vector3.Distance(playerTransform.position, enemyTransform.position);
                // ͨ��ϵ������offset
                Vector3 aimPos = (playerTransform.position + enemyTransform.position) / 2 + new Vector3(0, 0, offsetZ * distance);
                //Quaternion q = Quaternion.identity;
                //if (distance > distanceMin)
                //{

                //    aimPos += new Vector3(0, 1.6f, -4);
                //    q = Quaternion.Euler(2.5f, 0, 0);
                //}
                transform.position = Vector3.Lerp(transform.position, aimPos , deltaTime * moveSpeed);
                // ͨ��ϵ������RotX
                //transform.rotation = Quaternion.Slerp(transform.rotation, q, deltaTime * rotSpeed);
            }
            // TargetPlayer
            else
            {
                transform.position = Vector3.Lerp(transform.position, playerTransform.position, deltaTime * moveSpeed);
            }
            if (EnableAreaLimit)
            {
                // ��������ƶ���Χ
                clampPos = new Vector3(Mathf.Clamp(playerTransform.position.x, xMinLimit, xMaxLimit), playerTransform.position.y,
                    Mathf.Clamp(playerTransform.position.z, zMinLimit, zMaxLimit));
                transform.position = Vector3.Lerp(transform.position, clampPos, deltaTime * moveSpeed);
            }
        }

        private void LaterUpdate()
        {
            if (autoTarget && (playerTransform == null || !playerTransform.gameObject.activeSelf))
                FindAndTargetPlayer();
            if (targetEnemy)
                FindAndTargetEnemy();
            if (updateType == UpdateType.LateUpdate)
                FollowTarget(Time.deltaTime);
        }

        private void ManualUpdate()
        {
            if (autoTarget && (playerTransform == null || !playerTransform.gameObject.activeSelf))
            {
                FindAndTargetPlayer();
            }
            if (targetEnemy)
                FindAndTargetEnemy();
            if (updateType == UpdateType.ManualUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (autoTarget && (playerTransform == null || !playerTransform.gameObject.activeSelf))
            {
                FindAndTargetPlayer();
            }
            if (targetEnemy && (enemyTransform == null || !enemyTransform.gameObject.activeSelf))
            {
                FindAndTargetEnemy();
            }
            if (updateType == UpdateType.FixedUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }
    }
}
