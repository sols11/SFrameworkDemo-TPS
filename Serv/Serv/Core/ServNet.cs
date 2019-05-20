using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Network
{
    // ����
    public class ServNet : Singleton<ServNet>
    {
        // ����Ƕ����
        public Socket listenfd;
        // �ͻ�������
        public Conn[] conns;
        // ���������
        public int maxConn = 50;
        // ����ʱ��
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        // ����ʱ��
        public long heartBeatTime = 180;
        // Э��
        public ProtocolBase proto;
        // ��Ϣ�ַ�
        public HandleConnMsg handleConnMsg = new HandleConnMsg();
        public HandlePlayerMsg handlePlayerMsg = new HandlePlayerMsg();
        public HandlePlayerEvent handlePlayerEvent = new HandlePlayerEvent();

        // ��ȡ���ӳ����������ظ�����ʾ��ȡʧ��
        public int NewIndex()
        {
            if (conns == null)
                return -1;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                {
                    conns[i] = new Conn();
                    return i;
                }
                else if (conns[i].isUse == false)
                {
                    return i;
                }
            }
            return -1;
        }

        // ����������
        public void Start(string host, int port)
        {
            // ��ʱ��
            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
            timer.AutoReset = false;
            timer.Enabled = true;
            // ���ӳ�
            conns = new Conn[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                conns[i] = new Conn();
            }
            // Socket
            listenfd = new Socket(AddressFamily.InterNetwork,
                                  SocketType.Stream, ProtocolType.Tcp);
            // Bind
            IPAddress ipAdr = IPAddress.Parse(host);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
            listenfd.Bind(ipEp);
            // Listen
            listenfd.Listen(maxConn);
            // Accept
            listenfd.BeginAccept(AcceptCb, null);
            Console.WriteLine("[������]�����ɹ�");
        }

        // Accept�ص�
        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket socket = listenfd.EndAccept(ar);
                int index = NewIndex();

                if (index < 0)
                {
                    socket.Close();
                    Console.Write("[����]��������");
                }
                else
                {
                    Conn conn = conns[index];
                    conn.Init(socket);
                    string adr = conn.GetAddress();
                    Console.WriteLine("�ͻ������� [" + adr + "] conn��ID��" + index);
                    conn.socket.BeginReceive(conn.readBuff,
                                             conn.buffCount, conn.BuffRemain(),
                                             SocketFlags.None, ReceiveCb, conn);
                }
                listenfd.BeginAccept(AcceptCb, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("AcceptCbʧ��:" + e.Message);
            }
        }

        // �ر�
        public void Close()
        {
            for (int i = 0; i < conns.Length; i++)
            {
                Conn conn = conns[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;
                lock (conn)
                {
                    conn.Close();
                }
            }
        }

        private void ReceiveCb(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            lock (conn)
            {
                try
                {
                    int count = conn.socket.EndReceive(ar);
                    // �ر��ź�
                    if (count <= 0)
                    {
                        Console.WriteLine("�յ� [" + conn.GetAddress() + "] �Ͽ�����");
                        conn.Close();
                        return;
                    }
                    conn.buffCount += count;
                    ProcessData(conn);
                    // ��������	
                    conn.socket.BeginReceive(conn.readBuff,
                                             conn.buffCount, conn.BuffRemain(),
                                             SocketFlags.None, ReceiveCb, conn);
                }
                catch (Exception e)
                {
                    Console.WriteLine("�յ� [" + conn.GetAddress() + "] �Ͽ�����");
                    conn.Close();
                }
            }
        }

        private void ProcessData(Conn conn)
        {
            // С�ڳ����ֽ�
            if (conn.buffCount < sizeof(Int32))
            {
                return;
            }
            // ��Ϣ���ȣ�ճ���ְ���
            Array.Copy(conn.readBuff, conn.lenBytes, sizeof(Int32));
            conn.msgLength = BitConverter.ToInt32(conn.lenBytes, 0);
            if (conn.buffCount < conn.msgLength + sizeof(Int32))
            {
                return;
            }
            // ������Ϣ
            ProtocolBase protocol = proto.Decode(conn.readBuff, sizeof(Int32), conn.msgLength);
            HandleMsg(conn, protocol);
            // ����Ѵ������Ϣ
            int count = conn.buffCount - conn.msgLength - sizeof(Int32);
            Array.Copy(conn.readBuff, sizeof(Int32) + conn.msgLength, conn.readBuff, 0, count);
            conn.buffCount = count;
            if (conn.buffCount > 0)
            {
                ProcessData(conn);
            }
        }

        // ��Ϣ�ַ���ʹ�÷��䣩������protocal����
        private void HandleMsg(Conn conn, ProtocolBase protoBase)
        {
            string name = protoBase.GetName();
            string methodName = "Msg" + name;
            // ����Э��ַ�
            if (conn.player == null || name == "HeatBeat" || name == "Logout")
            {
                MethodInfo mm = handleConnMsg.GetType().GetMethod(methodName);
                if (mm == null)
                {
                    string str = "[����]HandleMsgû�д������ӷ��� ";
                    Console.WriteLine(str + methodName);
                    return;
                }
                Object[] obj = new object[] { conn, protoBase };
                Console.WriteLine("[����������Ϣ]" + conn.GetAddress() + " :" + name);
                mm.Invoke(handleConnMsg, obj);
            }
            // ��ɫЭ��ַ�
            else
            {
                MethodInfo mm = handlePlayerMsg.GetType().GetMethod(methodName);
                if (mm == null)
                {
                    string str = "[����]HandleMsgû�д�����ҷ��� ";
                    Console.WriteLine(str + methodName);
                    return;
                }
                Object[] obj = new object[] { conn.player, protoBase };
                Console.WriteLine("[���������Ϣ]" + conn.player.id + " :" + name);
                mm.Invoke(handlePlayerMsg, obj);    // ����
            }
        }

        // ����
        public void Send(Conn conn, ProtocolBase protocol)
        {
            byte[] bytes = protocol.Encode();
            byte[] length = BitConverter.GetBytes(bytes.Length);
            byte[] sendbuff = length.Concat(bytes).ToArray();
            try
            {
                conn.socket.BeginSend(sendbuff, 0, sendbuff.Length, SocketFlags.None, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("[������Ϣ]" + conn.GetAddress() + " : " + e.Message);
            }
        }

        // �㲥
        public void Broadcast(ProtocolBase protocol)
        {
            for (int i = 0; i < conns.Length; i++)
            {
                if (!conns[i].isUse)
                    continue;
                if (conns[i].player == null)
                    continue;
                Send(conns[i], protocol);
            }
        }

        // ����ʱ��
        public void HandleMainTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            // ��������
            HeartBeat();
            timer.Start();
        }

        // ����
        public void HeartBeat()
        {
            //Console.WriteLine ("[����ʱ��ִ��]");
            long timeNow = Sys.GetTimeStamp();

            for (int i = 0; i < conns.Length; i++)
            {
                Conn conn = conns[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;
                // ��������ʱ�����δ����tickTime����δ���ܵ�����������Ͽ�����
                if (conn.lastTickTime < timeNow - heartBeatTime)
                {
                    Console.WriteLine("[��������Ͽ�����]" + conn.GetAddress());
                    lock (conn)
                        conn.Close();
                }
            }
        }

        // ��ӡ��Ϣ
        public void Print()
        {
            Console.WriteLine("===��������¼��Ϣ===");
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                    continue;
                if (!conns[i].isUse)
                    continue;

                string str = "����[" + conns[i].GetAddress() + "] ";
                if (conns[i].player != null)
                    str += "���id " + conns[i].player.id;

                Console.WriteLine(str);
            }
        }
    }
}