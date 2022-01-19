using System;
using System.Collections.Generic;
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
using Minesweeper.game;
using System.IO;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<BitmapImage> images = new List<BitmapImage>();
        List<BitmapImage> smiles = new List<BitmapImage>();
        List<List<Image>> boardButtons;
        Game game;
        int gameState = 0;
        public MainWindow()
        {
            InitializeComponent();
            string[] files = Directory.GetFiles("images/tiles");
            foreach (string s in files)
            {
                images.Add(new BitmapImage(new Uri(System.IO.Path.GetFullPath(s), UriKind.Absolute)));
            }
            files = Directory.GetFiles("images/smiles");
            foreach (string s in files)
            {
                smiles.Add(new BitmapImage(new Uri(System.IO.Path.GetFullPath(s), UriKind.Absolute)));
            }
            NewGame();
        }

        public void NewGame()
        {
            Image image = new Image();
            image.Source = smiles[0];
            btnSmile.Content = image;
            game = new Game(20, 50);

            List<List<int>> playBoard = game.GetPlayBoard();
            gameState = game.GetGameState();
            int size = playBoard.Count;
            boardButtons = new List<List<Image>>();
            board.RowDefinitions.Clear();
            board.ColumnDefinitions.Clear();
            board.IsEnabled = true;

            for (int i = 0; i < size; i++)
            {
                List<Image> buttons = new List<Image>();
                board.RowDefinitions.Add(new RowDefinition());
                board.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < size; j++)
                {
                    image = new Image();
                    image.SetValue(Grid.RowProperty, i);
                    image.SetValue(Grid.ColumnProperty, j);
                    image.Source = images[2];

                    int r = i;
                    int c = j;
                    image.MouseLeftButtonUp += (snder, EventArgs) => { btn_LeftClick(snder, EventArgs, r, c); };
                    image.MouseRightButtonUp += (snder, EventArgs) => { btn_RightClick(snder, EventArgs, r, c); };
                    buttons.Add(image);
                    board.Children.Add(image);
                }
                boardButtons.Add(buttons);
            }

            //drawBoard();
        }

        private void DrawBoard()
        {
            List<List<int>> playBoard = game.GetPlayBoard();
            gameState = game.GetGameState();
            int size = playBoard.Count;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    boardButtons[i][j].Source = images[playBoard[i][j] + 4];
                }
            }

            if (gameState == -1)
            {
                Image image = new Image();
                image.Source = smiles[2];
                btnSmile.Content = image;
                board.IsEnabled = false;
            }
            else if (gameState == 1)
            {
                Image image = new Image();
                image.Source = smiles[1];
                btnSmile.Content = image;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (playBoard[i][j] == -2)
                        {
                            image = new Image();
                            image.Source = images[0];
                            boardButtons[i][j] = image;
                        }
                    }
                }
                board.IsEnabled = false;
            }
        }

        private void btn_LeftClick(object sender, EventArgs e, int i, int j)
        {
            game.Move(i, j);
            DrawBoard();
        }

        private void btn_RightClick(object sender, EventArgs e, int i, int j)
        {
            game.Flag(i, j);
            DrawBoard();
        }

        private void btnSmile_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
