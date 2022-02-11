using ZBase.Common;
using ZBase.World;

namespace ZBase.Network {
    public interface INetworkClient {
        string Ip { get; set; }
        string Name { get; set; }
        bool Verified { get; set; }
        IMinecraftPlayer ClientPlayer { get; set; }
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

        void QueueBlockChange(IIndevPacket bcPacket);

        void SendHandshake(bool op, string motd = null);
        /// <summary>
        /// Get this client's Minecraft Player Instance for use
        /// </summary>
        /// <returns></returns>
        IMinecraftPlayer GetPlayerInstance();

        void SetName(string name);

        /// <summary>
        /// Send a network packet to this client
        /// </summary>
        /// <param name="packet"></param>
        void SendPacket(IIndevPacket packet);
    }
}
