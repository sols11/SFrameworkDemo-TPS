using System;

namespace Network
{
    public partial class HandleConnMsg
    {
        /// <summary>
        /// �������
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="protoBase"></param>
        public void MsgHeatBeat(Conn conn, ProtocolBase protoBase)
        {
            conn.lastTickTime = Sys.GetTimeStamp();
            Console.WriteLine("[��������ʱ��]" + conn.GetAddress());
        }

        /// <summary>
        /// ע�ᡣЭ�������str�û���,str���롣����Э�飺-1��ʾʧ�� 0��ʾ�ɹ�
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="protoBase"></param>
        public void MsgRegister(Conn conn, ProtocolBase protoBase)
        {
            // ��ȡ��ֵ
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            string protoName = protocol.GetString(start, ref start);
            string id = protocol.GetString(start, ref start);
            string pw = protocol.GetString(start, ref start);
            string strFormat = "[�յ�ע��Э��]" + conn.GetAddress();
            Console.WriteLine(strFormat + " �û�����" + id + " ���룺" + pw);
            // ��������Э��
            protocol = new ProtocolBytes();
            protocol.AddString("Register");
            // ע��
            if (DataMgr.Instance.Register(id, pw))
            {
                protocol.AddInt(0);
            }
            else
            {
                protocol.AddInt(-1);
            }
            // ������ɫ
            DataMgr.Instance.CreatePlayer(id);
            // ����Э����ͻ���
            conn.Send(protocol);
        }

        /// ��¼
        /// Э�������str�û���,str����
        /// ����Э�飺-1��ʾʧ�� 0��ʾ�ɹ�
        public void MsgLogin(Conn conn, ProtocolBase protoBase)
        {
            // ��ȡ��ֵ
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            string protoName = protocol.GetString(start, ref start);
            string id = protocol.GetString(start, ref start);
            string pw = protocol.GetString(start, ref start);
            string strFormat = "[�յ���¼Э��]" + conn.GetAddress();
            Console.WriteLine(strFormat + " �û�����" + id + " ���룺" + pw);
            // ��������Э��
            ProtocolBytes protocolRet = new ProtocolBytes();
            protocolRet.AddString("Login");
            // ��֤
            if (!DataMgr.Instance.CheckPassWord(id, pw))
            {
                protocolRet.AddInt(-1);
                conn.Send(protocolRet);
                return;
            }
            // �Ƿ��Ѿ���¼
            ProtocolBytes protocolLogout = new ProtocolBytes();
            protocolLogout.AddString("Logout");
            if (!Player.KickOff(id, protocolLogout))
            {
                protocolRet.AddInt(-1);
                conn.Send(protocolRet);
                return;
            }
            // ��ȡ�������
            PlayerData playerData = DataMgr.Instance.GetPlayerData(id);
            if (playerData == null)
            {
                protocolRet.AddInt(-1);
                conn.Send(protocolRet);
                return;
            }
            conn.player = new Player(id, conn);
            conn.player.data = playerData;
            // �¼�����
            ServNet.Instance.handlePlayerEvent.OnLogin(conn.player);
            // ����
            protocolRet.AddInt(0);
            conn.Send(protocolRet);
            return;
        }

        /// ����
        /// Э�������
        /// ����Э�飺0-��������
        public void MsgLogout(Conn conn, ProtocolBase protoBase)
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("Logout");
            protocol.AddInt(0);
            if (conn.player == null)
            {
                conn.Send(protocol);
                conn.Close();
            }
            else
            {
                conn.Send(protocol);
                conn.player.Logout();
            }
        }
    }
}