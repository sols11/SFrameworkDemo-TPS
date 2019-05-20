using System;

namespace Network
{
    public class HandlePlayerEvent
    {
        // ����
        public void OnLogin(Player player)
        {
            Scene.Instance.AddPlayer(player.id);
        }
        // ����
        public void OnLogout(Player player)
        {
            Scene.Instance.DelPlayer(player.id);
        }
    }
}