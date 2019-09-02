using System;
using System.Linq;
using System.Collections.Generic;

namespace A
{
    class A2048
    {
        private int[,] board;
        
        public A2048()
        {
            board = new int[4, 4];
            board.Initialize();

			GenerateNewBlock();
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

		private void GenerateNewBlock()
		{
			List<Tuple<int, int>> emptyList = new List<Tuple<int, int>>();

			for (int i = 0; i < 4; ++i)
			{
				for (int j = 0; j < 4; ++j)
				{
					if (board[i, j] == 0)
						emptyList.Add(new Tuple<int, int>(i, j));
				}
			}

			Random randomSeed = new Random();
            var pair = emptyList[ randomSeed.Next(0, emptyList.Count()) ];
            board[pair.Item1, pair.Item2] = (randomSeed.Next(0, 2) == 0 ? 2 : 4);
		}
        
        private void MoveLeft()
        {
            bool moved = false;
            for (int i = 0; i < 4; ++i)
            {
				List<int> emptyList = new List<int>();
                for (int j = 0; j < 4; ++j)
                {
                    if (board[i, j] == 0)
                        emptyList.Add(j);
                    else if (emptyList.Count() != 0) {
                        board[i, emptyList[0]] = board[i, j];
                        board[i, j] = 0;
						emptyList.RemoveAt(0);
						emptyList.Add(j);
                        moved = true;
                    }
                }

				for (int j = 0; j < 3; ++j)
				{
					if (board[i, j] == board[i, j + 1] && board[i, j] != 0) {
						moved = true;
						board[i, j] *= 2;
						for (int k = j + 1; k < 3; ++k)
						{
							board[i, k] = board[i, k + 1];
						}
						board[i, 3] = 0;
					}
				}
            }
            
            if (moved) {
                GenerateNewBlock();
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
        
        private void PrintBoard()
        {
            Console.Clear();
            for (int i = 0; i < 4; ++i)
            {
				for (int j = 0; j < 4; ++j)
				{
					Console.Write("|");
					for (int k = 0; k < 7; ++k)
					{
						Console.Write("-");
					}
				}
				Console.Write("|\n");

                for (int j = 0; j < 4; ++j)
                {
					if (board[i,j] == 0)
						Console.Write("|       ");
					else
						Console.Write("|{0, -7}", board[i, j]);
                }
                Console.Write("|\n");
            }

			for (int j = 0; j < 4; ++j)
				{
					Console.Write("|");
					for (int k = 0; k < 7; ++k)
					{
						Console.Write("-");
					}
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
