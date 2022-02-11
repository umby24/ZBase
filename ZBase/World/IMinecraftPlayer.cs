using System;
using System.Collections.Generic;
using System.Text;

namespace ZBase.World {
    interface IMinecraftPlayer {
        int Id { get; }
        int Rank { get; }
        int CustomBlockLevel { get; }
        int Ping { get; }
        string LoginName { get; }
        bool Stopped { get; }

        void SendChat(string message);
        void Kick(string reason, bool hide = false);
        void SendDefineBlock();
        void SendDeleteBlock(byte blockId);
        void ChangeMap(HcMap map);
    }
}
