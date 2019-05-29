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

namespace SFramework
{
    /// <summary>
    /// 随机生成器
    /// </summary>
	public class RandomSpawner : MonoBehaviour
    {
        public Transform[] trans;   // 设置左上，右下两个角
        public GameObject[] prefabs;
        public GameObject[] guns;
        public int spawnCount = 5;

        private void Start()
        {
            for (int i = 0; i < spawnCount; ++i)
            {
                RandomSpawn();
                RandomSpawnGun();
            }
        }

        private void RandomSpawn()
        {
            float x = Random.Range(trans[0].position.x, trans[1].position.x);
            float z = Random.Range(trans[1].position.z, trans[0].position.z);
            int i = Random.Range(0, prefabs.Length);
            Instantiate(prefabs[i], new Vector3(x, 0.3f, z), Quaternion.Euler(-90, 0, 0));
        }

        private void RandomSpawnGun()
        {
            float x = Random.Range(trans[0].position.x, trans[1].position.x);
            float z = Random.Range(trans[1].position.z, trans[0].position.z);
            int i = Random.Range(0, guns.Length);
            Instantiate(guns[i], new Vector3(x, 0.5f, z), Quaternion.identity);
        }
    }
}
