using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Reversi
{
    public interface ISetGame
    {
        Disks CreateGameField();
    }
    public class NewGame : ISetGame
    {
        int height, width;

        
        public NewGame(int h, int w)
        {
            height = h;
            width = w;
        }
        public Disks CreateGameField()
        {
            Disks disks = new Disks(height, width);
            Set_Init_Disks(disks);
            return disks;
        }
        private void Set_Init_Disks(Disks disks)
        {
            Disks.Disk[,] Placed_disks = disks.Placed_disks;
            int row = Placed_disks.GetLength(0) - 1;
            int col = Placed_disks.GetLength(1) - 1;
            if (row * col % 4 == 0)
                (row, col) = (row / 2, col / 2);
            else
                (row, col) = ((int)Math.Floor((double)row / 2), (int)Math.Floor((double)col / 2));
            int i_border = (int)Math.Floor((double)Placed_disks.GetLength(0) / 8);
            int j_border = ((int)Math.Floor((double)Placed_disks.GetLength(1) / 8));
            for (int i = 0; i <= i_border; i++)
                for (int j = 0; j <= j_border; j += 2)
                {
                    Placed_disks[row + i, col + i + j] = new Disks.Disk(Brushes.Black, 1);
                    Placed_disks[row + i, col - i + j + 1] = new Disks.Disk(Brushes.White, 2);
                }
        }
    }
    public class LoadGame : ISetGame
    {
        int height, width;
        int[] fromJson;
        public LoadGame(int h, int w, int[] arr)
        {
            height = h;
            width = w;
            fromJson = arr;
        }
        public Disks CreateGameField()
        {
            Disks disks = new Disks(height, width);
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    switch (fromJson[i * width + j])
                    {
                        case 0:
                            disks.Placed_disks[i, j] = null;
                            break;
                        case 1:
                            disks.Placed_disks[i, j] = new Disks.Disk(Brushes.Black, 1);
                            break;
                        case 2:
                            disks.Placed_disks[i, j] = new Disks.Disk(Brushes.White, 2);
                            break;
                    }

            return disks;
        }
    }

    public class GameSetter
    {
        public GameAttributes MakeGame(string name1, string name2, bool fpnext, ISetGame setter)
        {
            if (name1 == "")
                name1 = "Игрок 1";
            Player Player1 = new Player(name1, Brushes.Black, 1);
            if (name2 == "")
                name2 = "Фортуна";
            Player Player2 = new Player(name2, Brushes.White, 2);
            Disks placedDisks = setter.CreateGameField();
            GameAttributes g = new GameAttributes(Player1, Player2, fpnext, placedDisks);
            g.CalculateScore();
            return g;
        }

            
    }
    public class GameAttributes
    {
        
        public Player ActivePlayer;
        public Player Player1;
        public Player Player2;
        public Disks Placed_Disks;
        public bool first_player_next;
        public bool gameFinished = false;
        public GameAttributes(Player p1, Player p2, bool firstPlayer, Disks disks)
        {
            Player1 = p1;
            Player2 = p2;
            first_player_next = firstPlayer;
            Placed_Disks = disks;
            ActivePlayer = first_player_next ? Player2 : Player1;
        }
        public void SetActivePlayer()
        {
            ActivePlayer = first_player_next ? Player1 : Player2;
            first_player_next = !first_player_next;
        }
        public void Add_Disk(int r, int c)
        {
            Placed_Disks.Add_Disk(r, c, ActivePlayer);
            SetActivePlayer();
            CalculateScore();
            if (Player1.Score + Player2.Score == Placed_Disks.Placed_disks.Length)
            {
                gameFinished = true;
            }
        }
        public void CalculateScore()
        {
            (Player1.Score, Player2.Score) = Placed_Disks.CalcScore();
        }
        public int[] DisksToInt()
        {
            Disks.Disk[,] disks = Placed_Disks.Placed_disks;
            int h = disks.GetLength(0);
            int w = disks.GetLength(1);
            int[,] toInt = new int[h, w];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                {
                    if (disks[i, j] == null)
                        toInt[i, j] = 0;
                    else
                        toInt[i, j] = disks[i, j].OwnerID;
                }
            return toInt.Cast<int>().ToArray();
        }
    }

}