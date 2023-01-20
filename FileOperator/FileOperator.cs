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

            [System.Text.Json.Serialization.JsonConstructor]
            public GameData(int Height, int[] Field, bool FirstPlayerNext, bool IsSinglePlayer, string Name1 = "", string Name2 = "")
            {
                this.Height = Height;
                this.Field = Field;
                this.FirstPlayerNext = FirstPlayerNext;
                this.Name1 = Name1;
                this.Name2 = Name2;
                this.IsSinglePlayer = IsSinglePlayer;
            }
        }

        public static GameData LoadGame()
        {
            try
            {
                string json = File.ReadAllText(fileName);
                if (json != "")
                {
                    var data = System.Text.Json.JsonSerializer.Deserialize<GameData>(json);
                    GameData gd = new GameData(data.Height, data.Field, data.FirstPlayerNext, data.IsSinglePlayer, data.Name1, data.Name2);
                    return gd;
                }
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public static void SaveGame(int h, int[] DisksToInt, bool first_player_next, bool single, string name1, string name2)
        {
            
            GameData gd = new GameData(h, DisksToInt, first_player_next, single, name1, name2);
            string json = System.Text.Json.JsonSerializer.Serialize(gd, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, json);
        }

    }
    public class Leaderboard
    {
        private static string fileName="Leaders.json";
        public class Leader
        {
            public int Height { get; set; }
            public int Score { get; set; }
            public string Name { get; set; }

            [System.Text.Json.Serialization.JsonConstructor]
            public Leader(int Height, int Score, string Name)
            {
                this.Height = Height;
                this.Score = Score;
                this.Name = Name;
            }
        }
        private Leader currentLead;
        private int currentLeadpos;
        private ObservableCollection<Leader> leaders;
        public ReadOnlyObservableCollection<Leader> Leaders { get; }
        public Leaderboard()
        {
            leaders = new ObservableCollection<Leader>();
            Leaders = new ReadOnlyObservableCollection<Leader>(leaders);
        }
        public void Add(int Height, int Score, string Name)
        {
            leaders.Add(new Leader(Height, Score, Name));
        }

        public void Import()
        {
            try
            {
                string json = File.ReadAllText(fileName);
                if (json != "")
                {
                    var list = JsonSerializer.Deserialize<Leader[]>(json);
                    foreach (var entry in list)
                    {
                        Add(entry.Height, entry.Score, entry.Name);
                    }
                }
            }
            catch{ }
        }
        private void Export()
        {
            string json = JsonSerializer.Serialize(leaders.ToList(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, json);
        }
        public string Find(int h)
        {
            currentLeadpos = 0;
            foreach (var lead in leaders)
            {
                if (lead.Height == h)
                {
                    currentLead = lead;
                    return lead.Name + " - " + lead.Score.ToString();
                }
                currentLeadpos++;
            }
            return "Нет рекорда.";
        }
        public void NewLead(int h, string name, int score)
        {
            if (currentLead != null)
            {
                if (currentLead.Score < score)
                    leaders[currentLeadpos] = new Leader(h, score, name);
            }
            else
                Add(h, score, name);
            Export();
        }
    }
}
