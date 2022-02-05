using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.game
{
    internal class Solver1
    {
        Tuple<List<List<int>>, List<List<int>>> Solve(List<List<int>> playBoard)
        {
            int flagCount = 0;
            int unknownCount = 0;

            //results
            List<List<int>> clickSquares = new List<List<int>>();
            List<List<int>> flagSquares = new List<List<int>>();

            int[] foundFlags = new int[playBoard.Count];
            int[] foundUnknown = new int[playBoard.Count];

            

            for (int i = 0; i < playBoard.Count; i++)
            {
                for (int j = 0; j < playBoard.Count; j++)
                {
                    if (playBoard[i][j] > 0)
                    {
                        int cellValue = playBoard[i][j];

                        int top = i - 1;
                        int bot = i + 1;
                        int left = j - 1;
                        int right = j + 1;
                        if (top < 0)
                            top = 0;
                        if (bot > playBoard.Count - 1)
                            bot = playBoard.Count - 1;
                        if (left < 0)
                            left = 0;
                        if (right > playBoard.Count - 1)
                            right = playBoard.Count - 1;
                        for (int a = top; a <= bot; a++)
                        {
                            for (int b = left; b <= right; b++)
                            {
                                bool found = false;
                                flagCount = 0;
                                unknownCount = 0;
                                if (playBoard[a][b] == -4)
                                {
                                    flagCount++;
                                }
                                else if (playBoard[a][b] == -2)
                                {
                                    unknownCount++;
                                }
                            }
                        }
                        if (flagCount < cellValue)
                        {
                            if (unknownCount + flagCount == cellValue)
                            {
                                top = i - 1;
                                bot = i + 1;
                                left = j - 1;
                                right = j + 1;
                                if (top < 0)
                                    top = 0;
                                if (bot > playBoard.Count - 1)
                                    bot = playBoard.Count - 1;
                                if (left < 0)
                                    left = 0;
                                if (right > playBoard.Count - 1)
                                    right = playBoard.Count - 1;
                                for (int a = top; a <= bot; a++)
                                {
                                    for (int b = left; b <= right; b++)
                                    {
                                        if (playBoard[a][b] == -2)
                                            flagSquares.Add(new List<int>() { a, b });
                                    }
                                }
                            }
                        }
                        else if (flagCount == cellValue)
                        {
                            if (unknownCount + flagCount == cellValue)
                            {
                                top = i - 1;
                                bot = i + 1;
                                left = j - 1;
                                right = j + 1;
                                if (top < 0)
                                    top = 0;
                                if (bot > playBoard.Count - 1)
                                    bot = playBoard.Count - 1;
                                if (left < 0)
                                    left = 0;
                                if (right > playBoard.Count - 1)
                                    right = playBoard.Count - 1;
                                for (int a = top; a <= bot; a++)
                                {
                                    for (int b = left; b <= right; b++)
                                    {
                                        if (playBoard[a][b] == -2)
                                            clickSquares.Add(new List<int>() { a, b });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (flagSquares.Count == 0 && clickSquares.Count == 0)
            {
                Random rand = new Random(Guid.NewGuid().GetHashCode());
                List<List<int>> candidates = new List<List<int>>();
                for(int i = 0; i < playBoard.Count; i++)
                {
                    for(int j = 0; j < playBoard.Count; j++)
                    {
                        if(hasNeighbour(playBoard, i, j))
                        {
                            candidates.Add(new List<int>() { i, j });
                        }
                    }
                }
                clickSquares.Add(candidates[rand.Next(0, candidates.Count)]);
            }

            return (new Tuple<List<List<int>>, List<List<int>>>(clickSquares, flagSquares));
        }

        static bool hasNeighbour(List<List<int>> playBoard, int x, int y)
        {
            if (x > 0)
                if (playBoard[x - 1][y] != -2)
                    return true;
            if (x < playBoard.Count - 1)
                if (playBoard[x + 1][y] != -2)
                    return true;

            if (y > 0)
                if (playBoard[x][y - 1] != -2)
                    return true;
            if (y < playBoard.Count - 1)
                if (playBoard[x][y + 1] != -2)
                    return true;

            return false;
        }
    }
}
