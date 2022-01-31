﻿using System;
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
using System.Threading;
using Microsoft.Win32;

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
        List<List<int>> playBoard = new List<List<int>>();
        Game game;
        int gameState = 0;
        Thread aiThread = new Thread(() => { });
        string filePath = "";
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
            cbxDifficulty.Items.Add("Easy");
            cbxDifficulty.Items.Add("Medium");
            cbxDifficulty.Items.Add("Hard");
            cbxDifficulty.Items.Add("Destytojas");
            cbxDifficulty.SelectedIndex = 0;
            rbtnRandom.IsChecked = true;
            NewGame();
            board.IsEnabled = false;
        }

        public void NewGame()
        {
            Image image = new Image();
            image.Source = smiles[0];
            btnSmile.Content = image;

            if (rbtnRandom.IsChecked == true)
            {
                if (cbxDifficulty.SelectedIndex == 0)
                    game = new Game(10, 11);
                else if (cbxDifficulty.SelectedIndex == 1)
                    game = new Game(16, 50);
                else if (cbxDifficulty.SelectedIndex == 2)
                    game = new Game(22, 99);
                else
                    game = new Game(10, -1);
            }
            else
                game = new Game(filePath);

            playBoard = game.GetPlayBoard();
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
                            boardButtons[i][j].Source = images[0];
                        }
                    }
                }
                board.IsEnabled = false;
            }
        }

        private void btn_LeftClick(object sender, EventArgs e, int i, int j)
        {
            Dispatcher.Invoke(() =>
            {
                game.Move(i, j);
                DrawBoard();
            });
        }

        private void btn_RightClick(object sender, EventArgs e, int i, int j)
        {
            Dispatcher.Invoke(() =>
            {
                game.Flag(i, j);
                DrawBoard();
            });
        }

        private void btnSmile_Click(object sender, RoutedEventArgs e)
        {
            aiThread.Abort();
            topPnlGame.Visibility = Visibility.Collapsed;
            topPnlSelection.Visibility = Visibility.Visible;
            board.IsEnabled = false;
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

        private void btnManual_Click(object sender, RoutedEventArgs e)
        {
            topPnlGame.Visibility = Visibility.Visible;
            topPnlSelection.Visibility = Visibility.Collapsed;
            NewGame();
        }

        private void btnAlgorithm1_Click(object sender, RoutedEventArgs e)
        {
            int number = -1;
            bool success = int.TryParse(tbxDelay.Text, out number);
            if (success && number >= 0)
            {
                topPnlGame.Visibility = Visibility.Visible;
                topPnlSelection.Visibility = Visibility.Collapsed;
                NewGame();
                board.IsEnabled = false;
                //Random rand = new Random();
                aiThread = new Thread(() =>
                {
                    /*
                    int size = boardButtons.Count;
                    while (gameState == 0)
                    {
                        int i = rand.Next(size);
                        int j = rand.Next(size);
                        while (playBoard[i][j] != -2)
                        {
                            i = rand.Next(size);
                            j = rand.Next(size);
                        }
                        btn_LeftClick(null, null, i, j);
                        Thread.Sleep(number);
                    }*/
                    //ai integration
                });
                aiThread.IsBackground = true;
                aiThread.Start();
            }
            else
                MessageBox.Show("Delay has to be a nonnegative integer");
        }

        private void btnAlgorithm2_Click(object sender, RoutedEventArgs e)
        {
            int number = -1;
            bool success = int.TryParse(tbxDelay.Text, out number);
            if (success && number >= 0)
            {
                topPnlGame.Visibility = Visibility.Visible;
                topPnlSelection.Visibility = Visibility.Collapsed;
                NewGame();
                board.IsEnabled = false;
                aiThread = new Thread(() =>
                {
                    //ai integration
                });
                aiThread.IsBackground = true;
                aiThread.Start();
            }
            else
                MessageBox.Show("Delay has to be a nonnegative integer");
        }

        private void rbtnRandom_Checked(object sender, RoutedEventArgs e)
        {
            spnlCustom.Visibility = Visibility.Collapsed;
            spnlRandom.Visibility = Visibility.Visible;
            btnManual.IsEnabled = true;
            btnAlgorithm1.IsEnabled = true;
            btnAlgorithm2.IsEnabled = true;
        }

        private void rbtnImport_Checked(object sender, RoutedEventArgs e)
        {
            spnlRandom.Visibility = Visibility.Collapsed;
            spnlCustom.Visibility = Visibility.Visible;
            if (filePath == "")
            {
                btnManual.IsEnabled = false;
                btnAlgorithm1.IsEnabled = false;
                btnAlgorithm2.IsEnabled = false;
            }
            else
            {
                btnManual.IsEnabled = true;
                btnAlgorithm1.IsEnabled = true;
                btnAlgorithm2.IsEnabled = true;
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "TXT|*.txt";
            if (dialog.ShowDialog() == true)
            {
                filePath = System.IO.Path.GetFullPath(dialog.FileName);
                if (ValidateFile(filePath))
                {
                    tbxFileName.Text = System.IO.Path.GetFileName(filePath);
                    btnManual.IsEnabled = true;
                    btnAlgorithm1.IsEnabled = true;
                    btnAlgorithm2.IsEnabled = true;
                }
                else
                    MessageBox.Show("File is not valid");
            }
        }

        private bool ValidateFile(string path)
        {
            bool valid = true;
            using (StreamReader file = new StreamReader(path))
            {
                int bombs = 0;
                int counter = 0;
                List<int> lineLengths = new List<int>();
                string ln;

                while ((ln = file.ReadLine()) != null && valid)
                {
                    List<string> line = ln.Split(' ').ToList();
                    lineLengths.Add(line.Count);
                    foreach(string s in line)
                    {
                        int number = -10;
                        bool success = int.TryParse(s, out number);
                        if (!success || (number != -1 && number != 0))
                        {
                            valid = false;
                            break;
                        }
                        if (number == -1)
                            bombs++;
                    }
                    counter++;
                }
                foreach(int i in lineLengths)
                {
                    if(i != counter)
                    {
                        valid = false;
                        break;
                    }
                }
                if (bombs == 0)
                    valid = false;
                file.Close();
            }
            return valid;
        }
    }
}
