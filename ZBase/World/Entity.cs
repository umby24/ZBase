using System.Collections.Generic;
using ZBase.Common;

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
                    RemoveMapEvents();
                    _currentMap.EntityRemove(this); // -- Invoke map events..
                    value.EntityAdd(this);
                }

                _currentMap = value;
                AddMapEvents();
            }
        }

        public Block HeldBlock { get; set; }
        public bool SendOwn;

        /// <summary>
        /// This event triggers when this entity moves, so any subscribed client will receive the update.
        /// </summary>
        public event EntityEventArgs EntityMoved;
        /// <summary>
        /// This event triggers when this entity changes its held block, to broadcast to subbed clients.
        /// </summary>
        public event EntityEventArgs HeldBlockChanged;
        /// <summary>
        /// This event is triggered when an entity visible to this entity moves.
        /// </summary>
        public event EntityEventArgs OtherEntityMoved;
        /// <summary>
        /// This event is triggered when an entity visible to this entity changes their held block.
        /// </summary>
        public event EntityEventArgs OtherEntityHeldBlockChange;
        /// <summary>
        /// This event is triggered when a newly visible entity has been created.
        /// </summary>
        public event EntityEventArgs OnEntitySpawned;
        /// <summary>
        /// This event is triggered when a visible entity is being removed.
        /// </summary>
        public event EntityEventArgs OnEntityDespawned;
        
        public Entity() {
            SendOwn = true;
            HeldBlock = BlockManager.GetBlock(0);
        }

        private void RemoveMapEvents() {
            // -- Deregister events
            var currentEntities = _currentMap.Entities;
            foreach (Entity entity in currentEntities) {
                entity.EntityMoved -= HandleVisibleEntityMoved;
            }

            CurrentMap.EntityCreated -= CurrentMapOnEntityCreated;
            CurrentMap.EntityDestroyed -= CurrentMapOnEntityDestroyed;
        }

        private void AddMapEvents() {
            CurrentMap.EntityCreated += CurrentMapOnEntityCreated;
            CurrentMap.EntityDestroyed += CurrentMapOnEntityDestroyed;

            var visibleEntities = _currentMap.Entities;
            foreach (Entity entity in visibleEntities) {
                entity.EntityMoved += HandleVisibleEntityMoved;
            }
        }
        
        public void Spawn() {
            lock (AllEntities) {
                AllEntities.Add(this);
            }
            
            CurrentMap.EntityAdd(this);
        }
        
        public void Despawn() {
            lock (AllEntities) {
                AllEntities.Remove(this);
            }
            RemoveMapEvents();
            // -- Trigger others to remove us.
            CurrentMap.EntityRemove(this);
        }
        
        private void HandleVisibleEntityMoved(Entity e) {
            OtherEntityMoved?.Invoke(e);
        }

        private void CurrentMapOnEntityDestroyed(Entity e) {
            e.EntityMoved -= HandleVisibleEntityMoved;
            OnEntityDespawned?.Invoke(e);
        }

        private void CurrentMapOnEntityCreated(Entity e) {
            e.EntityMoved += HandleVisibleEntityMoved;
            OnEntitySpawned?.Invoke(e);
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
