namespace ZBase.Network {
    interface INetworkClient {
        void Kick(string reason); // -- Friendly Disconnect
        void Shutdown(); // -- Unfriendly disconnect
    }
}
