using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZBase.Common;
using ZBase.Build;
using ZBase.Network;
using ZBase.Network.Indev;
using ZBase.Persistence;

namespace ZBase.World {
    public partial class IndevPlayer : IMinecraftPlayer {
        public string Name { get; set; }
        public Rank CurrentRank { get; set; }
        public DateTime MutedUntil { get; set; }
        public int Id => throw new NotImplementedException();

        public int Rank => throw new NotImplementedException();

        public int CustomBlockLevel => throw new NotImplementedException();

        public int Ping => throw new NotImplementedException();

        public string LoginName => _client.Name;

        bool IMinecraftPlayer.Stopped => Stopped;

        public BuildState CurrentState { get; set; }
        public Entity Entity { get; set; }
        public Block LastMaterial { get; set; }
        public bool Stopped;
        private bool _banned;

        private INetworkClient _client;
        public IndevPlayer(INetworkClient client) {
            _client = client;
            CurrentState = new BuildState();
        }

        public void Login() {
            LoadDbInfo();

            if (_banned) {
                _client.Kick(Constants.DefaultBanMessage);
                return;
            }

            var loginMap = HcMap.DefaultMap;
            var loginMapSize = loginMap.GetSize();

            Entity = new Entity { // -- Create our entity
                Name = Name,
                Location = loginMap.GetSpawn(),
                CurrentMap = loginMap,
                PrettyName = CurrentRank.Prefix + Name + CurrentRank.Suffix
            };

            Entity.OnEntityDespawned += DespawnEntity;
            Entity.OnEntitySpawned += SpawnEntity;
            Entity.OtherEntityMoved += SomeoneMoved;
            
            _client.SendHandshake(false);

            // -- some stuff..
            MapSend(loginMap.GetMapBlocks(), new byte[loginMapSize.X * loginMapSize.Y * loginMapSize.Z]);

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

        protected internal void LoadDbInfo() {
            if (!ClassicubePlayer.Database.ContainsPlayer(Name)) {
                var newPlayer = new PlayerModel {
                    Name = Name,
                    Ip = _client.Ip,
                    Rank = 0,
                    GlobalChat = true
                };

                ClassicubePlayer.Database.CreatePlayer(newPlayer);
            }

            PlayerModel dbEntry = ClassicubePlayer.Database.GetPlayerModel(Name);
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

        private void SendSplitChat(IEnumerable<string> messages) {
            foreach (var msg in messages) {
                var final = msg;
                if (Chat.FirstIsEmote(final[0])) {
                    final = "." + final;
                }
                final = final.Replace("&", "§");
                _client.SendPacket(new ChatMessagePacket() { Message = final });
            }
        }

        private void MapSend(byte[] data, byte[] metadata) {
            byte[] withLenMap = new byte[data.Length + 4];
            byte[] mapMeta = new byte[metadata.Length + 4];
            

            byte[] lenBytes = BitConverter.GetBytes(data.Length); // -- Get the length of the map
            Array.Reverse(lenBytes); // -- Reverse it for proper endianness
            Buffer.BlockCopy(lenBytes, 0, mapMeta, 0, 4); // -- Copy the length into the send map
            Buffer.BlockCopy(lenBytes, 0, withLenMap, 0, 4); // -- Copy the length into the send map
            Buffer.BlockCopy(data, 0, withLenMap, 4, data.Length); // -- Copy all of the block data in

            byte[] zippedMap = GZip.Compress(withLenMap);
            byte[] zippedMeta = GZip.Compress(mapMeta);

            var mapDataPack = new LevelData() {
                MapData = zippedMap,
                MetaData = zippedMeta,
                MapSize = zippedMap.Length,
                MetaSize = zippedMeta.Length
            };
            _client.SendPacket(mapDataPack);
            var mapSize = Entity.CurrentMap.GetSize();

            var finalize = new Network.Indev.LevelFinalize() {
                LevelSize = zippedMap.Length,
                LevelShape = 0,
                LevelTheme = 0,
                LevelType = 0,
                Width = mapSize.X,
                Height = mapSize.Z,
                Depth = mapSize.Y,
                WorldTime = 100
            };
            _client.SendPacket(finalize);
        }

        public void ChangeMap(HcMap map) {
            Chat.SendGlobalChat($"§SPlayer {Entity.PrettyName}§S changed to map '{map.MapProvider.MapName}'", 0); // -- TODO: Move this into an event handler.
            //DespawnEntities();

            // -- Cleanup events
            Entity.CurrentMap.BlockChanged -= MapBlockChange;
            Entity.CurrentMap.MapChatSent -= HandleChatReceived;

            // -- Send the new map
            Entity.CurrentMap = map;

            _client.SendHandshake(CurrentRank.ClientOp);
            var loginMapSize = Entity.CurrentMap.GetSize();
            MapSend(Entity.CurrentMap.GetMapBlocks(), new byte[loginMapSize.X * loginMapSize.Y * loginMapSize.Z]);
            Entity.CurrentMap.BlockChanged += MapBlockChange; // -- sub to block changes
            Entity.CurrentMap.MapChatSent += HandleChatReceived;

            // -- setup our entity
            Entity.Location = Entity.CurrentMap.GetSpawn();

            var entities = Entity.CurrentMap.Entities;
            Parallel.ForEach(entities, SpawnEntity);

            Entity.HandleMove(); // -- make sure initial position is set.
        }

        public void Kick(string reason, bool hide = false) {
            throw new NotImplementedException();
        }

        public void SendChat(string message) {
            SendSplitChat(Text.SplitLines(message));
        }

        public void SendDefineBlock() {
            throw new NotImplementedException();
        }

        public void SendDeleteBlock(byte blockId) {
            throw new NotImplementedException();
        }
    }
}
