using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using static Reversi.Disks;

namespace Reversi
{
    public class Computer_Player
    {
        private bool added_successfully;
        private Player player;
        private Ellipse added_disk;
        private (int, int) canva_ind;
        public Computer_Player(Player pl)
        {
            player = pl;
        }
        public ((int, int), Ellipse) Computers_Turn(GameAttributes game) 
        {
            added_successfully = false;
            Random rand = new Random();
            int rnd = rand.Next(0, 4);
            (int, int)[] directions = { (0, 1), (-1, 0), (0, -1), (1, 0) };
            (int, int) dir = directions[rnd];
            Disk[,] disks = game.Placed_Disks.Placed_disks;
            int rows = disks.GetLength(0);
            int cols = disks.GetLength(1);
            PlaceDisk(disks, rows, cols, dir, game);
            if (added_successfully)
            {
                return (canva_ind, added_disk);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                    if (i != rnd)
                    {
                        PlaceDisk(disks, rows, cols, directions[i], game);
                        if (added_successfully)
                            return (canva_ind, added_disk);
                    }
            }
            return ((-1, -1), null);
            
        }
        
        private void PlaceDisk(Disk[,] disks, int rows, int cols, (int, int) dir, GameAttributes game)
        {
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (Is_In_Borders(i + dir.Item1, j + dir.Item2, disks))
                        if (disks[i, j] == null && disks[i + dir.Item1, j + dir.Item2] != null)
                            if (disks[i + dir.Item1, j + dir.Item2].OwnerID != player.ID)
                            {
                                game.Add_Disk(i, j);
                                canva_ind = (i, j);
                                added_disk = game.Placed_Disks.Placed_disks[i, j].shape;
                                added_successfully = true;
                                return;
                            }
        }
        
        private bool Is_In_Borders(int c, int r , Disk[,] disks)
        {
            if (c < 0 || r < 0 || r >= disks.GetLength(0) || c >= disks.GetLength(1))
                return false;
            return true;
        }
    }
}
