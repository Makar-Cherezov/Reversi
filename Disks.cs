using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Reversi
{
    public class Disks
    {
        public class Disk
        {
            public Ellipse shape;
            private int grid_size = 20;
            public int OwnerID { get; set; }
            public Disk(SolidColorBrush brush, int id)
            {
                OwnerID = id;
                shape = new Ellipse
                {
                    Width = grid_size,
                    Height = grid_size,
                    Fill = brush
                };
            }
        }

        public Disk[,] Placed_disks;
        public Disks(int height, int width)
        {
            Placed_disks = new Disk[height, width];
        }
        private bool Is_In_Borders(int c, int r)
        {
            if (c < 0 || r < 0 || r >= Placed_disks.GetLength(0) || c >= Placed_disks.GetLength(1))
                return false;
            return true;
        }
        private bool Has_Neighbours(int r, int c)
        {
            for (int i = r - 1; i < r + 2; i++)
                for (int j = c - 1; j < c + 2; j++)
                {
                    if (Is_In_Borders(i, j))
                        if (Placed_disks[i, j] != null)
                            return true;
                }
            return false;
        }
        public void Add_Disk(int i, int j, Player ActivePlayer)
        {
            if (Has_Neighbours(i, j) && Placed_disks[i, j] == null)
            {
                Placed_disks[i, j] = new Disk(ActivePlayer.Players_brush, ActivePlayer.ID);
                Flip_Disks(i, j, ActivePlayer);
            }
            else
                throw new NotImplementedException();
        }

        public (int, int) CalcScore()
        {
            int pl1Score = 0, pl2Score = 0;
            for (int i = 0; i < Placed_disks.GetLength(0); i++)
                for (int j = 0; j < Placed_disks.GetLength(1); j++)
                    if (Placed_disks[i, j] != null)
                        if (Placed_disks[i, j].OwnerID == 1)
                            pl1Score++;
                        else
                            pl2Score++;
            return (pl1Score, pl2Score);
        }

        private void Flip_Disks(int r, int c, Player ActivePlayer)
        {
            (int, int)[] directions = { (1, 0), (1, 1), (0, 1), (-1, 0), (-1, -1), (0, -1), (1, -1), (-1, 1) };
            int Act_ID = ActivePlayer.ID;
            SolidColorBrush brush = ActivePlayer.Players_brush;
            int count, i, j;
            foreach (var dir in directions)
            {
                count = 0;
                i = r + dir.Item1;
                j = c + dir.Item2;
                while (Is_In_Borders(i, j))
                {
                    if (Placed_disks[i, j] != null)
                    {
                        if (Placed_disks[i, j].OwnerID == Act_ID)
                        {
                            Reverse_Disks(r, c, count, dir, Act_ID, brush);
                            break;
                        }
                    }
                    else
                        break;
                    i += dir.Item1;
                    j += dir.Item2;
                    count++;
                }
            }
        }
        private void Reverse_Disks(int r, int c, int count, (int, int) dir, int Act_ID, SolidColorBrush brush)
        {
            for (int i = 0; i < count; i++)
            {
                r += dir.Item1;
                c += dir.Item2;
                Placed_disks[r, c].OwnerID = Act_ID;
                Placed_disks[r, c].shape.Fill = brush;
            }
        }


    }
}
