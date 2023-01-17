using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Reversi
{
    public class ViewModel : INotifyPropertyChanged
    {
        public delegate void AccountHandler((int, int) iter, Ellipse disk);
        public event AccountHandler AddDiskToField;
        public delegate void AccountHandler2();
        public event AccountHandler2 StartNewGame, IncorrectInput, EndGame, ShowActivePlayer, NoDataFound, Saved, Loaded;
        private (int, int) canvas_indexes;
        private Ellipse last_added;
        public GameAttributes Game;
        private bool single_player;
        private Computer_Player Comp_player;
        private bool computerTurnInProcess;
        private GameSetter gameSetter;
        private Leaderboard leaderboard;
        public ViewModel()
        {
            Visibility = "Hidden";
            gameSetter = new GameSetter();
        }
        public void Create_GameAttributes(int h, int w, bool single_player, string name1, string name2)
        {
            Game = gameSetter.MakeGame(name1, name2, false, new NewGame(h, w));
            Display_Init_Gameset();
            this.single_player = single_player;
            computerTurnInProcess = false;
            if (single_player)
                Comp_player = new Computer_Player(Game.Player2);
            ShowActivePlayer.Invoke();
            ShowLeader();
        }

        private void Display_Init_Gameset()
        {
            int h = Game.Placed_Disks.Placed_disks.GetLength(0);
            int w = Game.Placed_Disks.Placed_disks.GetLength(1);
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    if (Game.Placed_Disks.Placed_disks[i, j] != null)
                        AddDiskToField.Invoke((i, j), Game.Placed_Disks.Placed_disks[i, j].shape);
        }

        public void Process_Click(System.Windows.Point p)
        {
            if (!computerTurnInProcess)
            {
                int col = (int)Math.Floor(p.X / 20);
                int row = (int)Math.Floor(p.Y / 20);
                try
                {
                    Game.Add_Disk(row, col);
                    canvas_indexes = (row, col);
                    last_added = Game.Placed_Disks.Placed_disks[row, col].shape;
                    ShowActivePlayer.Invoke();
                }
                catch
                {
                    canvas_indexes = (-1, -1);
                    last_added = null;
                }
                AddDiskToField.Invoke(canvas_indexes, last_added);
                if (!Game.gameFinished)
                {
                    if (single_player && last_added != null)
                        Make_Computers_Turn();
                }
                else
                {
                    ProcessEndGame();
                }
                }
            }
        private async void Make_Computers_Turn()
        {
            computerTurnInProcess = true;
            await Task.Delay(500);
            (canvas_indexes, last_added) = Comp_player.Computers_Turn(Game);
            if (last_added == null || Game.gameFinished)
                ProcessEndGame();
            else
            {
                ShowActivePlayer.Invoke();
                AddDiskToField.Invoke(canvas_indexes, last_added);
            }
            computerTurnInProcess = false;
            
        }
        private void ProcessEndGame()
        {
            AddLeader();
            EndGame.Invoke();
            Visibility = "Hidden";
        }
        private void SaveGame()
        {
            int h = Game.Placed_Disks.Placed_disks.GetLength(0);
            int w = Game.Placed_Disks.Placed_disks.GetLength(1);
            FileOperator.SaveGame(h, w, Game.DisksToInt(), Game.first_player_next, single_player, Game.Player1.Player_name, Game.Player2.Player_name);
        }
        private void LoadGame()
        {
            FileOperator.GameData gd = FileOperator.LoadGame();
            if (gd == null)
            {
                NoDataFound.Invoke();
            }
            else
            {
                Game = gameSetter.MakeGame(gd.Name1, gd.Name2, gd.FirstPlayerNext, new LoadGame(gd.Height, gd.Width, gd.Field));
                this.single_player = gd.IsSinglePlayer;
                computerTurnInProcess = false;
                if (single_player)
                    Comp_player = new Computer_Player(Game.Player2);
                ShowActivePlayer.Invoke();
            }
        }
        private void ShowLeader()
        {
            leaderboard = new Leaderboard();
            leaderboard.Import();
            LeaderInfo = leaderboard.Find(Game.Placed_Disks.Placed_disks.GetLength(0), Game.Placed_Disks.Placed_disks.GetLength(1));
        }
        private void AddLeader()
        {
            Player win = Game.Winner();
            leaderboard.NewLead(Game.Placed_Disks.Placed_disks.GetLength(0), Game.Placed_Disks.Placed_disks.GetLength(1), win.Player_name, win.Score);
        }
        private Command startGame;
        public Command StartGame
        {
            get
            {
                return startGame ?? (startGame = new Command(obj =>
                {
                    try
                    {
                        int h = int.Parse(FieldHeight);
                        int w = int.Parse(FieldWidth);
                        if (h < 8 || h > 20 || w < 8 || w > 20)
                            IncorrectInput.Invoke();
                        else
                        {
                            StartNewGame.Invoke();
                            Visibility = "Visible";
                            Pl1Name = "";
                            Pl2Name = "";
                        }
                        }
                    catch
                    {
                        IncorrectInput.Invoke();
                    }
                    FieldHeight = "";
                    FieldWidth = "";
                    
                    
                }));
            }
        }
        private Command load;
        public Command Load
        {
            get
            {
                return load ?? (load = new Command(obj =>
                {
                    LoadGame();
                    if (Game != null)
                    {
                        Visibility = "Visible";
                        Loaded.Invoke();
                        Display_Init_Gameset();
                        ShowLeader();
                    }
                }));
            }
        }
        private Command save;
        public Command Save
        {
            get
            {
                return save ?? (save = new Command(obj =>
                {
                    SaveGame();
                    Saved.Invoke();
                }));
            }
        }
        private string fieldHeight;
        public string FieldHeight
        {
            get
            {
                return this.fieldHeight;
            }
            set
            {
                fieldHeight = value;
                OnPropertyChanged();
            }
        }
        private string fieldWidth;
        public string FieldWidth
        {
            get
            {
                return this.fieldWidth;
            }
            set
            {
                fieldWidth = value;
                OnPropertyChanged();
            }
        }
        private string visibility;
        public string Visibility
        {
            get
            {
                return this.visibility;
            }
            set
            {
                visibility = value;
                OnPropertyChanged();
            }
        }

        private string currentName;
        public string CurrentName
        {
            get
            {
                return this.currentName;
            }
            set
            {
                currentName = value;
                OnPropertyChanged();
            }
        }
        private string pl1Name;
        public string Pl1Name
        {
            get
            {
                return this.pl1Name;
            }
            set
            {
                pl1Name = value;
                OnPropertyChanged();
            }
        }
        private string pl2Name;
        public string Pl2Name
        {
            get
            {
                return this.pl2Name;
            }
            set
            {
                pl2Name = value;
                OnPropertyChanged();
            }
        }
        private string leaderInfo;
        public string LeaderInfo
        {
            get
            {
                return this.leaderInfo;
            }
            set
            {
                leaderInfo = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
