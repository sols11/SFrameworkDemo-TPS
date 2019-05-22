using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using ProjectScript;

namespace ProjectScript.Network
{
    public class PositionSync : MonoBehaviour
    {
        // Player预设
        public GameObject prefab;
        // 上一次移动时间
        public float lastMoveTime;
        // 玩家列表
        private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
        // ID是自己的IP和端口
        private string playerId;

        public PositionSync(string id)
        {
            playerId = id;
            AddPlayer(id, prefab.transform.position);
        }

        private void Start()
        {

        }

        // 响应函数
        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        private void AddPlayer(string id, Vector3 pos)
        {
            GameObject player = (GameObject)Instantiate(prefab, pos, Quaternion.identity);
            // player下有一个TextMesh来显示ID
            TextMesh textMesh = player.GetComponentInChildren<TextMesh>();
            textMesh.text = id;
            players.Add(id, player);
        }

        /// <summary>
        /// 删除玩家
        /// </summary>
        /// <param name="id"></param>
        private void DelPlayer(string id)
        {
            // 已经初始化该玩家
            if (players.ContainsKey(id))
            {
                Destroy(players[id]);
                players.Remove(id);
            }
        }

        /// <summary>
        /// 更新玩家信息（被动）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="score"></param>
        public void UpdateInfo(string id, Vector3 pos)
        {
            // 自身
            if (id == playerId)
            {
                return;
            }
            // 其他人
            // 已经初始化该玩家
            if (players.ContainsKey(id))
            {
                players[id].transform.position = pos;
            }
            // 尚未初始化该玩家
            else
            {
                AddPlayer(id, pos);
            }
        }

        /// <summary>
        /// 接受服务端广播的 PlayerLeave 协议，通知客户端删除该角色
        /// </summary>
        /// <param name="protocol"></param>
        public void PlayerLeave(Protocol protocol)
        {
            string id = protocol.bodyStr;
            DelPlayer(id);
        }

        /// <summary>
        /// 更新所有对象位置
        /// </summary>
        /// <param name="protocol"></param>
        public void GetList(Protocol protocol)
        {
            // 未完成
            int end = 0;
            byte[] b = System.Text.Encoding.UTF8.GetBytes(protocol.bodyStr.ToCharArray(), 0, 4);
            int count = Protocol.GetInt(b, 0, ref end);
            // 遍历
            for (int i = 0; i < count; i++)
            {
                string id = "";
                float x;
                float y;
                float z;
                Vector3 pos = new Vector3(0, 0, 0);
                UpdateInfo(id, pos);
            }
        }

        // 移动
        private void Move()
        {
            if (playerId == "")
                return;

            GameObject player = players[playerId];
            // 上
            if (Input.GetKey(KeyCode.UpArrow))
            {
                player.transform.position += new Vector3(0, 0, 1);
                SendPos();
            }
            // 下
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                player.transform.position += new Vector3(0, 0, -1); ;
                SendPos();
            }
            // 左
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                player.transform.position += new Vector3(-1, 0, 0);
                SendPos();
            }
            // 右
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                player.transform.position += new Vector3(1, 0, 0);
                SendPos();
            }
        }

        // 离开
        void OnDestory()
        {
            SendLeave();
        }

        private void Update()
        {
            // 移动
            Move();
        }

        // 发送函数
        /// <summary>
        /// 发送自身位置（主动）
        /// </summary>
        private void SendPos()
        {
            GameObject player = players[playerId];
            Vector3 pos = player.transform.position;
            // 组装协议
            Dictionary<string, Vector3> info = new Dictionary<string, Vector3>();
            info[playerId] = pos;
            string jsonStr = LitJson.JsonMapper.ToJson(info);
            NetMgr.srvConn.Send("UpdateInfo", jsonStr);
        }

        /// <summary>
        /// 发送离开协议
        /// </summary>
        private void SendLeave()
        {
            NetMgr.srvConn.Send("PlayerLeave", playerId);
        }
    }
}
