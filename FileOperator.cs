using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text.Json;

namespace Reversi
{
    
    public class FileOperator
    {
        private static string fileName = "Save.json";
        public class GameData
        {
            public string Name1 { get; set; }
            public string Name2 { get; set; }
            public bool FirstPlayerNext { get; set; }
            public bool IsSinglePlayer { get; set; }
            public int[] Field { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }

        [System.Text.Json.Serialization.JsonConstructor]
            public GameData(int Height, int Width, int[] Field, bool FirstPlayerNext, bool IsSinglePlayer, string Name1 = "", string Name2 = "")
            {
                this.Height = Height;
                this.Width = Width;
                this.Field = Field;
                this.FirstPlayerNext = FirstPlayerNext;
                this.Name1 = Name1;
                this.Name2 = Name2;
                this.IsSinglePlayer = IsSinglePlayer;
            }
        }
        
        public static GameData LoadGame()
        {
            string json = File.ReadAllText(fileName);
            if (json != "")
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<GameData>(json);
                GameData gd = new GameData(data.Height, data.Width, data.Field, data.FirstPlayerNext, data.IsSinglePlayer, data.Name1, data.Name2);
                return gd;
            }
            else
                return null;
        }

        public static void SaveGame(GameAttributes g, bool single)
        {
            int h = g.Placed_Disks.Placed_disks.GetLength(0);
            int w = g.Placed_Disks.Placed_disks.GetLength(1);
            GameData gd = new GameData(h, w, g.DisksToInt(), g.first_player_next, single, g.Player1.Player_name, g.Player2.Player_name);
            string json = System.Text.Json.JsonSerializer.Serialize(gd, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, json);
        }
        
    }
}
