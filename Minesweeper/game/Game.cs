using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Minesweeper.game
{
    class Game
    {
        int size;
        int mineCount;
        int gameState;
        Random rand = new Random();
        List<List<int>> fullBoard = new List<List<int>>();
        List<List<int>> playBoard = new List<List<int>>();
        public Game(int size, int mineCount)
        {
            this.size = size;
            this.mineCount = mineCount;
            gameState = 0;
            newBoard();
        }

        private void newBoard()
        {
            for (int i = 0; i < size; i++)
            {
                List<int> fullRow = new List<int>();
                List<int> playRow = new List<int>();
                for (int j = 0; j < size; j++)
                {
                    fullRow.Add(0);
                    playRow.Add(-2);
                }
                fullBoard.Add(fullRow);
                playBoard.Add(playRow);
            }
        }

        private void calculateNeighbours()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (fullBoard[i][j] != -1)
                    {
                        int top = i - 1;
                        int bot = i + 1;
                        int left = j - 1;
                        int right = j + 1;
                        if (top < 0)
                            top = 0;
                        if (bot > size - 1)
                            bot = size - 1;
                        if (left < 0)
                            left = 0;
                        if (right > size - 1)
                            right = size - 1;
                        for (int a = top; a <= bot; a++)
                        {
                            for (int b = left; b <= right; b++)
                            {
                                if (fullBoard[a][b] == -1)
                                    fullBoard[i][j]++;
                            }
                        }
                    }
                }
            }
        }

        private void layMines(int x, int y)
        {
            int currmines = mineCount;
            while (currmines > 0)
            {
                int a = rand.Next(size);
                int b = rand.Next(size);
                if (fullBoard[a][b] != -1 && x != a && y != b)
                {
                    fullBoard[a][b] = -1;
                    currmines--;
                }
            }

            calculateNeighbours();
        }

        public Tuple<List<List<int>>, int> getPlayBoard()
        {
            return Tuple.Create(playBoard, gameState);
        }

        public void move(int x, int y)
        {
            if (playBoard[x][y] == -4)
                return;
            bool contains = false;
            foreach (List<int> row in fullBoard)
            {
                if (row.Contains(-1))
                {
                    contains = true;
                    break;
                }
            }

            if (!contains)
                layMines(x, y);

            if (fullBoard[x][y] != -1)
            {
                floodFill(x, y);
                int squaresLeft = 0;
                foreach (List<int> row in playBoard)
                    foreach (int i in row)
                        if(i != -2 && i != -4)
                            squaresLeft++;
                if (squaresLeft == size*size - mineCount)
                    gameState = 1;
                Console.WriteLine(squaresLeft);
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (fullBoard[i][j] == -1)
                            playBoard[i][j] = fullBoard[i][j];
                    }
                }
                gameState = -1;
                playBoard[x][y] = -3;
            }
        }

        private void floodFill(int x, int y)
        {
            if (x < 0 || x > size - 1)
                return;
            if (y < 0 || y > size - 1)
                return;
            if (playBoard[x][y] != -2)
                return;
            playBoard[x][y] = fullBoard[x][y];
            if (fullBoard[x][y] == 0)
            {
                floodFill(x + 1, y);
                floodFill(x, y + 1);
                floodFill(x - 1, y);
                floodFill(x, y - 1);

                floodFill(x + 1, y + 1);
                floodFill(x + 1, y - 1);
                floodFill(x - 1, y + 1);
                floodFill(x - 1, y - 1);
            }
        }

        public void flag(int x, int y)
        {
            if(playBoard[x][y] == -2)
            {
                playBoard[x][y] = -4;
            }
            else if(playBoard[x][y] == -4)
            {
                playBoard[x][y] = -2;
            }
        }
    }
}
