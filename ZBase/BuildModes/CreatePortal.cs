namespace ZBase.BuildModes {
    //public class PortalCommand : Command {
    //    public PortalCommand() {
    //        CommandString = "portal";
    //        CommandAliases = new[] {"teleporter", "createtp"};
    //        MinRank = 200;
    //        Group = "Map";
    //        Description = "Usage: /portal [add/remove/find] [name]<br>" +
    //                      "Allows creation, addition, or finding of portals.<br>" +
    //                      "Add requires a name.";
    //    }

    //    private void HandleAll() {

    //    }
    //    private void HandleAdd(Client c, string[] args) {
    //        if (args.Length != 2) {
    //            Chat.SendClientChat("§EInvalid number of arguments.", 0, c);
    //            return;
    //        }

    //        c.ClientPlayer.BMode = BuildModeManager.BuildModes["CreatePortal"];
    //        c.ClientPlayer.State = new BuildState();
    //        Chat.SendClientChat("§SBuildmode 'Create Portal' started.", 0, c);
    //        Chat.SendClientChat("§SPlace two blocks to define the portal area.", 0, c);
    //        c.ClientPlayer.State.SetCoord(c.ClientPlayer.Entity.Location, 0); // -- Save the location of where to TP to as the current location.
    //        c.ClientPlayer.State.SetString(c.ClientPlayer.CurrentMap.Name, 0); // -- and current map.
    //        c.ClientPlayer.State.SetInt(0, 0); // -- Save a state flag to say no blocks have been set yet.
    //        c.ClientPlayer.State.SetString(args[1], 1);
    //        // -- TODO: Maybe put build mode info in the bottom right with message types?
    //    }
    //    private void HandleRemove(Client c, string[] args) {
    //        if (args.Length == 2) { // -- Remove by name..
    //            var found =
    //                c.ClientPlayer.CurrentMap.Teleporters.FirstOrDefault(a => a.Name.ToLower() == args[1].ToLower());
    //            if (found == null) {
    //                Chat.SendClientChat($"§EPortal '{args[1]}' not found.", 0, c);
    //                return;
    //            }
    //            c.ClientPlayer.CurrentMap.Teleporters.Remove(found);
    //            Chat.SendClientChat("§STeleporter deleted.", 0, c);
    //            return;
    //        }

    //        c.ClientPlayer.BMode = BuildModeManager.BuildModes["DeletePortal"];
    //        c.ClientPlayer.State = new BuildState();
    //        Chat.SendClientChat("§SDelete Portal Buildmode Started.", 0, c);
    //        Chat.SendClientChat("§SPlace a block inside a portal to delete it.", 0, c);
    //    }
    //    private void HandleFind() {

    //    }
    //    public override void Execute(Client c, string[] args) {
    //        if (args.Length > 2 || args.Length < 0) {
    //            Chat.SendClientChat("§EInvalid number of arguments.", 0, c);
    //            return;
    //        }
    //        switch (args[0].ToLower()) {
    //            case "add":
    //                HandleAdd(c, args);
    //                break;
    //            case "remove":
    //                HandleRemove(c, args);
    //                break;
    //            case "find":
    //                HandleFind();
    //                break;
    //            case "all":
    //                HandleAll();
    //                break;
    //            default:
    //                Chat.SendClientChat($"§EInvalid portal mode '{args[0]}'.", 0, c);
    //                return;
    //        }
    //    }
    //}
    //public class DeletePortal : BuildMode {
    //    public DeletePortal() {
    //        Name = "DeletePortal";
    //    }
    //    public override void Invoke(Client client, Vector3S location, byte mode, Block block) {
    //        Teleporter matches = Teleporter.Matches(location, client.ClientPlayer.CurrentMap.Teleporters);

    //        if (matches == null) {
    //            Chat.SendClientChat("§EThere is no portal here.", 0, client);
    //            client.ClientPlayer.State.ResendBlocks(client);
    //            return;
    //        }

    //        Chat.SendClientChat($"§SPortal {matches.Name} deleted.", 0, client);
    //        client.ClientPlayer.CurrentMap.Teleporters.Remove(matches);
    //        client.ClientPlayer.BMode = null;
    //        client.ClientPlayer.State.ResendBlocks(client);
    //    }
    //}

    //public class CreatePortal : BuildMode {
    //    public CreatePortal() {
    //        Name = "CreatePortal";
    //    }

    //    public override void Invoke(Client client, Vector3S location, byte mode, Block block) {
    //        if (mode == 0)
    //            return;

    //        int state = client.ClientPlayer.State.GetInt(0);
    //        if (state == 0) {
    //            client.ClientPlayer.State.SetCoord(location, 1);
    //            client.ClientPlayer.State.SetInt(1, 0);
    //            return;
    //        }

    //        var dest = client.ClientPlayer.State.GetCoord(0);
    //        var map = client.ClientPlayer.State.GetString(0);
    //        var firstBlock = client.ClientPlayer.State.GetCoord(1);

    //        // -- Fix up the coordinates
    //        var oStart = new Vector3S(Math.Min(firstBlock.X, location.X),
    //            Math.Min(firstBlock.Y, location.Y), Math.Min(firstBlock.Z, location.Z));

    //        var oEnd = new Vector3S(Math.Max(firstBlock.X, location.X),
    //            Math.Max(firstBlock.Y, location.Y), Math.Max(firstBlock.Z, location.Z));

    //        var newTp = new Teleporter {
    //            Name = client.ClientPlayer.State.GetString(1),
    //            Destination = dest,
    //            DestinationLook = 0,
    //            DestinationMap = map,
    //            DestinationRot = 0,
    //            OriginEnd = oEnd,
    //            OriginStart = oStart
    //        };

    //        // -- Check for existing teleporters of the same name:
    //        Teleporter item =
    //            client.ClientPlayer.CurrentMap.Teleporters.FirstOrDefault(a => a.Name.ToLower() == newTp.Name.ToLower());

    //        if (item != null) {
    //            Chat.SendClientChat("§SA teleporter with that name already exists in this map.", 0, client);
    //            client.ClientPlayer.State.ResendBlocks(client);
    //            return;
    //        }

    //        client.ClientPlayer.CurrentMap.Teleporters.Add(newTp);
    //        Chat.SendClientChat("§STeleporter Created.", 0, client);
    //        client.ClientPlayer.BMode = null;
    //        client.ClientPlayer.State.ResendBlocks(client);
    //    }
    //}
}
