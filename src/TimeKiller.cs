using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TimeKiller
{
    class TimeKiller
    {
        public const string GAME_VERSION = "test_v1.0";
        public const string BASIC_PATH = @"c:\Program Files\TimeKiller";
        public const string LOG_PATH = BASIC_PATH + @"\Log.dat";
        
        static void BasicSet()
        {
            Directory.CreateDirectory(BASIC_PATH);
            if (!File.Exists(LOG_PATH)) {
                BinaryWriter bw = new BinaryWriter(File.Open(LOG_PATH, FileMode.CreateNew));
                bw.Close();
            }
        }

        static void Log(string name)
        {
            using ( BinaryWriter logger = new BinaryWriter(File.Open(LOG_PATH, FileMode.Append)) )
            {
                // logger.Seek(0, SeekOrigin.Begin);
                logger.Write(DateTime.Now.ToString());
                logger.Write(name);
            }
        }
        
        static void Main()
        {
            BasicSet();
            
            while (true)
            {
                Console.Clear();
                Console.WriteLine("심심풀이 " + GAME_VERSION);
                Console.WriteLine("1. 부자가 되어보자");
                Console.WriteLine("2. 숫자야구");
                Console.WriteLine("3. 로또 추첨기");
                Console.WriteLine("4. 오목");
				Console.WriteLine("5. 2048");
                
                switch(Console.ReadKey(true).KeyChar)
                {
                    case '1':
                        Log("부자가 되어보자");
                        BeARich.Game();
                        break;
                    case '2':
                        Log("숫자야구");
                        // NumberBaseBall.Game();
                        break;
                    case '3':
                        Log("로또 추첨기");
                        // Lottery.Game();
                        break;
                    case '4':
                        Log("오목");
                        // Renju.Game();
                        break;
					case '5':
                        Log("2048");
						A2048 game = new A2048();
						game.Play();
						break;
                    case '6':
                        Log("블랙홀");
                        // Blackhole game = new Blackhole();
                        // game.Play();
                        break;
                    case '>':
                        if (Console.ReadLine() == "power overwhelming") {
                            Log("관리자");
                            Administrator.Play();
                        }
                        break;
                }
            }
        }
    }

    class Administrator
    {
        public Administrator()
        {

        }

        public static void Play()
        {
            Console.Clear();
            try {
                BinaryReader logReader = new BinaryReader(File.Open(TimeKiller.LOG_PATH, FileMode.Open));
                while (true)
                {
                    Console.Write(logReader.ReadString() + " / ");
                    Console.WriteLine(logReader.ReadString());
                }
            } catch (EndOfStreamException e) {
                Console.ReadKey(true);
            }
            // Console.ReadKey(true);
        }
    }
    
    class BeARich
    {
        public const long StartMoney = 1000;
        public const string GAME_VERSION = "test_v1.0";
        public const string BASIC_PATH = TimeKiller.BASIC_PATH + @"\BeARich";
        public const string SCOREBOARD_PATH = BASIC_PATH + @"\Scoreborad.dat";
        public const string SAVE_PATH = BASIC_PATH + @"Save.dat";
        public const string PATCH_NOTE = GAME_VERSION + @" 패치 노트
1. 심심풀이 프로그램에 탑재 완료
2. 메뉴 추가
3. 점수판 기능 추가
4. 이어하기 기능 추가";
        
        
        private static Tuple<string, long>[] scoreBoard = new Tuple<string, long>[10];
        
        public static void Game()
        {
            BasicSet();
            do
            {
                Console.Clear();
                Console.WriteLine("부자가 되어보자 " + GAME_VERSION);
                Console.WriteLine("1. 새로하기");
                Console.WriteLine("2. 이어하기");
                Console.WriteLine("3. 점수판");
                Console.WriteLine("4. 패치 노트");
                Console.WriteLine("5. 종료");
                
                char choice = Console.ReadKey(true).KeyChar;
                switch(choice)
                {
                    case '1':
                        PlayNew();
                        break;
                    case '2':
                        PlayNew(PlayContinue());
                        break;
                    case '3':
                        ShowScoreBoard();
                        break;
                    case '4':
                        Console.Clear();
                        Console.WriteLine(PATCH_NOTE);
                        Console.WriteLine("PRESS ANY KEY TO CONTINUE");
                        Console.ReadKey(true);
                        break;
                    case '5':
                        return;
                }
                    
            } while (true);
        }
        
        private static void BasicSet()
        {
            if (!File.Exists(SCOREBOARD_PATH)) {
                using (BinaryWriter bw = new BinaryWriter(File.Open(SCOREBOARD_PATH, FileMode.CreateNew)))
                {
                    bw.Write("홍민준");
                    bw.Write(10000L);
                    bw.Close();
                    Console.WriteLine("점수판을 생성했습니다.");
                    Console.ReadKey(true);
                }
            }
                using (BinaryReader bw = new BinaryReader(File.Open(SCOREBOARD_PATH, FileMode.Open)))
                {
                    int i = 0;
                    try {
                        for (i = 0; i < 10; ++i)
                        {
                            string name = bw.ReadString();
                            long score = bw.ReadInt64();
                            scoreBoard[i] = new Tuple<string, long>(name, score);
                        }
                    }
                    catch (EndOfStreamException e) {
                        for (int j = i; j < 10; ++j)
                            scoreBoard[j] = new Tuple<string, long>("", 0L);
                    }
                }
        }
        
        static void PlayNew(long money = StartMoney)
        {
            do {
                Console.Clear();
                Console.WriteLine("돈 : " + money);
                Console.WriteLine("1. 배팅한 돈의 2배 : 50%");
                Console.WriteLine("2. 배팅한 돈의 4배 : 25%");
                Console.WriteLine("3. 배팅한 돈의 10배 : 10%");
                Console.WriteLine("4. 배팅한 돈의 100배 : 1%");
                Console.WriteLine("5. 노가다 : + 50");
                Console.WriteLine("6. 구구단 맞추기 : + 100");
                
                switch(Console.ReadKey(true).KeyChar)
                {
                    case '1':
                        Console.WriteLine("1번을 입력하셨습니다.");
                        Console.ReadKey(true);
                        Gamble(ref money, 2);
                        break;
                    case '2':
                        Console.WriteLine("2번을 입력하셨습니다.");
                        Console.ReadKey(true);
                        Gamble(ref money, 4);
                        break;
                    case '3':
                        Console.WriteLine("3번을 입력하셨습니다.");
                        Console.ReadKey(true);
                        Gamble(ref money, 10);
                        break;
                    case '4':
                        Console.WriteLine("4번을 입력하셨습니다.");
                        Console.ReadKey(true);
                        Gamble(ref money, 100);
                        break;
                }
            } while (money > 0);
        }
        
        static long PlayContinue()
        {
            long money;
            using (BinaryReader br = new BinaryReader(File.Open(SAVE_PATH, FileMode.Open)))
            {
                money = br.ReadInt64();
            }
            return money;
        }
        
        static void ShowScoreBoard()
        {
            Console.Clear();
            Console.WriteLine("점수판\n");
            int i = 1;
            foreach (var tuple in scoreBoard)
            {
                Console.WriteLine(i.ToString() + ' ' + tuple.Item2 + ' ' + tuple.Item1);
                i++;
            }
            Console.ReadKey(true);
        }
        
        static void WriteScoreBoard()
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(SCOREBOARD_PATH, FileMode.CreateNew)))
            {
                foreach (var tuple in scoreBoard)
                {
                    bw.Write(tuple.Item1);
                    bw.Write(tuple.Item2);
                }
                bw.Close();
            }
        }
        
        static void Gamble(ref long money, int nTimes)
        {
            double winChance = 1.0 / nTimes;
            Console.WriteLine("얼마를 배팅하시겠습니까?");
            long betMoney;
            try {
                betMoney = long.Parse(Console.ReadLine());
            }
            catch (FormatException e) {
                Console.WriteLine("잘못된 입력입니다..!");
                Console.ReadKey(true);
                return;
            }
            
            if (betMoney <= 0 || betMoney > money) {
                Console.WriteLine("잘못된 입력입니다..!");
                Console.ReadKey(true);
                return;
            }
            
            Console.WriteLine("- " + betMoney);
            money -= betMoney;
            Console.WriteLine("추첨을 진행하겠습니다.");
            Console.ReadKey(true);
            
            Random randomSeed = new Random();
            if (randomSeed.NextDouble() <= winChance) {
                Console.WriteLine("축하합니다! 당첨되셨습니다! + " + betMoney * nTimes);
                money += betMoney * nTimes;
                Console.ReadKey(true);
            }
            else {
                Console.WriteLine("블랙 말랑카우가 되셨습니다.. ㅠㅜ");
                Console.ReadKey(true);
            }
            
        }
    }

	class A2048
    {
		public const string BASIC_PATH = TimeKiller.BASIC_PATH + @"\A2048";
		public const string SAVE_PATH = BASIC_PATH + @"\Save.dat";
		public const string SCOREBOARD_PATH = BASIC_PATH + @"\ScoreBoard.dat";


        private int[,] board;
        private int score;
        private Random randomSeed;

        public A2048()
        {
            board = new int[4, 4];
            board.Initialize();
            randomSeed = new Random();
			GenerateNewBlock();
        }
        
        public void Play()
        {
			if (File.Exists(SAVE_PATH)) {
				using (BinaryReader br = new BinaryReader(File.Open(SAVE_PATH, FileMode.Open)))
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
			else
				Directory.CreateDirectory(BASIC_PATH);



            while (CheckIfDead() == false)
            {
                PrintBoard();
                bool escape = false;
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
                    case ConsoleKey.Escape:
                        return;
                        break;
                }
				SaveData();
            }
			File.Delete(SAVE_PATH);
			PrintBoard();
			Console.WriteLine("GAME OVER");
            Console.ReadKey(true);
            Console.WriteLine("Press Esc Key to Continue");
			
            while (true)
            {
                var input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.Escape)
                    break;
            }

			Console.Clear();
			Console.WriteLine("Your Score : " + score);
        }

		private void SaveData()
		{
			using (BinaryWriter bw = new BinaryWriter(File.Open(SAVE_PATH, FileMode.OpenOrCreate)))
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
}
