using ZBase.Common;

namespace ZBase.Network {
    public interface INetworkClient {
        bool HasData();
        /// <summary>
        /// Prompt the client to send any queued packets
        /// </summary>
        void SendQueued();

        /// <summary>
        /// Prompt this client to handle any packets it may have received.
        /// </summary>
        void Handle();

        /// <summary>
        /// Friendly disconnect
        /// </summary>
        /// <param name="reason">Reason for the disconnection.</param>
        void Kick(string reason); // -- Friendly Disconnect

        /// <summary>
        /// Unfriendly disconnection. Used to forcibly and immediately disconnect a client.
        /// </summary>
        void Shutdown(); // -- Unfriendly disconnect

        void SetName(string name);
        void SendPacket(IIndevPacket packet);
    }
}
