namespace Minesweeper.game {
    class Solver2 {
        static readonly int[] dx = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
        static readonly int[] dy = { 1, 1, 1, 0, 0, 0, -1, -1, -1 };

        public static bool IsDone(
                int[,] board,
                Stack<Coordinates> candidates
                ){
            int xMax = board.GetLength(0);
            int yMax = board.GetLength(1);

            for(int x = 0; x < xMax; x++) {
                for(int y = 0; y < yMax; y++) {
                    for(int i = 0; i < 9; i++) {
                        if(x + dx[i] >= 0 &&
                                y + dy[i] >= 0 &&
                                x + dx[i] < xMax &&
                                y + dy[i] < yMax &&
                                (board[x, y] == -2) &&
                                (board[x + dx[i], y + dy[i]] > 0)
                                ){
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static bool IsSafe(
                int[,] board,
                Coordinates coord
                ) {
            int xMax = board.GetLength(0);
            int yMax = board.GetLength(1);

            for(int i = 0; i < 9; i++) {
                if(coord.x + dx[i] >= 0 &&
                        coord.y + dy[i] >= 0 &&
                        coord.x + dx[i] < xMax &&
                        coord.y + dy[i] < yMax &&
                        (board[coord.x + dx[i], coord.y + dy[i]] - 1 == -1)
                        ){
                    return false;
                }
            }
            return true;
        }

        public static bool FindSolution(
                int[,] board,
                ref bool[,] mines,
                Stack<Coordinates> candidates,
                ref int[,] probability,
                ref int numberOfSolutions
                ) {
            int xMax = board.GetLength(0);
            int yMax = board.GetLength(1);

            if(IsDone(board, candidates)){
                for(int x = 0; x < xMax; x++) {
                    for(int y = 0; y < yMax; y++) {
                        if(mines[x, y] == true) {
                            probability[x, y]++;
                        }
                    }
                }
                numberOfSolutions++;
                return false;
            };

            if(candidates.Count() == 0) return false;

            Coordinates coord = candidates.Pop();

            if(IsSafe(board, coord)) {
                // Lower Neighbours
                for(int i = 0; i < 9; i++) {
                    if(coord.x + dx[i] >= 0 &&
                            coord.y + dy[i] >= 0 &&
                            coord.x + dx[i] < xMax &&
                            coord.y + dy[i] < yMax &&
                            (board[coord.x + dx[i], coord.y + dy[i]] > 0)
                            ){
                        board[coord.x + dx[i], coord.y + dy[i]] -= 1;
                    }
                }

                mines[coord.x, coord.y] = true;
                if(FindSolution(board, ref mines, candidates, ref probability, ref numberOfSolutions)) return true;
                mines[coord.x, coord.y] = false;

                // Reset Neighbours
                for(int i = 0; i < 9; i++) {
                    if(coord.x + dx[i] >= 0 &&
                            coord.y + dy[i] >= 0 &&
                            coord.x + dx[i] < xMax &&
                            coord.y + dy[i] < yMax &&
                            (board[coord.x + dx[i], coord.y + dy[i]] >= 0)
                            ){
                        board[coord.x + dx[i], coord.y + dy[i]] += 1;
                    }
                }
            }
            if(FindSolution(board, ref mines, candidates, ref probability, ref numberOfSolutions)) return true;
            candidates.Push(coord);
            return false;
        }


        public static Stack<Coordinates> FindCandidates(int[,] board) {
            int xMax = board.GetLength(0);
            int yMax = board.GetLength(1);
            var candidates = new Stack<Coordinates>();

            for(int x = 0; x < xMax; x++) {
                for(int y = 0; y < yMax; y++) {
                    if(board[x, y] != -2)
                        continue;
                    for(int i = 0; i < 9; i++) {
                        if(x + dx[i] >= 0 &&
                                y + dy[i] >= 0 &&
                                x + dx[i] < xMax &&
                                y + dy[i] < yMax &&
                                (board[x + dx[i], y + dy[i]] > 0)
                                ){
                            candidates.Push(new Coordinates(x, y));
                            break;
                        }
                    }
                }
            }
            return candidates;
        }

        public static void ListToArray(List<List<int>> gameBoard, out int[,] board) {
            int xMax = gameBoard.Count();
            int yMax = gameBoard[0].Count();

            board = new int[xMax, yMax];
            for(int x = 0; x < xMax; x++) {
                for(int y = 0; y < yMax; y++) {
                    board[x, y] = gameBoard[x][y];
                }
            }
        }

        public static Coordinates FindUnknown(int[,] board) {
            int xMax = board.GetLength(0);
            int yMax = board.GetLength(1);
            if (board[0, 0] == -2) {
                return new Coordinates(0, 0);
            }
            else if (board[xMax - 1, yMax - 1] == -2) {
                return new Coordinates(xMax - 1, yMax - 1);
            }
            else if (board[0, yMax - 1] == -2) {
                return new Coordinates(0, yMax - 1);
            }
            else if (board[xMax - 1, 0] == -2) {
                return new Coordinates(xMax - 1, 0);
            }

            for(int x = 0; x < xMax; x++) {
                for(int y = 0; y < yMax; y++) {
                    if (board[x, y] == -2) {
                        return new Coordinates(x, y);
                    }
                }
            }
            throw new Exception("No Unknown found! There are probably too many flags set.");
        }

        public static Coordinates GetSafestOption(int[,] probability, Stack<Coordinates> candidates) {
            Coordinates safest = new Coordinates(0, 0);
            int min = Int32.MaxValue;
            foreach(Coordinates c in candidates) {
                if(probability[c.x, c.y] < min) {
                    safest = c;
                    min = probability[c.x, c.y];
                }
            }
            return safest;
        }

        public static int GetOpenCount(int[,] board) {
            int xMax = board.GetLength(0);
            int yMax = board.GetLength(1);
            int count = 0;
            for(int x = 0; x < xMax; x++) {
                for(int y = 0; y < yMax; y++) {
                    if (board[x, y] != -2) {
                        count++;
                    }
                }
            }
            return count;
        }

        public static (List<List<int>>, List<List<int>>) Solve(List<List<int>> gameBoard) {
            var toClick = new List<List<int>>();
            var toFlag = new List<List<int>>();
            // Init
            int xMax = gameBoard.Count();
            int yMax = gameBoard[0].Count();
            bool[,] mines = new bool[xMax, yMax];
            int[,] board;
            ListToArray(gameBoard, out board);

            for(int x = 0; x < xMax; x++) {
                for(int y = 0; y < yMax; y++) {
                    if (board[x, y] != -4) {
                        mines[x, y] = false;
                        continue;
                    }
                    mines[x, y] = true;
                    // Lower Neighbours
                    for(int i = 0; i < 9; i++) {
                        if(x + dx[i] >= 0 &&
                                y + dy[i] >= 0 &&
                                x + dx[i] < xMax &&
                                y + dy[i] < yMax &&
                                (board[x + dx[i], y + dy[i]] > 0)
                                ){
                            board[x + dx[i], y + dy[i]] -= 1;
                        }
                    }
                }
            }
            //
            Stack<Coordinates> candidates = FindCandidates(board);

            if(GetOpenCount(board) < 4) {
                Coordinates unknown =FindUnknown(board);
                toClick.Add(new List<int>{unknown.x, unknown.y});
                return (toClick, toFlag);
            }
            Stack<Coordinates> candidates1 = FindCandidates(board);

            int[,] probability = new int[xMax, yMax];
            int numberOfSolutions = 0;

            FindSolution(board, ref mines, candidates, ref probability, ref numberOfSolutions);
            foreach(Coordinates c in candidates1) {
                if(probability[c.x, c.y] == 0) {
                    toClick.Add(new List<int>{c.x, c.y});
                } else if (probability[c.x, c.y] == numberOfSolutions){
                    toFlag.Add(new List<int>{c.x, c.y});
                }
            }
            if(toClick.Count() == 0) {
                Coordinates safest =GetSafestOption(probability, candidates1);
                toClick.Add(new List<int>{safest.x, safest.y});
            }
            return (toClick, toFlag);
        }

    }

    class Coordinates {
        public int x;
        public int y;

        public Coordinates(int _x, int _y) {
            x = _x;
            y = _y;
        }
    }
}
