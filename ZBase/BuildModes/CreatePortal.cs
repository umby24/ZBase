using System;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.BuildModes {
    public class PortalCommand : Command {
        public PortalCommand()
        {
            CommandString = "portal";
            CommandAliases = new[] { "teleporter", "createtp" };
            MinRank = 200;
            Group = "Map";
            Description = "Usage: /portal [add/remove/find] [name]<br>" +
                          "Allows creation, addition, or finding of portals.<br>" +
                          "Add requires a name.";
        }

        private void HandleAll()
        {

        }
        private void HandleAdd(string[] args)
        {
            if (args.Length != 2)
            {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }
            var bm = BuildModeManager.Instance.GetBuildmode(Constants.AddPortalBuildModeName, ExecutingClient);
            ExecutingClient.ClientPlayer.CurrentState.CurrentMode = bm;
            
            SendExecutorMessage("§SPlace two blocks to define the portal area.");
            var playerState = ExecutingClient.ClientPlayer.CurrentState;

            playerState.SetCoord(ExecutingClient.ClientPlayer.Entity.Location, 0); // -- Save the location of where to TP to as the current location.
            playerState.Set(ExecutingClient.ClientPlayer.Entity.CurrentMap.MapProvider.MapName, 0); // -- and current map.
            playerState.Set(0, 0); // -- Save a state flag to say no blocks have been set yet.
            playerState.Set(args[1], 1);
            // -- TODO: Maybe put build mode info in the bottom right with message types?
        }
        private void HandleRemove(string[] args)
        {
            if (args.Length == 2)
            { // -- Remove by name..
                var found = ExecutingClient.ClientPlayer.Entity.CurrentMap.Portals.GetByName(args[1]);

                if (found == null)
                {
                    SendExecutorMessage($"§EPortal '{args[1]}' not found.");
                    return;
                }

                ExecutingClient.ClientPlayer.Entity.CurrentMap.Portals.Remove(found.Name);
                SendExecutorMessage("§STeleporter deleted.");
                return;
            }

            var bm = BuildModeManager.Instance.GetBuildmode(Constants.DeletePortalBuildModeName, ExecutingClient);
            ExecutingClient.ClientPlayer.CurrentState.CurrentMode = bm;

            SendExecutorMessage("§SDelete Portal Buildmode Started.");
            SendExecutorMessage("§SPlace a block inside a portal to delete it.");
        }
        private void HandleFind()
        {

        }
        public override void Execute(string[] args)
        {
            if (args.Length > 2 || args.Length <= 0)
            {
                SendExecutorMessage(Constants.InvalidNumArgumentsMessage);
                return;
            }

            switch (args[0].ToLower())
            {
                case "add":
                    HandleAdd(args);
                    break;
                case "remove":
                    HandleRemove(args);
                    break;
                case "find":
                    HandleFind();
                    break;
                case "all":
                    HandleAll();
                    break;
                default:
                    SendExecutorMessage($"§EInvalid portal mode '{args[0]}'.");
                    return;
            }
        }
    }
    public class DeletePortal : BuildMode
    {
        public DeletePortal()
        {
            Name = Constants.DeletePortalBuildModeName;
        }

        public override void Invoke(Vector3S location, byte mode, Block block)
        {
            var newLocation = new MinecraftLocation();
            newLocation.SetAsBlockCoords(location);

            Teleporter matches = ExecutingClient.ClientPlayer.Entity.CurrentMap.Portals.GetPortal(newLocation); //Teleporter.Matches(location, client.ClientPlayer.Entity.CurrentMap.Teleporters);

            if (matches == null) {
                SendExecutorMessage("§EThere is no portal here.");
                PlayerState.ResendBlocks(ExecutingClient);
                return;
            }

            SendExecutorMessage($"§SPortal {matches.Name} deleted.");
            ExecutingClient.ClientPlayer.Entity.CurrentMap.Portals.Remove(matches.Name);
            PlayerState.CurrentMode = null;
            PlayerState.ResendBlocks(ExecutingClient);
        }
    }
    public class CreatePortal : BuildMode
    {

        public CreatePortal()
        {
            Name = Constants.AddPortalBuildModeName;
        }

        public override void Invoke(Vector3S location, byte mode, Block block) {
            if (mode == 0)
                return;

            var state = PlayerState.GetInt(0);
            if (state == 0)
            {
                var newLocation = new MinecraftLocation();
                newLocation.SetAsBlockCoords(location);

                PlayerState.SetCoord(newLocation, 1);
                PlayerState.Set(1, 0);
                return;
            }

            MinecraftLocation dest = PlayerState.GetCoord(0);
            var map = PlayerState.GetString(0);
            MinecraftLocation firstBlock = PlayerState.GetCoord(1);

            var oEnd = new MinecraftLocation();
            oEnd.SetAsBlockCoords(location);
            
            // -- Fix up the coordinates

            var newTp = new Teleporter
            {
                Name = PlayerState.GetString(1),
                Destination = dest,
                DestinationMap = map,
                OriginEnd = oEnd,
                OriginStart = firstBlock
            };

            // -- Check for existing teleporters of the same name:
            Teleporter item = ExecutingClient.ClientPlayer.Entity.CurrentMap.Portals.GetByName(newTp.Name);

            if (item != null)
            {
                SendExecutorMessage("§SA teleporter with that name already exists in this map.");
                PlayerState.ResendBlocks(ExecutingClient);
                return;
            }

            ExecutingClient.ClientPlayer.Entity.CurrentMap.Portals.Create(newTp);
            SendExecutorMessage("§STeleporter Created.");
            PlayerState.CurrentMode = null;
            PlayerState.ResendBlocks(ExecutingClient);
        }
    }
}
