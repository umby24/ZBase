using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZBase.Network {
    interface INetworkClient {
        void Kick(string reason); // -- Friendly Disconnect
        void Shutdown(); // -- Unfriendly disconnect

        // -- Network IO, Send and receive network packets.
        void ReceiveLoop();
        void SendLoop();

        // -- Process events
        void ProcessEvents();
    }
}
