using System;

namespace Network
{
    public class HandlePlayerEvent
    {
        // 上线
        public void OnLogin(Player player)
        {
            Scene.Instance.AddPlayer(player.id);
        }
        // 下线
        public void OnLogout(Player player)
        {
            Scene.Instance.DelPlayer(player.id);
        }
    }
}