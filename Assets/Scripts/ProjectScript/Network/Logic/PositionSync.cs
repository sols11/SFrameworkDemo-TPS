using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using ProjectScript;

namespace ProjectScript.Network
{
    [Serializable]
    public struct PlayerInfo
    {
        // 由于litJson不支持float所以用double
        public string Id   { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double PosZ { get; set; }

        public PlayerInfo(string playerId, Vector3 pos)
        {
            Id = playerId;
            PosX = pos.x;
            PosY = pos.y;
            PosZ = pos.z;
        }
    }

    /// <summary>
    /// 处理Player的消息。该类需要和PlayerMgr，EnemyMgr等协作。
    /// </summary>
    public class PositionSync
    {
        // Player预设
        //public GameObject prefab;
        // 上一次移动时间
        public float lastMoveTime;
        // 玩家列表
        private static Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
        // ID是自己的IP和端口
        private static string playerId;

        // 响应函数
        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        public static void AddPlayer(string id, GameObject player, Vector3 pos)
        {
            playerId = id;
            // player下有一个TextMesh来显示ID
            //TextMesh textMesh = player.GetComponentInChildren<TextMesh>();
            //textMesh.text = id;
            if (!players.ContainsKey(id))
                players.Add(id, player);
        }

        /// <summary>
        /// 删除玩家
        /// </summary>
        /// <param name="id"></param>
        public static void DelPlayer(string id)
        {
            // 已经初始化该玩家
            if (players.ContainsKey(id))
            {
                players.Remove(id);
            }
        }

        /// <summary>
        /// 更新玩家信息（被动）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="score"></param>
        public static void UpdateInfo(string id, Vector3 pos)
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
                //AddPlayer(id, pos);
            }
        }

        /// <summary>
        /// 接受服务端广播的 PlayerLeave 协议，通知客户端删除该角色
        /// </summary>
        /// <param name="protocol"></param>
        public static void PlayerLeave(Protocol protocol)
        {
            string id = protocol.bodyStr;
            DelPlayer(id);
        }

        /// <summary>
        /// 更新所有对象位置
        /// </summary>
        /// <param name="protocol"></param>
        public static void GetList(Protocol protocol)
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

        // 发送函数
        /// <summary>
        /// 发送自身位置（主动）
        /// </summary>
        public static void SendPos()
        {
            GameObject player = players[playerId];
            PlayerInfo info = new PlayerInfo(playerId, player.transform.position);
            // 组装协议
            string jsonStr = LitJson.JsonMapper.ToJson(info);
            Debug.Log(jsonStr);
            NetMgr.srvConn.Send("UpdateInfo", jsonStr);
        }

        /// <summary>
        /// 发送离开协议
        /// </summary>
        public static void SendLeave()
        {
            NetMgr.srvConn.Send("PlayerLeave", playerId);
        }

        /// <summary>
        /// 暂时没用上的转换函数
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static List<float> Vector3ToList(Vector3 pos)
        {
            List<float> list = new List<float>();
            list.Add(pos.x);
            list.Add(pos.y);
            list.Add(pos.z);
            return list;
        }
    }
}
