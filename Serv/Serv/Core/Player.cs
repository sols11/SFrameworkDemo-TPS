using System;

namespace Network
{
    public class Player
    {
        public string id;
        // ������
        public Conn conn;
        // ����
        public PlayerData data;
        // ��ʱ����
        public PlayerTempData tempData;

        public Player(string id, Conn conn)
        {
            this.id = id;
            this.conn = conn;
            tempData = new PlayerTempData();
        }

        // ѡ��Э�鷢����Ϣ����װ����ServNet.Send
        public void Send(ProtocolBase proto)
        {
            if (conn == null)
                return;
            ServNet.Instance.Send(conn, proto);
        }

        // �����ߣ����id������ʲô��Ϣ��
        public static bool KickOff(string id, ProtocolBase proto)
        {
            Conn[] conns = ServNet.Instance.conns;
            if (conns == null)  // 
                return true;
            // �������ӳ�
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
                    // �÷��������ӵ���Ϣ������ͬһ�̣߳���Ҫ�����̰߳�ȫ
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

        // ����
        public bool Logout()
        {
            // �¼��ַ�
            ServNet.Instance.handlePlayerEvent.OnLogout(this);
            // ����
            if (!DataMgr.Instance.SavePlayer(this))
                return false;
            conn.player = null;
            conn.Close();
            return true;
        }
    }
}
