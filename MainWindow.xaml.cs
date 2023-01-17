using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reversi
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
            ((ViewModel)  DataContext).AddDiskToField += RenewGameArea;
            ((ViewModel) DataContext).ShowActivePlayer += ChangeCurrentInfo;
            ((ViewModel) DataContext).NoDataFound += NoDataMessage;
            ((ViewModel)DataContext).Saved += Save_Click;
            ((ViewModel)DataContext).Loaded += Load_Click;
            SecondPlayerInfo.Visibility = System.Windows.Visibility.Hidden;
        }
        private void NoDataMessage()
        {
            MessageBox.Show("Сохранённая игра не найдена!");
        }
        private void EndGame()
        {
            int Score1 = ((ViewModel) DataContext).Game.Player1.Score;
            int Score2 = ((ViewModel) DataContext).Game.Player2.Score;
            string name1 = ((ViewModel) DataContext).Game.Player1.Player_name;
            string name2 = ((ViewModel) DataContext).Game.Player2.Player_name;
            string winner = "- Дружба";
            CurrentInfo.Visibility = System.Windows.Visibility.Hidden;
            if (Score1 != Score2)
                winner = Score1 > Score2 ? "- " + name1 : "- " + name2;
            MessageBox.Show($"Конец игры! \n Счёт игрока {name1}: {Score1}" +
                $"\n Счёт игрока {name2}: {Score2}" +
                $"\n Победитель {winner}!" +
                $"\n Начать новую игру?");
            GameArea.Children.Clear();
            
        }
        private void ChangeCurrentInfo()
        {
            Player player = ((ViewModel) DataContext).Game.ActivePlayer;
            CurrentPlayerName.Content = player.Player_name;
            CurrentPlayerBrush.Fill = player.Players_brush;
            Score.Content = player.Score;
        }
        private void DrawGameArea(int field_height, int field_width, int side = 20)
        {
            bool doneDrawingBackground = false;
            int nextX = 0, nextY = 0;
            int rowCounter = 0;
            int colCounter = 0;
            bool nextIsOdd = false;
            while (doneDrawingBackground == false)
            {
                Rectangle rect = new Rectangle
                {
                    Width = side,
                    Height = side,
                    Fill = nextIsOdd ? Brushes.Silver : Brushes.DarkGray
                };
                GameArea.Children.Add(rect);
                Canvas.SetTop(rect, nextY);
                Canvas.SetLeft(rect, nextX);
                Canvas.SetZIndex(rect, 0);
                nextIsOdd = !nextIsOdd;
                nextX += side;
                colCounter++;
                if (nextX > GameArea.ActualWidth || colCounter == field_width)
                {
                    nextX = 0;
                    nextY += side;
                    rowCounter++;
                    colCounter = 0;
                    nextIsOdd = rowCounter % 2 != 0;
                }

                if (nextY > GameArea.ActualHeight || rowCounter == field_height)
                    doneDrawingBackground = true;
            }
        }
        private void RenewGameArea((int, int) iter, Ellipse disk)
        {
            if (disk == null)
                MessageBox.Show("Диски можно размещать только рядом с другими дисками!");
            else
            {
                GameArea.Children.Add(disk);
                Canvas.SetTop(disk, iter.Item1 * 20); 
                Canvas.SetLeft(disk, iter.Item2 * 20);
                Canvas.SetZIndex(disk, 1);
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            GameArea.Children.Clear();
            int width, height;
            try
            {
                width = int.Parse(fieldWidth.Text);
                height = int.Parse(fieldHeight.Text);
                DrawGameArea(height, width);
                ((ViewModel) DataContext).Create_GameAttributes(height, width, (bool)One.IsChecked, Pl1Name.Text, Pl2Name.Text);
                ((ViewModel) DataContext).EndGame += EndGame;
            }
            catch
            {
                MessageBox.Show("Введите числа!");
                
            }
        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(GameArea);
            ((ViewModel) DataContext).Process_Click(p);
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if ((bool)One.IsChecked)
                SecondPlayerInfo.Visibility = System.Windows.Visibility.Hidden;
            else
                SecondPlayerInfo.Visibility = System.Windows.Visibility.Visible;
        }

        private void Save_Click()
        {
            MessageBox.Show("Игра сохранена.");
        }
        private void Load_Click()
        {
            GameArea.Children.Clear();
            ((ViewModel) DataContext).EndGame += EndGame;
            int h = ((ViewModel)DataContext).Game.Placed_Disks.Placed_disks.GetLength(0);
            int w = ((ViewModel)DataContext).Game.Placed_Disks.Placed_disks.GetLength(1);
            DrawGameArea(h, w);
        }
    }
}

