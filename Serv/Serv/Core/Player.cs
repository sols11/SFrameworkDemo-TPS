using System;

namespace Network
{
    public class Player
    {
        public string id;
        // 连接类
        public Conn conn;
        // 数据
        public PlayerData data;
        // 临时数据
        public PlayerTempData tempData;

        public Player(string id, Conn conn)
        {
            this.id = id;
            this.conn = conn;
            tempData = new PlayerTempData();
        }

        // 选择协议发送消息，封装调用ServNet.Send
        public void Send(ProtocolBase proto)
        {
            if (conn == null)
                return;
            ServNet.Instance.Send(conn, proto);
        }

        // 踢下线（玩家id，发送什么消息）
        public static bool KickOff(string id, ProtocolBase proto)
        {
            Conn[] conns = ServNet.Instance.conns;
            if (conns == null)  // 
                return true;
            // 遍历连接池
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                    continue;
                if (!conns[i].isUse)
                    continue;
                if (conns[i].player == null)
                    continue;
                if (conns[i].player.id == id)
                {
                    // 该方法与连接的消息处理不在同一线程，需要考虑线程安全
                    lock (conns[i].player)
                    {
                        if (proto != null)
                            conns[i].player.Send(proto);

                        return conns[i].player.Logout();
                    }
                }
            }
            return true;
        }

        // 下线
        public bool Logout()
        {
            // 事件分发
            ServNet.Instance.handlePlayerEvent.OnLogout(this);
            // 保存
            if (!DataMgr.Instance.SavePlayer(this))
                return false;
            conn.player = null;
            conn.Close();
            return true;
        }
    }
}
