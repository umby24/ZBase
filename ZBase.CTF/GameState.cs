using System;
using System.Text.Json;
using ZBase.Common;
using ZBase.Network;
using ZBase.World;

namespace ZBase.CTF;

public class GameState
{
    public static GameState Instance { get; set; } = new GameState();
    public bool Running { get; set; }
    public HcMap GameMap { get; set; }
    public List<HcMap> Maps {get; set;}
    public List<Player> GamePlayers { get; set; }
  

    public static void SaveSettings()
    {
        try
        {
            File.WriteAllText("ctfsettings.json", JsonSerializer.Serialize(Instance));
        }  catch (Exception ex)
        {
            Console.WriteLine($"Error saving CTF settings: {ex.Message}");
        }
    }

    public static void LoadSettings()
    {
        try
        {
            if (File.Exists("ctfsettings.json"))
            {
                Instance = JsonSerializer.Deserialize<GameState>(File.ReadAllText("ctfsettings.json"));
            }
        } catch (Exception ex)
        {
            Console.WriteLine($"Error loading CTF settings: {ex.Message}");
        }
    }

    public static void GameStart()
    {
        Chat.SendGlobalChat($"§SA CTF game is starting on {Instance.GameMap.Name}! Type /ctf join to join.", 0);
    }
}
