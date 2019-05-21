using UnityEngine;
using System;
using System.Collections.Generic;

namespace ProjectScript.Network
{
    /// <summary>
    /// 由消息分发系统处理消息。处理的是body中name后面的数据，通常是Json字符串。
    /// 事件需要自行添加和移除
    /// </summary>
    public class MsgDistribution
    {
        // 每帧处理消息的数量
        public int num = 15;
        // 消息列表
        public List<Protocol> msgList = new List<Protocol>();
        // 委托类型
        public delegate void Delegate(Protocol proto);
        // 事件监听表
        private Dictionary<string, Delegate> eventDict = new Dictionary<string, Delegate>();
        private Dictionary<string, Delegate> onceDict = new Dictionary<string, Delegate>();

        // Update. Called by Connection
        public void Update()
        {
            for (int i = 0; i < num; i++)
            {
                if (msgList.Count > 0)
                {
                    DispatchMsgEvent(msgList[0]);
                    lock (msgList)
                        msgList.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
        }

        // 消息分发
        public void DispatchMsgEvent(Protocol protocol)
        {
            string name = protocol.name;
            Debug.Log("[收到事件消息] " + name);
            if (eventDict.ContainsKey(name))
            {
                eventDict[name](protocol);
            }
            if (onceDict.ContainsKey(name))
            {
                onceDict[name](protocol);
                onceDict[name] = null;
                onceDict.Remove(name);
            }
        }

        // 添加事件监听 
        public void AddListener(string name, Delegate cb)
        {
            if (eventDict.ContainsKey(name))
                eventDict[name] += cb;
            else
                eventDict[name] = cb;
        }

        // 添加单次监听事件
        public void AddOnceListener(string name, Delegate cb)
        {
            if (onceDict.ContainsKey(name))
                onceDict[name] += cb;
            else
                onceDict[name] = cb;
        }

        // 删除监听事件
        public void DelListener(string name, Delegate cb)
        {
            if (eventDict.ContainsKey(name))
            {
                eventDict[name] -= cb;
                if (eventDict[name] == null)
                    eventDict.Remove(name);
            }
        }

        // 删除单次监听事件
        public void DelOnceListener(string name, Delegate cb)
        {
            if (onceDict.ContainsKey(name))
            {
                onceDict[name] -= cb;
                if (onceDict[name] == null)
                    onceDict.Remove(name);
            }
        }
    }
}