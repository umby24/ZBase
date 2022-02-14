using System;
using System.Collections.Generic;
using System.Linq;
using ClassicWorldCore;
using fNbt;
using ZBase.Build;
using ZBase.Network;
using ZBase.World;

namespace ZBase.Common {

    public static class Extensions {
        public static string UpperFirst(this string input) {
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }

    public class WatchdogModule {
        public string Name { get; set; }    
        public DateTime WatchTime { get; set; }
        public string BiggestMessage { get; set; }
        public string LastMessage { get; set; }
        public int Timeout { get; set; }
        public int BiggestTimeout { get; set; }
        public int TimeoutMax { get; set; }
        public int CallsPerSecond { get; set; }
    }

    public abstract class Mapfill {

        public string Name { get; set; }
        public Vector3S MapSize;
        public int GetBlockCoords(int x, int y, int z) {
            return (z * MapSize.Z + y) * MapSize.X + x;
        }
        public abstract void Execute(HcMap map, string[] args);
    }

	public class Rank {
		public string Name {get; set;}	
		public string Prefix {get; set;}
		public string Suffix {get; set;}
		public int Value {get; set;}
		public bool ClientOp {get; set;}

        public override string ToString() {
            return $"{Prefix}{Name}{Suffix}";
        }
		public static Rank[] GetDefaultRanks() {
			var defaults = new[] {
				new Rank {
					Name = "Guest",
					Value = 0,
					Prefix = "&9"
				},
				new Rank {
					Name = "Owner",
					Prefix = "&4",
					Value = 65535
				}
			};

			return defaults;
		}

		public static Rank GetRank(int number) {
			var current = -32769;
			var index = 0;

			for (int i = 0; i < Configuration.Settings.Ranks.Length; i++) {
				var rnk = Configuration.Settings.Ranks [i];

				if (number >= rnk.Value && current < rnk.Value) {
					current = rnk.Value;
					index = i;
				}
			}

			return Configuration.Settings.Ranks [index];
		}

	    public static Rank GetRank(string name) {
	        foreach (Rank rnk in Configuration.Settings.Ranks) {
	            if (rnk.Name.ToLower() == name.ToLower())
	                return rnk;
	        }

	        return null;
	    }
	}

    public abstract class Command {
        public string CommandString { get; set; } // -- Provides the primary command string to execute this command
        public string[] CommandAliases { get; set; } // -- Provides default aliases for executing this command.
		public string Group {get; set;} // -- The group this command belongs to (Ex. General, Op, Map)
		public int MinRank { get; set; }
        public string Description { get; set; }
        public Client ExecutingClient { get; set; }
        public abstract void Execute(string[] args);

        protected Client[] GetOnlineClient(string name) {
            var clients = Server.RoClients.Where(a => String.Equals(a.ClientPlayer.Name, name, StringComparison.CurrentCultureIgnoreCase)).ToArray();
            return clients;
        }
        
        protected void SendExecutorMessage(string message) {
            Chat.SendClientChat(message, 0, ExecutingClient);    
        }
    }

	public class HotkeyEntry {
		public string Label {get; set;}	
		public string Action {get; set;}
		public int KeyCode {get; set;}
		public byte Keymods {get; set;}
	}

    public delegate void EmptyEventArgs();

    public delegate void EntityEventArgs(Entity e);

    public delegate void StringEventArgs(string s);

    public delegate void LogEventArgs(LogItem i);
	public class CpeSettings {
		public List<HotkeyEntry> Hotkeys {get; set;}	
		public short ClickDistance {get; set;}

		public CpeSettings() {
			Hotkeys = new List<HotkeyEntry> ();
			ClickDistance = 160;
		}
	}

    public class TextSettings {
        public string Error { get; set; }
        public string System { get; set; }
        public string Divider { get; set; }

        public TextSettings() {
            Error = "&4Error:&f ";
            System = "&e";
            Divider = "&3|";
        }
    }


    public abstract class BuildMode {
        public string Name { get; set; }
        public Client ExecutingClient { get; set; }
        public BuildState PlayerState {
            get { return ExecutingClient.ClientPlayer.CurrentState; }
        }
        public abstract void Invoke(Vector3S location, byte mode, Block block);
        
        protected void SendExecutorMessage(string message) {
            Chat.SendClientChat(message, 0, ExecutingClient);    
        }
    }

    public class UndoItem {
        public Vector3S Location { get; set; }
        public Block OldBlock { get; set; }
        public Block NewBlock { get; set; }
    }

    public class GeneralSettings {
        public string Name { get; set; }
        public string Motd { get; set; }
        public string LogLevel { get; set; }
        public string LogDirectory { get; set; }
        public int LogPrune { get; set; }
        public string DefaultMap { get; set; }
        public bool LogArguments { get; set; }

