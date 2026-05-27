using ZBase.Common;
using ZBase.World;
using Newtonsoft.Json;

namespace ZBase.CTF;

public class MapSettings
{
    public  MinecraftLocation DefaultSpawn {get; set;}
    public  MinecraftLocation BlueSpawn {get; set;}
    public  MinecraftLocation RedSpawn {get; set;}
    public  MinecraftLocation RedFlag {get; set;}
    public  MinecraftLocation BlueFlag {get; set;}
    public Block RedFlagBlock {get; set;}
    public Block BlueFlagBlock {get; set;}
    public int ZDividier {get; set;}
    public int RoundPoints {get; set;} = 3;
    public int TagPointGain {get; set;} = 5;
    public int TagPointLoss {get; set;} = 5;
    public int CapturePointGain { get; set; } = 10;
    public int CapturePointLoss { get; set; } = 10;

    private HcMap _map;

    public MapSettings(HcMap map)
    {
        _map = map;
    }
    
    public void SetDefaults()
    {
        var mapSize = _map.GetSize();
        ZDividier = mapSize.Y / 2;
        RedFlagBlock = BlockManager.GetBlock(21);
        BlueFlagBlock = BlockManager.GetBlock(29);
        
        int midX = mapSize.X / 2;
        int midZ = mapSize.Z / 2;
        int topZ = midZ + 2;
        int maxY = mapSize.Y - 1;
        
        RedFlag = new MinecraftLocation();
        RedFlag.SetAsBlockCoords(new Vector3S(midX, 0, midZ));
        
        RedSpawn = new MinecraftLocation();
        RedSpawn.SetAsBlockCoords(new Vector3S(midX, 0, midZ));
        
        BlueSpawn = new MinecraftLocation();
        BlueSpawn.SetAsBlockCoords(new Vector3S(midX, maxY, midZ));
        
        BlueFlag = new MinecraftLocation();
        BlueFlag.SetAsBlockCoords(new Vector3S(midX, maxY, topZ));
    }

    private string GetFilePath()
    {
        return _map.Filename.Replace(".cw", "") + "_ctf.json";
    }
    public void Load()
    {
        try
        {
            this = JsonConvert.DeserializeObject<MapSettings>(File.ReadAllText(GetFilePath()));
        }
        catch (Exception e)
        {
            
        }
    }

    public void Save()
    {
        
    }
}