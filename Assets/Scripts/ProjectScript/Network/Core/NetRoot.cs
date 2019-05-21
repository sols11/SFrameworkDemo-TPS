using UnityEngine;
using System.Collections;

namespace ProjectScript.Network
{
    public class NetRoot : MonoBehaviour
    {
        void Start()
        {
            Application.runInBackground = true;
            NetMgr.ConnectServ();
        }

        void Update()
        {
            NetMgr.Update();
        }
    }

}
