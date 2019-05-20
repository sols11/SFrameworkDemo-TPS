using System;

namespace Network
{
    public partial class HandlePlayerMsg
    {
        /// <summary>
        /// ��ȡ����������Э�飺int����
        /// </summary>
        /// <param name="player"></param>
        /// <param name="protoBase"></param>
        public void MsgGetScore(Player player, ProtocolBase protoBase)
        {
            ProtocolBytes protocolRet = new ProtocolBytes();
            protocolRet.AddString("GetScore");      // Name
            protocolRet.AddInt(player.data.score);  // Int
            player.Send(protocolRet);
            Console.WriteLine("MsgGetScore " + player.id + player.data.score);
        }

        /// <summary>
        /// ���ӷ���
        /// </summary>
        /// <param name="player"></param>
        /// <param name="protoBase"></param>
        public void MsgAddScore(Player player, ProtocolBase protoBase)
        {
            // ��ȡ��ֵ
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            string protoName = protocol.GetString(start, ref start);
            // Ȼ������
            player.data.score += 1;
            Console.WriteLine("MsgAddScore " + player.id + " " + player.data.score.ToString());
        }

        // ��ȡ����б�
        public void MsgGetList(Player player, ProtocolBase protoBase)
        {
            Scene.Instance.SendPlayerList(player);
        }

        // ������Ϣ������Ϣ������װ���㲥��
        public void MsgUpdateInfo(Player player, ProtocolBase protoBase)
        {
            // ��ȡ��ֵ
            int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            string protoName = protocol.GetString(start, ref start);
            float x = protocol.GetFloat(start, ref start);
            float y = protocol.GetFloat(start, ref start);
            float z = protocol.GetFloat(start, ref start);
            int score = player.data.score;
            Scene.Instance.UpdateInfo(player.id, x, y, z, score);
            // �㲥
            ProtocolBytes protocolRet = new ProtocolBytes();
            protocolRet.AddString("UpdateInfo");
            protocolRet.AddString(player.id);
            protocolRet.AddFloat(x);
            protocolRet.AddFloat(y);
            protocolRet.AddFloat(z);
            protocolRet.AddInt(score);
            ServNet.Instance.Broadcast(protocolRet);
        }
    }
}