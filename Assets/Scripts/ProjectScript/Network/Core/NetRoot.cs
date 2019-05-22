using UnityEngine;
using System.Collections;

namespace ProjectScript.Network
{
    /// <summary>
    /// MonoBehaviour的Root类，用于开启网络服务
    /// </summary>
    public class NetRoot : MonoBehaviour
    {
        void Start()
        {
            Application.runInBackground = true;
        }

        void Update()
        {
            NetMgr.Update();
        }
    }

}
