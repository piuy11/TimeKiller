using System;
using System.Linq;
using System.Collections.Generic;

namespace A
{
    class A2048
    {
        private int[,] board;
        private List<Tuple<int, int>> emptyList;
        
        public A2048()
        {
            board = new int[4, 4];
            emptyList = new List<Tuple<int, int>>();
            
            board.Initialize();
            board[0, 3] = 2;
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    emptyList.Add(new Tuple<int, int>(i, j));
                }
            }
            emptyList.RemoveAt(0);
        }
        
        public void Play()
        {
            while (true)
            {
                PrintBoard();
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        MoveDown();
                        break;
                    case ConsoleKey.LeftArrow:
                        MoveLeft();
                        break;
                    case ConsoleKey.RightArrow:
                        MoveRight();
                        break;
                }
            }
        }
        
        private void MoveLeft()
        {
            bool moved = false;
            for (int i = 0; i < 4; ++i)
            {
                int empty = 4;
                for (int j = 0; j < 4; ++j)
                {
                    if (board[i, j] == 0 && empty == 4)
                        empty = j;
                    else if (board[i, j] != 0 && empty != 4) {
                        board[i, empty] = board[i, j];
                        board[i, j] = 0;
                        empty++;
                        moved = true;
                    }
                        
                        
                }
            }
        }
        
        private void MoveRight()
        {
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    int temp = board[i, j];
                    board[i, j] = board[i, 3 - j];
                    board[i, 3 - j] = temp;
                }
            }
            
            MoveLeft();
            
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    int temp = board[i, j];
                    board[i, j] = board[i, 3 - j];
                    board[i, 3 - j] = temp;
                }
            }
        }
        
        private void MoveUp()
        {
            for (int i = 1; i < 4; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    int temp = board[i, j];
                    board[i, j] = board[j, i];
                    board[j, i] = temp;
                }
            }
            
            MoveLeft();
            
            for (int i = 1; i < 4; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    int temp = board[i, j];
                    board[i, j] = board[j, i];
                    board[j, i] = temp;
                }
            }
        }
        
        private void MoveDown()
        {
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    int temp = board[i, j];
                    board[i, j] = board[i, 3 - j];
                    board[i, 3 - j] = temp;
                }
            }
            for (int i = 1; i < 4; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    int temp = board[i, j];
                    board[i, j] = board[j, i];
                    board[j, i] = temp;
                }
            }
            
            MoveLeft();
            
            for (int i = 1; i < 4; ++i)
            {
                for (int j = 0; j < i; ++j)
                {
                    int temp = board[i, j];
                    board[i, j] = board[j, i];
                    board[j, i] = temp;
                }
            }
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    int temp = board[i, j];
                    board[i, j] = board[i, 3 - j];
                    board[i, 3 - j] = temp;
                }
            }
        }
        
        private void PrintBoard()
        {
            Console.Clear();
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    Console.Write("{0, 7}", board[i, j]);
                }
                Console.Write("\n\n");
            }
        }
    }
    
    class A
    {
        static void Main()
        {
            A2048 game = new A2048();
            game.Play();
        }
    }
}
