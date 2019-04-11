using System.Collections.Generic;
using ZBase.Common;
using ZBase.Network;

namespace ZBase.World {
    public class Entity {
        public static List<Entity> AllEntities = new List<Entity>();
        public string Name { get; set; }
        public string PrettyName { get; set; }
        public sbyte ClientId { get; set; }
        public MinecraftLocation Location { get; set; }
        private HcMap _currentMap;

        public HcMap CurrentMap {
            get {
                return _currentMap;
            }
            set {
                if (_currentMap != null && value != _currentMap) {
                    _currentMap.EntityRemove(this); // -- Invoke map events..
                    value.EntityAdd(this);
                }

                _currentMap = value;
            }
        }

        public Block HeldBlock { get; set; }
        public Client AssoClient;
        public bool SendOwn;

        /// <summary>
        /// This event triggers when this entity moves, so any subscribed client will receive the update.
        /// </summary>
        public event EntityEventArgs EntityMoved;


        public Entity() {
            SendOwn = true;
        }

        public Entity(Client c) {
            AssoClient = c;
            SendOwn = true;
        }

        public void Spawn() {
            lock (AllEntities) {
                AllEntities.Add(this);
            }
            
            foreach (var client in Server.RoClients) {
                if (client.Verified == false)
                    continue;

                if (client.ClientPlayer.CurrentMap != _currentMap)
                    continue;

                client.ClientPlayer.SpawnEntity(this);
            }
        }


        public void Despawn() {
            lock (AllEntities) {
                AllEntities.Remove(this);
            }

            foreach (Client client in Server.RoClients) {
                if (client.Verified == false)
                    continue;
                if (client.ClientPlayer.CurrentMap != _currentMap)
                    continue;

                client.ClientPlayer.DespawnEntity(this);
            }

            CurrentMap.ReturnEntityId((byte)ClientId);
        }

        public void HandleMove() {
            EntityMoved?.Invoke(this);
            SendOwn = false;
        }

        public Vector3S GetBlockCoords() {
            return Location.GetAsBlockCoords();
        }
    }
}
