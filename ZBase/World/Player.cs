using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZBase.Build;
using ZBase.Common;
using ZBase.Network;
using ZBase.Persistence;

namespace ZBase.World {
    public partial class ClassicubePlayer : IMinecraftPlayer {
        public static readonly PlayerDb Database = new PlayerDb();
        public string Name { get; set; }
        public Rank CurrentRank { get; set; }
        public DateTime MutedUntil { get; set; }
        public BuildState CurrentState { get; set; }
        public string ChatBuffer { get; set; }
        // -- Interface items
        public int Id => throw new NotImplementedException();

        public int Rank => CurrentRank.Value;

        public int CustomBlockLevel => 0;

        public int Ping => 0;

        public string LoginName => Name;

        bool IMinecraftPlayer.Stopped => Stopped;

        // -- /Interface items

        public Entity Entity { get; set; }

        private readonly Client _client;
        public Block Material;
        public Block LastMaterial;
        public bool Stopped;
        private bool _banned;

        public ClassicubePlayer(Client client) {
            _client = client;
            CurrentState = new BuildState();
            ChatBuffer = "";
        }

        public void Login() {
            LoadDbInfo();

            if (_banned) {
                _client.Kick(Constants.DefaultBanMessage);
                return;
            }

            var loginMap = HcMap.DefaultMap;

            Entity = new Entity { // -- Create our entity
                Name = Name,
                Location = loginMap.GetSpawn(),
                CurrentMap = loginMap,
                PrettyName = CurrentRank.Prefix + Name + CurrentRank.Suffix
            };

            Entity.OnEntityDespawned += DespawnEntity;
            Entity.OnEntitySpawned += SpawnEntity;
            Entity.OtherEntityMoved += SomeoneMoved;

            _client.SendHandshake(CurrentRank.ClientOp); // -- Send the handshake (acknowledgement)

            MapSend(loginMap.GetChunks()); // -- Send the map to the client.

            loginMap.BlockChanged += MapBlockChange; // -- Subscribe to block change events
            loginMap.MapChatSent += HandleChatReceived; // -- sub to map chat.

            _client.Verified = true; // -- Register the client, announce their arrival, and set them as verified (Can perform actions)

            Entity.Spawn(); // -- Spawn this client for everyone (including themselves)

            var entities = Entity.CurrentMap.Entities;
            Parallel.ForEach(entities, SpawnEntity);

            Entity.HandleMove(); // -- make sure initial position is set.

            // -- Register this person for global chat messages.
            Chat.GlobalChatSent += HandleChatReceived;

            Chat.SendGlobalChat($"{Entity.PrettyName}§S logged in.", 0, true);
        }

        public void Logout() {
            Entity.Despawn();

            Chat.SendGlobalChat($"§S{Name} logged out.", 0, true);
            Entity.CurrentMap.BlockChanged -= MapBlockChange;
            Entity.CurrentMap.MapChatSent -= HandleChatReceived;
            Chat.GlobalChatSent -= HandleChatReceived;
        }

        private void SendSplitChat(IEnumerable<string> messages) {
            foreach (var msg in messages) {
                var final = msg;
                if (Chat.FirstIsEmote(final[0])) {
                    final = "." + final;
                }

                _client.SendPacket(PacketCreator.CreateChat(final));
            }
        }

        private void MapSend(IReadOnlyList<byte[]> data) {
            _client.SendPacket(new LevelInit());

            for (var i = 0; i < data.Count; i++) {
                byte[] a = data[i];
                _client.SendPacket(PacketCreator.CreateMapChunk(a, (byte)((i / data.Count) * 100), a.Length));
            }

            _client.SendPacket(PacketCreator.CreateMapFinal(Entity.CurrentMap.GetSize()));
        }

        /// <summary>
        /// Changes the map this player is on to another map
        /// </summary>
        /// <param name="map">The map to send this player to.</param>
        public void ChangeMap(HcMap map) {
            // -- Announce
            Chat.SendGlobalChat($"§SPlayer {Entity.PrettyName}§S changed to map '{map.MapProvider.MapName}'", 0); // -- TODO: Move this into an event handler.
            DespawnEntities();

            // -- Cleanup events
            Entity.CurrentMap.BlockChanged -= MapBlockChange;
            Entity.CurrentMap.MapChatSent -= HandleChatReceived;

            // -- Send the new map
            Entity.CurrentMap = map;

            _client.SendHandshake(CurrentRank.ClientOp);
            MapSend(Entity.CurrentMap.GetChunks());
            Entity.CurrentMap.BlockChanged += MapBlockChange; // -- sub to block changes
            Entity.CurrentMap.MapChatSent += HandleChatReceived;

            // -- setup our entity
            Entity.Location = Entity.CurrentMap.GetSpawn();

            var entities = Entity.CurrentMap.Entities;
            Parallel.ForEach(entities, SpawnEntity);

            Entity.HandleMove(); // -- make sure initial position is set.
        }

        private void DespawnEntities() {
            // -- Remove all entities from this client
            var entities = Entity.CurrentMap.Entities;

            foreach (Entity entity in entities) {
                DespawnEntity(entity);
            }
        }

        protected internal void LoadDbInfo() {
            if (!Database.ContainsPlayer(Name)) {
                var newPlayer = new PlayerModel {
                    Name = Name,
                    Ip = _client.Ip,
                    Rank = 0,
                    GlobalChat = true
                };

                Database.CreatePlayer(newPlayer);
            }

            PlayerModel dbEntry = Database.GetPlayerModel(Name);
            CurrentRank = Common.Rank.GetRank(dbEntry.Rank);

            if (Entity != null) {
                Entity.PrettyName = CurrentRank.Prefix + Name + CurrentRank.Suffix;
            }

            Stopped = dbEntry.Stopped;
            _banned = dbEntry.Banned;

            var bannedUntil = dbEntry.BannedUntil;
            var mutedUntil = dbEntry.TimeMuted;

            DateTime banTime = Utils.GetUnixEpoch().AddSeconds(bannedUntil);

            if (banTime > DateTime.UtcNow) {
                _banned = true;
            }

            MutedUntil = Utils.GetUnixEpoch().AddSeconds(mutedUntil);
        }

        public void SendChat(string message) {
            SendSplitChat(Text.SplitLines(message));
        }

        public void Kick(string reason, bool hide = false) {
            _client.Kick(reason);
        }

        public void SendDefineBlock() {
            throw new NotImplementedException();
        }

        public void SendDeleteBlock(byte blockId) {
            throw new NotImplementedException();
        }
    }
}
