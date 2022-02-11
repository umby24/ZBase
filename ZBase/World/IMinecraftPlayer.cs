using System;
using System.Collections.Generic;
using System.Text;

namespace ZBase.World {
    public interface IMinecraftPlayer {
        int Id { get; }
        int Rank { get; }
        int CustomBlockLevel { get; }
        int Ping { get; }
        string LoginName { get; }
        bool Stopped { get; }
        Entity Entity { get; set; }
        void Login();
        void Logout();
        void SendChat(string message);
        void Kick(string reason, bool hide = false);
        void SendDefineBlock();
        void SendDeleteBlock(byte blockId);
        void ChangeMap(HcMap map);

        void HandleChatReceived(string message);
    }
}