        public GeneralSettings() {
            LogLevel = "Info";
            Name = "Hypercube Server";
            Motd = "&eWelcome to the server!";
            DefaultMap = "default.cw";
            LogArguments = true;
            LogPrune = 5;
            LogDirectory = "Logs";
        }
    }
    public class NetworkSettings {
        public int MaxPlayers { get; set; }
        public int ListenPort { get; set; }
        public bool VerifyNames { get; set; }
        public bool Public { get; set; }

        public NetworkSettings() {
            ListenPort = 25565;
            VerifyNames = true;
            Public = true;
            MaxPlayers = 128;
        }
    }

    public enum LogType {
        Verbose,
        Debug,
        Error,
        Warning,
        Info,
        Chat,
        Command
    }

    public enum WeatherType {
        Sunny = 0,
        Raining,
        Snowing
    }

    public struct LogItem {
        public LogType Type;
        public DateTime Time;
        public string Message;
    }

    public enum EnviromentColorType {
        SkyColor = 0,
        CloudColor,
        FogColor,
        AmbientLight,
        DiffuseLight
    }

    public enum EnvProperties {
        SideBlock = 0,
        EdgeBlock,
        EdgeHeight,
        CloudHeight,
        MaxViewDistance,
        CloudSpeed,
        WeatherSpeed,
        WeatherFade,
        UseExponentialFog,
        EdgeOffset
    }

    public abstract class TaskItem {
        public TimeSpan Interval;
        public DateTime LastRun;
        public abstract void Setup();
        public abstract void Main();
        public abstract void Teardown();
    }

    /// <summary>
    /// Interface for Classic packets.
    /// </summary>
    public interface IPacket {
        int PacketLength { get; }
        void Read(IByteBuffer client);
        void Write(IByteBuffer client);
        void Handle(Client client);
    }

    /// <summary>
    /// Interface for Classic packets.
    /// </summary>
    public interface IIndevPacket
    {
        int PacketLength { get; }
        static byte Id { get; }
        int GetId();
        void Read(IByteBuffer client);
        void Write(IByteBuffer client);
        void Handle(INetworkClient client);
    }



    public class Block {
        public byte Id { get; set; } // -- Internal server ID
        public byte OnClient { get; set; } // -- The ID sent to the client
        public string Name { get; set; } // -- Name of the block
        
        public int Color { get; set; } // -- Color when used for generating maps

        // -- Physics Related
        public int Physics { get; set; } // -- Which variation on the built in server physics does this use?
        public int ReplaceOnLoad { get; set; } // -- Should this block be replaced whenever a map is loaded? if so with what
        public int PhysicsDelay { get; set; } // -- Delay between runs of physics
        public int PhysicsRandom { get; set; } // -- Random value to use on physics
        public string PhysicsPlugin { get; set; } // -- Which plugin will handle physics for this block?
        public bool RepeatPhysics { get; set; } // -- Does physics only occur once, or does it continue repeatedly?
        
        public bool Kill { get; set; } // -- Does this block kill?
        public bool Special { get; set; } // -- Custom materials show up on /materials list.

        public int PlaceRank { get; set; }
        public int DeleteRank { get; set; }
    }

    public class CustomBlock : Block {
        public int CPELevel { get; set; } // -- Which level of CPE implements this block?
        public int CPEReplace { get; set; } // -- What should this be replaced with for non-supportive clients?
        
        public byte Solidity { get; set; }
        public byte MovementSpeed { get; set; }
        public byte TopTextureId { get; set; }
        public byte SideTextureId { get; set; }
        public byte BottomTextureId { get; set; }
        
        // -- CustomBlockExt:
        public byte LeftTextureId { get; set; }
        public byte RightTextureId { get; set; }
        public byte FrontTextureId { get; set; }
        public byte BackTextureId { get; set; }
        public Vector3S Minimum { get; set; } // -- Actually bytes, but oh well.
        public Vector3S Maximum { get; set; }
        //--
        
        public byte TransmitsLight { get; set; }
        public byte WalkSound { get; set; }
        public byte FullBright { get; set; }
        public byte Shape { get; set; }
        public byte BlockDraw { get; set; }
        public byte FogDensity { get; set; }
        public byte FogR { get; set; }
        public byte FogG { get; set; }
        public byte FogB { get; set; }
    }
    
    public class EnviromentColors {
        public EnviromentColorType ColorType;
        public short Red { get; set; }
        public short Green { get; set; }
        public short Blue { get; set; }
    }

    public class SelectionCuboid {
        public byte SelectionId { get; set; }
        public string Label { get; set; }
        public Vector3S Start { get; set; }
        public Vector3S End { get; set; }
        public Vector3S Color { get; set; } // -- X = R, Y = G, Z = B.
        public short Opacity { get; set; }
    }

