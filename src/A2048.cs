using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace A
{
    class A2048
    {
        private int[,] board;
        private int score;

        public A2048()
        {
            board = new int[4, 4];
            board.Initialize();

			GenerateNewBlock();
        }
        
        public void Play()
        {
			if (File.Exists("Save.dat")) {
				using (BinaryReader br = new BinaryReader(File.Open("Save.dat", FileMode.Open)))
				{
					for (int i = 0; i < 4; ++i)
					{
						for (int j = 0; j < 4; ++j)
						{
							board[i, j] = br.ReadInt32();
						}
					}
				}
			}



            while (CheckIfDead() == false)
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
				SaveData();
            }
			File.Delete("Save.dat");
			PrintBoard();
			Console.WriteLine("GAME OVER");
			Console.ReadKey(true);

			Console.Clear();
			Console.WriteLine("Your Score : " + score);
        }

		private void SaveData()
		{
			using (BinaryWriter bw = new BinaryWriter(File.Open("Save.dat", FileMode.OpenOrCreate)))
			{
				for (int i = 0; i < 4; ++i)
				{
					for (int j = 0; j < 4; ++j)
					{
						bw.Write(board[i, j]);
					}
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

		private void Swap(ref int a, ref int b)
		{
			int temp = a;
			a = b;
			b = temp;
		}

		private void FlipDiagonally()
		{
            for (int i = 1; i < 4; ++i)
            {
                for (int j = 0; j < i; ++j)
					Swap(ref board[i, j], ref board[j ,i]);
            }
		}

		private void FlipVertically()
		{
			for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 2; ++j)
					Swap(ref board[i, j], ref board[i, 3 - j]);
            }
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
			FlipVertically();
            MoveLeft();
			FlipVertically();
        }
        
        private void MoveUp()
        {
			FlipDiagonally();            
            MoveLeft();
			FlipDiagonally();
        }
        
        private void MoveDown()
        {
			FlipDiagonally();
            FlipVertically();
            MoveLeft();
            FlipVertically();
			FlipDiagonally();
        }
        
        private void PrintBoard()
        {
            Console.Clear();

			score = 0;
			for (int i = 0; i < 4; ++i)
            {
				for (int j = 0; j < 4; ++j)
				{
					score += board[i, j];
				}
			}
			Console.WriteLine("score : " + score);

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
			Console.WriteLine("|");
        }

		private bool CheckIfDead()
		{
			for (int i = 0; i < 4; ++i)
			{
				for (int j = 0; j < 4; ++j)
				{
					if (board[i, j] == 0)
						return false;
				}
			}

			for (int i = 0; i < 3; ++i)
			{
				for (int j = 0; j < 3; ++j)
				{
					if (board[i, j] == board[i, j + 1] || board[i, j] == board[i + 1, j])
						return false;
				}
			}

			for (int i = 0; i < 3; ++i)
			{
				if (board[i, 3] == board[i + 1, 3] || board[3, i] == board[3, i + 1])
					return false;
			}

			return true;
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
