using ZBase.Common;
using ZBase.Network;

namespace ZBase.World {
    public partial class ClassicubePlayer {
        public void HandleChatReceived(string message) {
            SendSplitChat(Text.SplitLines(message));
        }

        private void MapBlockChange(Vector3S location, byte type) {
            IPacket bc = PacketCreator.CreateSetBlockServer(location, type);
            
            _client.BlockChanges.Enqueue(bc);
            _client.DataAvailable = true;
        }

        #region Entity Events
        /// <summary>
        /// Handle a client moving.
        /// </summary>
        /// <param name="entity"></param>
        private void SomeoneMoved(Entity entity) {
            sbyte entityId = -1;
            
            if (entity != Entity || !entity.SendOwn) {
                if (entity == Entity && !entity.SendOwn)
                    return;
                
                entityId = entity.ClientId;
            }

            IPacket t = PacketCreator.CreateTeleport(entity.Location, entityId);
            _client.SendPacket(t);
        }
        
        /// <summary>
        /// Spawns a new entity
        /// </summary>
        /// <param name="e">E.</param>
        public void SpawnEntity(Entity e) {
            IPacket spawn = PacketCreator.CreateSpawnPlayer(e.Location, e == Entity ? (sbyte)-1 : e.ClientId, e.PrettyName);
            _client.SendPacket(spawn);     
        }

        public void DespawnEntity(Entity e) {
            if (e == Entity)
                return;

            var despawn = PacketCreator.CreateDespawnPlayer(e.ClientId);
            _client.SendPacket(despawn);
        }
        /// <summary>
        /// This is triggered when the current player (our own entity) moves.
        /// </summary>
        /// <param name="location">This is the new location we have moved to.</param>
        public void HandleMove(MinecraftLocation location) {
            if (location == Entity.Location)
                return;

            Entity.Location = location; 

            Teleporter portal = Entity.CurrentMap.Portals.GetPortal(location);
            
            if (portal != null)
            {
                Entity.Location = portal.Destination;
                Entity.SendOwn = true;
            }

            Entity.HandleMove();
        }
        #endregion
       

        public bool CanPlaceBlock(byte blockType) {
            // -- Make sure the player hasn't been stopped.
            if (Stopped) {
                Chat.SendClientChat("§SYou are stopped!", 0, _client);
                return false;
            }

            // -- Check map build permissions
            if (Entity.CurrentMap.BuildRank > CurrentRank.Value) {
                Chat.SendClientChat("§EOnly " + Common.Rank.GetRank(Entity.CurrentMap.BuildRank) + "+ can build here.", 0, _client);
                return false;
            }

            // -- Deny placing of bedrock(?) if not an op.
            return blockType != 13 || CurrentRank.ClientOp;

            // -- TODO: Per-area build permissions based on rank..
        }
        
        /// <summary>
        /// Resend a single block to the client.
        /// Typical usage is denying a client block change.
        /// </summary>
        /// <param name="location">The location of the block to resend.</param>
        public void BounceBlock(Vector3S location) {
            MapBlockChange(location, Entity.CurrentMap.GetBlockId(location.X, location.Y, location.Z));
        }
        
        // -- TODO: Maybe create a 'CanBuildAt(Location, Entity) method on the map?'

        /// <summary>
        /// Handles an incoming block change for the current player/client
        /// </summary>
        /// <param name="location">Location of incoming block change</param>
        /// <param name="type">Given block ID</param>
        /// <param name="mode">Mode. 1 = place, 0 = delete.</param>
        public void HandleBlockPlace(Vector3S location, byte type, byte mode) {
            if (!CanPlaceBlock(type)) { // -- Handle the case that a player does not support block permissions but isn't allowed to place something..
                BounceBlock(location);
                return;
            }

            if (CurrentState.CurrentMode != null) {
                CurrentState.AddBlock(location.X, location.Y, location.Z);
                CurrentState.CurrentMode.Invoke(location, mode, BlockManager.GetBlock(type));
                return;
            }
            
            byte actualType = 0;

            if (mode == 1)
                actualType = type;

            LastMaterial = BlockManager.GetBlock(type);
            Entity.CurrentMap.SetBlockId(location.X, location.Y, location.Z, actualType);
        }
    }
}