    public class BlockPermission {
        public byte BlockId { get; set; }
        public bool CanPlace { get; set; }
        public bool CanDelete { get; set; }
    }

    public class HackPermissions {
        public bool Fly { get; set; }
        public bool NoClip { get; set; }
        public bool Speed { get; set; }
        public bool Spawn { get; set; }
        public bool ThirdPerson { get; set; }
        public short JumpHeight { get; set; }
        public HackPermissions() {
            Fly = true;
            NoClip = true;
            Speed = true;
            Spawn = true;
            ThirdPerson = true;
            JumpHeight = -1;
        }
    }

    public struct HypercubeMetadata : IMetadataStructure {
        public HackPermissions HackPermissions { get; set; }
        public Dictionary<EnvProperties, int> EnviromentProperties { get; set; }
        public string TextureUrl { get; set; }
        public short BuildRank { get; set; }
        public short ShowRank { get; set; }
        public short JoinRank { get; set; }
        public int MetadataVersion => 1;
        public NbtCompound Read(NbtCompound metadata) {
            var hcData = metadata.Get<NbtCompound>("ZBase");

            if (hcData == null)
                return metadata;
            int currentVersion = hcData["Version"].IntValue;

            if (currentVersion == 1)
                ReadVersionOne(hcData);

            metadata.Remove(hcData);
            return metadata;
        }

        private void ReadVersionOne(NbtCompound hcData) {

            if (hcData["EnvProps"] != null) {
                EnviromentProperties = new Dictionary<EnvProperties, int>();
                for (var i = 0; i < 9; i++) {
                    var propName = (EnvProperties)i;

                    if (hcData["EnvProps"][propName.ToString()] != null)
                        EnviromentProperties.Add(propName, hcData["EnvProps"][propName.ToString()].IntValue);

                }
            }

            if (hcData["HackPerms"] != null) {
                HackPermissions = new HackPermissions {
                    Fly = hcData["HackPerms"]["Fly"].ByteValue > 0,
                    JumpHeight = hcData["HackPerms"]["Jump"].ShortValue,
                    NoClip = hcData["HackPerms"]["Clip"].ByteValue > 0,
                    Spawn = hcData["HackPerms"]["Spawn"].ByteValue > 0,
                    Speed = hcData["HackPerms"]["Speed"].ByteValue > 0,
                    ThirdPerson = hcData["HackPerms"]["Third"].ByteValue > 0
                };
            }

            if (hcData["TextureUrl"] != null)
                TextureUrl = hcData["TextureUrl"].StringValue;

            if (hcData["Permissions"] != null) {
                BuildRank = (short)hcData["Permissions"]["Build"].IntValue;
                JoinRank = (short)hcData["Permissions"]["Join"].IntValue;
                ShowRank = (short)hcData["Permissions"]["Show"].IntValue;
            }

        }
        public NbtCompound Write() {
            var baseComp = new NbtCompound("ZBase");

            if (HackPermissions != null) {
                var hackTag = new NbtCompound("HackPerms")
                {
                    new NbtByte("Fly", HackPermissions.Fly ? (byte)1 : (byte)0),
                    new NbtByte("Clip", HackPermissions.NoClip ? (byte)1 : (byte)0),
                    new NbtByte("Spawn", HackPermissions.Spawn ? (byte)1 : (byte)0),
                    new NbtByte("Speed", HackPermissions.Speed ? (byte)1 : (byte)0),
                    new NbtByte("Third", HackPermissions.ThirdPerson ? (byte)1 : (byte)0),
                    new NbtShort("Jump", HackPermissions.JumpHeight)
                };
                baseComp.Add(hackTag);
            }

            if (EnviromentProperties != null) {
                var propsTag = new NbtCompound("EnvProps");

                foreach (KeyValuePair<EnvProperties, int> property in EnviromentProperties) {
                   propsTag.Add(new NbtInt(property.Key.ToString(), property.Value));
                }

                baseComp.Add(propsTag);
            }

            if (!string.IsNullOrEmpty(TextureUrl))
                baseComp.Add(new NbtString("TextureUrl", TextureUrl));

            var permsComp = new NbtCompound("Permissions") {
                new NbtShort("Build", BuildRank),
                new NbtShort("Show", ShowRank),
                new NbtShort("Join", JoinRank)
            };

            baseComp.Add(permsComp);
            baseComp.Add(new NbtInt("Version", MetadataVersion));

            return baseComp.Any() ? baseComp : null;
        }
    }
    
    public abstract class ZBasePlugin {
        public string PluginName { get; set; }
        public string PluginAuthor { get; set; }
        public int PluginVersion { get; set; }
        public int ApiVersion { get; set; }

        public abstract void PluginInit();
        public abstract void PluginUnload();
    }
}
