using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Timers;
using System.Threading;

namespace General
{
    class General
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
}

namespace TimeKiller
{
    class TimeKiller
    {
        public const string GAME_VERSION = "test_v1.0";
        public const string BASIC_PATH = @"c:\Program Files\TimeKiller";
        public const string LOG_PATH = BASIC_PATH + @"\Log.dat";

        public static void Continue()
        {
            Console.WriteLine("계속 하려면 아무 키를 눌러 주세요.");
            Console.ReadKey(true);
        }
        
        static void BasicSet()
        {
            Directory.CreateDirectory(BASIC_PATH);
            if (!File.Exists(LOG_PATH)) {
                BinaryWriter bw = new BinaryWriter(File.Open(LOG_PATH, FileMode.CreateNew));
                bw.Close();
            }
            Log("프로그램 실행");
        }

        static void Log(string input)
        {
            List<string> logs = new List<string>();

            using ( BinaryReader logReader = new BinaryReader(File.Open(LOG_PATH, FileMode.Open)))
            {
                try {
                    while (true)
                    {
                        logs.Add(logReader.ReadString());
                    }
                } catch (EndOfStreamException) {

                }
            }

            logs.Insert(0, DateTime.Now.ToString());
            logs.Insert(1, input);

            using ( BinaryWriter logger = new BinaryWriter(File.Open(LOG_PATH, FileMode.Create)) )
            {
                foreach(var str in logs)
                {
                    logger.Write(str);
                }
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
				Console.WriteLine("2. 2048");
                Console.WriteLine("3. 블랙홀");
                Console.WriteLine("4. 야찌");
                Console.WriteLine("5. 테트리스");
                // 숫자야구,로또추첨기, 오목, 게시판
                
                Game game;
                switch(Console.ReadKey(true).KeyChar)
                {
                    case '1':
                        Log("부자가 되어보자");
                        game = new BeARich();
                        break;
					case '2':
                        Log("2048");
						game = new A2048();
						break;
                    case '3':
                        Log("블랙홀");
                        game = new Blackhole();
                        break;
                    case '4':
                        Log("야찌");
                        game = new Yahtzee();
                        break;
                    case '5':
                        Log("테트리스");
                        game = new Tetris();
                        break;
                    case '>':
                        if (Console.ReadLine() == "power overwhelming") {
                            Log("관리자");
                            Administrator.Play();
                        }
                        continue;
                    default:
                        continue;
                }
                game.Menu();
            }
        }
    }

    abstract class Game
    {
        protected abstract long Play();
        protected abstract void ResetGame();

        public virtual void Menu()
        {
            ConsoleKeyInfo input;
            do {
                ResetGame();

                Console.Clear();
                Console.WriteLine("1. 게임 시작");
                Console.WriteLine("2. 게임 설명");
                Console.WriteLine("Esc 키를 눌러 나갑니다.");
                input = Console.ReadKey(true);

                if (input.KeyChar == '1') {
                    if (Play() == -1)
                        continue;
                }
                else if (input.KeyChar == '2')
                    Description();
            } while (input.Key != ConsoleKey.Escape);
        }

        protected virtual void Description()
        {
            Console.Clear();
            Console.WriteLine("게임 설명이 없습니다..");
            TimeKiller.Continue();
        }
    }

    abstract class GameWithScoreboard : Game
    {
        Tuple<string, long>[] monthlyScores, allTimeScores;

        protected abstract string GetLogPath(bool isMonthScore);

        public GameWithScoreboard()
        {
            monthlyScores = new Tuple<string, long>[10];
            allTimeScores = new Tuple<string, long>[10];
            
            FirstSet(true);
            FirstSet(false);
        }

        private void FirstSet(bool isMonthScore)
        {
            Tuple<string, long>[] scores = isMonthScore ? monthlyScores : allTimeScores;
            if ( !File.Exists(GetLogPath(isMonthScore)) ) {
                BinaryWriter bw = new BinaryWriter(File.Open(GetLogPath(isMonthScore), FileMode.CreateNew));
                bw.Close();

                for (int i = 0; i < 10; ++i)
                    scores[i] = new Tuple<string, long>("", 0L);

                WriteScoreboard(isMonthScore);
            }
            else {
                using ( BinaryReader bw = new BinaryReader(File.Open(GetLogPath(isMonthScore), FileMode.Open)) )
                {
                    for (int i = 0; i < 10; ++i)
                    {
                        string name = bw.ReadString();
                        long score = bw.ReadInt64();
                        scores[i] = new Tuple<string, long>(name, score);
                    }
                }
            }

        }

        public override void Menu()
        {
            ResetGame();
            
            ConsoleKeyInfo input;
            do {
                ResetGame();

                Console.Clear();
                Console.WriteLine("1. 게임 시작");
                Console.WriteLine("2. 게임 설명");
                Console.WriteLine("3. 점수판");
                Console.WriteLine("Esc 키를 눌러 나갑니다.");
                input = Console.ReadKey(true);

                if (input.KeyChar == '1') {
                    long score = Play();
                    if (score == -1)
                        continue;
                    SetScoreboard(score);
                }
                else if (input.KeyChar == '2')
                    Description();
                else if (input.KeyChar == '3')
                    ShowScoreboard();
            } while (input.Key != ConsoleKey.Escape);
            
        }

        private void SetScoreboard(long score)
        {
            int monthlyRank = GetRank(true, score);
            int allTimeRank = GetRank(false, score);
            
            if (monthlyRank != 10 || allTimeRank != 10) {
                string name;
                do {
                    Console.Clear();
                    Console.WriteLine("기록 달성!");
                    Console.WriteLine("이름을 입력해주세요. (최대 20자)");
                    name = Console.ReadLine();
                } while ( (name.Length >= 1 && name.Length <= 20) == false );

                Tuple<string, long> player = new Tuple<string, long>(name, score);
                if (monthlyRank != 10)
                    PushScoreboard(true, player, monthlyRank);
                if (allTimeRank != 10)
                    PushScoreboard(false, player, allTimeRank);
                
                WriteScoreboard();
                ShowScoreboard(monthlyRank, allTimeRank);
            }
        }

        private int GetRank(bool isMonthScore, long score)
        {
            int i;
            Tuple<string, long>[] scores = isMonthScore ? monthlyScores : allTimeScores;

            for (i = 9; i >= 0; --i)
            {
                if (scores[i].Item2 >= score)
                    break;
            }
            return i + 1;
        }

        private void PushScoreboard(bool isMonthScore, Tuple<string, long> data, int rank)
        {
            Tuple<string, long>[] scores = isMonthScore ? monthlyScores : allTimeScores;

            for (int i = 9; i > rank; --i)
                scores[i] = scores[i - 1];
            scores[rank] = data;
        }

        protected void ShowScoreboard(int monthlyRank = 10, int allTimeRank = 10)
        {
            ShowScoreboard(true, monthlyRank);
            ShowScoreboard(false, allTimeRank);
        }

        protected void ShowScoreboard(bool isMonthScore, int rank)
        {
            Tuple<string, long>[] scores = isMonthScore ? monthlyScores : allTimeScores;

            Console.Clear();
            if (isMonthScore)
                Console.WriteLine("이번 달 점수판\n");
            else  
                Console.WriteLine("전체 점수판\n");

            foreach (int i in Enumerable.Range(0, 10))
            {
                var tuple = scores[i];
                if (i == rank)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("{0, -2} {1, -12} {2}", i + 1, tuple.Item2, tuple.Item1);
                if (i == rank)
                    Console.ResetColor();
            }            
            Console.ReadKey(true);
        }

        protected void WriteScoreboard()
        {
            WriteScoreboard(true);
            WriteScoreboard(false);
        }

        protected void WriteScoreboard(bool isMonthScore)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(GetLogPath(isMonthScore), FileMode.Create)))
            {
                Tuple<string, long>[] scores;
            
                if (isMonthScore)
                    scores = monthlyScores;
                else
                    scores = allTimeScores;
                
                foreach (var tuple in scores)
                {
                    bw.Write(tuple.Item1);
                    bw.Write(tuple.Item2);
                }

                bw.Close();
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
            using ( BinaryReader logReader = new BinaryReader(File.Open(TimeKiller.LOG_PATH, FileMode.Open)) )
            {
                try {
                    while (true)
                    {
                        Console.Clear();
                        foreach (var i in Enumerable.Range(0, 10))
                        {
                            Console.Write(logReader.ReadString() + " / ");
                            Console.WriteLine(logReader.ReadString());
                        }
                        foreach (var i in Enumerable.Range(0, 20))
                        {
                            Console.Write('-');
                        }
                        Console.Write('\n');
                        Console.ReadKey(true);
                    }
                } catch (EndOfStreamException) {
                    Console.WriteLine("END OF FILE");
                    Console.ReadKey(true);
                }
            }
        }
    }
    
    class BeARich : GameWithScoreboard
    {
        public const long startMoney = 10000L;
        Random randomSeed;
        private long money, nTimes, bestMoney;

        public BeARich() : base()
        {
            
        }

        protected override string GetLogPath(bool isMonthScore)
        {
            return TimeKiller.BASIC_PATH + (isMonthScore ? @"\BeARich\MonthlyScoreboard.dat" : @"\BeARich\Scoreboard.dat");
        }

        protected override void ResetGame()
        {
            bestMoney = money = startMoney;
            randomSeed = new Random();
        }
        
        protected override long Play()
        {
            do {
                bestMoney = (bestMoney < money) ? money : bestMoney;
                Console.Clear();
                Console.WriteLine("돈 : " + money);
                Console.WriteLine("1. 배팅한 돈의 2배 : 50%");
                Console.WriteLine("2. 배팅한 돈의 4배 : 25%");
                Console.WriteLine("3. 배팅한 돈의 10배 : 10%");
                Console.WriteLine("4. 배팅한 돈의 100배 : 1%");
                Console.WriteLine("5. 노가다");
                // Console.WriteLine("6. 구구단 맞추기 : + 100");
                Console.WriteLine("Esc 키를 눌러 자살합니다.");
                
                var input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.Escape)
                    break;
                else if (input.KeyChar >= '1' && input.KeyChar <= '5') {
                    Console.WriteLine("{0}번을 입력하셨습니다.", input.KeyChar);
                    Console.ReadKey(true);
                    
                    switch(input.KeyChar)
                    {
                        case '1':
                            nTimes = 2;
                            break;
                        case '2':
                            nTimes = 4;
                            break;
                        case '3':
                            nTimes = 10;
                            break;
                        case '4':
                            nTimes = 100;
                            break;
                        case '5':
                            HardWork();
                            continue;
                    }
                }
                else
                    continue;
                
                Gamble();
            } while (money > 0);

            Console.Clear();
            Console.WriteLine("당신은 굶어 죽었습니다...");
            Console.WriteLine("최고 재산 : " + bestMoney);
            Console.ReadKey(true);

            return bestMoney;
        }
        
        private void Gamble()
        {
            double winChance = 1.0 / nTimes;
            Console.WriteLine("얼마를 배팅하시겠습니까?");
            long betMoney;
            try {
                betMoney = long.Parse(Console.ReadLine());
            }
            catch (FormatException) {
                Console.WriteLine("잘못된 입력입니다..!");
                Console.ReadKey(true);
                return;
            }
            
            if (betMoney <= 0 || betMoney > money) {
                Console.WriteLine("낼 수 없는 금액입니다..!");
                Console.ReadKey(true);
                return;
            }
            
            Console.WriteLine("- " + betMoney);
            money -= betMoney;
            Console.WriteLine("추첨을 진행하겠습니다.");
            Console.ReadKey(true);
            
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

        private void HardWork()
        {
            ConsoleKeyInfo input;
            do {
                Console.Clear();
                int gained = randomSeed.Next(100, 200);
                money += gained;
                Console.WriteLine("+ " + gained);
                Console.WriteLine("현재 재산 : " + money);
                Console.WriteLine("아무 키나 누르면 계속 노가다를 진행합니다.");
                Console.WriteLine("Esc 키를 누르면 노가다를 멈춥니다.");
                input = Console.ReadKey(true);
            } while (input.Key != ConsoleKey.Escape);
        }
    }

	class A2048 : GameWithScoreboard
    {
		public const string BASIC_PATH = TimeKiller.BASIC_PATH + @"\A2048";
		public const string SAVE_PATH = BASIC_PATH + @"\Save.dat";

        private int[,] board;
        private int score;
        private Random randomSeed;

        public A2048() : base()
        {

        }

        protected override string GetLogPath(bool isMonthScore)
        {
            return BASIC_PATH + (isMonthScore ? @"\MonthlyScoreboard.dat" : @"\Scoreboard.dat");
        }

        protected override void ResetGame()
        {
            board = new int[4, 4];
            board.Initialize();
            randomSeed = new Random();
			GenerateNewBlock();
        }
        
        protected override long Play()
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
                        return -1;
                }
				SaveData();
            }
			File.Delete(SAVE_PATH);
			PrintBoard();
			Console.WriteLine("GAME OVER");
            Console.ReadKey(true);

            return score;
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
            board[pair.Item1, pair.Item2] = (randomSeed.Next(0, 10) == 0 ? 4 : 2);
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

    class Blackhole : Game
    {
        private Card[,] board;
        private List<Card> blackhole;
        private int[] cardLeft;

        public Blackhole() : base()
        {

        }

        protected override void ResetGame()
        {
            board = new Card[17, 3];
            blackhole = new List<Card>();
            cardLeft = new int[17];

            Deck deck = new Deck();
            foreach (int i in Enumerable.Range(0, 17))
            {
                foreach (int j in Enumerable.Range(0, 3))
                {
                    Card card = deck.Pick();
                    if (card.name == "A♠") {
                        blackhole.Add(card);
                        board[i, j] = deck.Pick();
                    }
                    else
                        board[i, j] = card;
                }
            }
            foreach (int i in Enumerable.Range(0, 17))
            {
                cardLeft[i] = 3;
            }
        }

        protected override long Play()
        {
            Console.Clear();
            
            while (true)
            {
                Console.Clear();
                PrintBoard();
                if (IsDead())
                    break;

                var input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.Escape)
                    return -1;
                char inputChar = input.KeyChar;
                if (inputChar >= 'a' && inputChar <= 'q') {
                    int index = inputChar - 'a';
                    if (cardLeft[index] == 0)
                        continue;
                    Rank targetRank = blackhole[blackhole.Count - 1].rank;
                    Card currentCard = board[index, cardLeft[index] - 1];
                    Rank currentRank = currentCard.rank;
                    if (Card.IsNearRank(targetRank, currentRank)) {
                        cardLeft[index]--;
                        blackhole.Add(currentCard);
                    }
                }
            }

            return 0;
        }

        private bool IsDead()
        {
            Rank targetRank = blackhole[blackhole.Count - 1].rank;
            foreach (int i in Enumerable.Range(0, 17))
            {
                Rank currentRank = board[i, cardLeft[i] - 1].rank;
                if (Card.IsNearRank(targetRank, currentRank))
                    return false;
            }

            return true;
        }

        public void PrintBoard()
        {
            foreach (int i in Enumerable.Range(0, 17))
            {
                Console.Write("{0, -4}", (char)('A' + i));
            }
            Console.Write('\n');
            foreach (int i in Enumerable.Range(0, 3))
            {
                foreach (int j in Enumerable.Range(0, 17))
                {
                    if (cardLeft[j] < i + 1)
                        Console.Write("{0, -4}", ' ');
                    else
                        Console.Write("{0, -4}", board[j, i].name);
                }
                Console.Write('\n');
            }
            Console.WriteLine("Blackhole");
            foreach (var card in blackhole)
            {
                Console.Write("{0, -4}", card.name);
            }
            Console.Write('\n');
        }
    }

    class Yahtzee : GameWithScoreboard
    {
        private Dice[] dices;
        private int[] scoreboard;
        private bool[] isScored;
        // private bool isAIMode;
        private const string scoreboardFrame = @"Esc 키를 누르면 메뉴로 나갑니다. (저장x)
=======================================
| 선택  점수    설명
=======================================
|  A  |      | 주사위 1 합
---------------------------------------
|  B  |      | 주사위 2 합
---------------------------------------
|  C  |      | 주사위 3 합
---------------------------------------
|  D  |      | 주사위 4 합
---------------------------------------
|  E  |      | 주사위 5 합
---------------------------------------
|  F  |      | 주사위 6 합
=======================================
|     |      | 여기까지 총합
=======================================
|     |      | 여기까지 총합 >= 63이면 +35
=======================================
|  G  |      | 트리플 (모든 주사위 합)
---------------------------------------
|  H  |      | 포카드 (모든 주사위 합)
---------------------------------------
|  I  |      | 풀하우스 (+25)
---------------------------------------
|  J  |      | 스몰 스트레이트 (+30)
---------------------------------------
|  K  |      | 라지 스트레이트 (+40)
---------------------------------------
|  L  |      | 야찌 (+50)
---------------------------------------
|  M  |      | 보너스 (모든 주사위 합)
=======================================
|     |      | 총합
=======================================
";

        public Yahtzee() : base()
        {
            
        }

        protected override string GetLogPath(bool isMonthScore = false)
        {
            return TimeKiller.BASIC_PATH + (isMonthScore ? @"\Yahtzee\MonthlyScoreboard.dat" : @"\Yahtzee\Scoreboard.dat");
        }

        protected override void ResetGame()
        {
            dices = new Dice[5];
            foreach (int i in Enumerable.Range(0, dices.Length))
                dices[i] = new Dice();

            scoreboard = new int[16];
            scoreboard.Initialize();
            isScored = new bool[16];
            isScored.Initialize();
            isScored[6] = isScored[7] = isScored[15] = true;
            
            // this.isAIMode = b;
        }

        protected override long Play()
        {
            foreach (int i in Enumerable.Range(0, 13))
            {
                Roll();
                int rerollLeft = 2;
                while (rerollLeft != 0)
                {
                    ConsoleKeyInfo input;
                    bool[] selection = new bool[dices.Length];
                    selection.Initialize();

                    while (true)
                    {
                        PrintBoard(rerollLeft);
                        foreach (int j in Enumerable.Range(0, selection.Length))
                        {
                            if (selection[j])
                                Console.BackgroundColor = ConsoleColor.Cyan;
                            Console.Write(j + 1);
                            if (selection[j])
                                Console.ResetColor();
                            Console.Write("    ");
                        }
                        
                        Console.Write('\n');
                        input = Console.ReadKey(true);
                        if (input.Key == ConsoleKey.Escape)
                            return -1;
                        else if (input.Key == ConsoleKey.Enter)
                            break; // selection end
                        else if ( (input.KeyChar >= '1' && input.KeyChar <= '5') == false)
                            continue; // strange input

                        int index = input.KeyChar - '1';
                        selection[index] = !selection[index];

                    }

                    if (selection.All(b => false))
                        break;
                    Roll(selection);
                    --rerollLeft;
                }

                while (true) // choose valid
                {
                    PrintBoard();
                    Console.WriteLine("주사위 값을 넣을 곳을 선택해 주세요. (A ~ M)");
                    
                    var input = Console.ReadKey(true);
                    if (input.Key == ConsoleKey.Escape)
                        return -1;
                    char choice = Char.ToUpper(input.KeyChar);
                    int index = (choice <= 'F') ? choice - 'A' : choice - 'A' + 2;
                    if (index < 0 || index > 16)
                        continue;
                    else if (isScored[index])
                        continue;
                    
                    int score = 0;
                    if (index < 6)
                        score = dices.Count(dice => dice.value == index + 1) * (index + 1);
                    else {
                        switch (index)
                        {
                        case 6:
                        case 7:
                            continue;
                        case 8: // 3 of a kind
                            int[] numCount1 = new int[6];
                            numCount1.Initialize();
                            foreach (var dice in dices)
                            {
                                numCount1[dice.value - 1]++;
                            }
                            bool isValid1 = numCount1.Any(num => num >= 3);
                            score = isValid1 ? dices.Sum(dice => dice.value) : 0;
                            break;
                        case 9: // 4 of a kind
                            int[] numCount2 = new int[6];
                            numCount2.Initialize();
                            foreach (var dice in dices)
                            {
                                numCount2[dice.value - 1]++;
                            }
                            bool isValid2 = numCount2.Any(num => num >= 4);
                            score = isValid2 ? dices.Sum(dice => dice.value) : 0;
                            break;
                        case 10: // full house
                            int[] numCount3 = new int[6];
                            numCount3.Initialize();
                            foreach (var dice in dices)
                            {
                                numCount3[dice.value - 1]++;
                            }
                            bool isValid3 = numCount3.Any(num => num == 3) && numCount3.Any(num => num == 2);
                            score = isValid3 ? 25 : 0;
                            break;
                        case 11: // small straight
                            Array.Sort(dices);
                            bool isValid4 = true;
                            bool chanceUsed = false;
                            int before = dices[0].value;
                            foreach (int j in Enumerable.Range(1, 4))
                            {
                                if (dices[j].value != before + 1) {
                                    if (!chanceUsed)
                                        chanceUsed = true;
                                    else {
                                        isValid4 = false;
                                        break;
                                    }
                                } else
                                    before = dices[j].value;
                            }
                            score = isValid4 ? 30 : 0;
                            break;
                        case 12: // large straight
                            Array.Sort(dices);
                            bool isValid5 = true;
                            foreach (int j in Enumerable.Range(1, 4))
                            {
                                if (dices[j].value != dices[j - 1].value + 1) {
                                    isValid5 = false;
                                    break;
                                }
                            }
                            score = isValid5 ? 40 : 0;
                            break;
                        case 13: // yahtzee
                            score = dices.All(dice => dice.value == dices[0].value) ? 50 : 0; 
                            break;
                        case 14: // bonus
                            score = dices.Sum(dice => dice.value);
                            break;
                        }
                    }

                    Console.WriteLine("{0}점을 받습니다. 계속 하시겠습니까?(Y to continue)", score);
                    input = Console.ReadKey(true);
                    var inputChar = Char.ToUpper(input.KeyChar);
                    if (inputChar == 'Y' || input.Key == ConsoleKey.Enter) {
                        scoreboard[index] = score;
                        isScored[index] = true;
                        break;
                    }
                }
                
                
                
            }
            PrintBoard(-1);
            Console.WriteLine("Your score : " + scoreboard[15]);
            Console.ReadKey(true);
            return scoreboard[15];
        }

        private void Roll()
        {
            foreach (int i in Enumerable.Range(0, dices.Length))
                dices[i].Roll();
        }

        private void Roll(bool[] b)
        {
            foreach (int i in Enumerable.Range(0, dices.Length))
            {
                if (b[i])
                    dices[i].Roll();
            }
        }

        private void WriteALine(char character = '-', int num = 39)
        {
            foreach (int i in Enumerable.Range(0, num))
                Console.Write(character);
            Console.Write('\n');
        }

        private void RefreshBoard()
        {
            scoreboard[6] = 0;
            foreach (int i in Enumerable.Range(0, 6))
                scoreboard[6] += scoreboard[i];
            if (scoreboard[6] >= 63)
                scoreboard[7] = 35;
            scoreboard[15] = 0;
            foreach (int i in Enumerable.Range(6, 9))
                scoreboard[15] += scoreboard[i];
        }

        private void PrintBoard(int rerollLeft = 0)
        {
            RefreshBoard();
            Console.Clear();

            Console.Write(scoreboardFrame);
            int left = Console.CursorLeft, top = Console.CursorTop;
            for (int i = 0; i < scoreboard.Length; ++i)
            {
                Console.SetCursorPosition(8, 4 + i * 2);
                if (isScored[i])
                    Console.Write(scoreboard[i]);
            }
            Console.CursorLeft = left;
            Console.CursorTop = top;

            if (rerollLeft > 0)
                Console.WriteLine("\n남은 리롤 횟수 : " + rerollLeft);
            else
                Console.Write('\n');
            
            if (rerollLeft < 0)
                return;
            
            foreach (int i in Enumerable.Range(0, 5))
            {

                Console.Write(Dice.DiceDic[dices[i].value].ToString() + ' ' + dices[i].value + ' ');
                Console.Write(' ');
            }
            Console.Write("\n");
        }
    }

    /*
    -Tetris rules-

    Matrix : 10 * 20 cells
    Hard Drop : instantly drop
    Soft Drop : 20 times faster
    Bag : 7 tetriminos are packed together, and their order can be chaged
    Queue : should show next tetriminos (1 ~ 6)
    Ghost Piece : show the dropping place (On | Off)
    Hold Queue (On | Off)
    If falls to the ground, 0.5 sec to lock down

    O : yellow
    I : light blue
    T : purple
    J : orange
    L : dark blue
    S : green
    Z : red

    Starting Location : 21st and 22nd row in the middle, 3-wide minos - 4th to 6th cell
    When tetrimino is generated,
    1. drops one row immediately if not bothered
    2. able to move/rotate
    3. ghost piece appears

    Pause : Esc, F1
    Hold : Shift, C, 0
    Rotate(counter-clockwise) : Z, 3, 7
    Rotate(clockwise) : Up, X, 1, 5, 9
    Hard Drop : Space, 8
    Soft Drop : Down, 2
    Move Left : <-, 4
    Move Right : ->, 6

    ■□
    */

    class Tetrimino : ICloneable
    {
        readonly int size;
        readonly char name;

        Tetris game;
        bool[,] isBlock;
        int x, y;
        ConsoleColor color;
        public bool isOnGround { get; set; }

        public Tetrimino(Tetris game, bool[,] isBlock, int size, char name, int x, int y, ConsoleColor color)
        {
            this.size = size;
            this.name = name;

            this.game = game;
            this.isBlock = isBlock;
            this.x = x;
            this.y = y;
            this.color = color;
            this.isOnGround = false;
        }

        public char GetName()
        {
            return name;
        }

        public ConsoleColor GetColor()
        {
            return color;
        }

        public object Clone()
        {
            bool[,] newIsBlock = new bool[size, size];
            foreach (int i in Enumerable.Range(0, size))
            {
                foreach (int j in Enumerable.Range(0, size))
                    newIsBlock[i, j] = isBlock[i, j];
            }
            Tetrimino newTet = new Tetrimino(game, newIsBlock, size, name, x, y, color);
            return newTet;
        }

        public void EraseTrace()
        {
            Print('.', ConsoleColor.White);
            int currentY = y;
            while (CheckCollision() == false)
                y++;
            y--;
            Print('.', ConsoleColor.White);
            y = currentY;
        }

        public void PrintTrace()
        {
            int currentY = y;
            while (CheckCollision() == false)
                y++;
            y--;
            if (currentY == y)
                isOnGround = true;
            else
                Print('□', color);
            y = currentY;
            Print('■', color);
        }

        public void Move(Action move)
        {
            isOnGround = false;
            EraseTrace();
            move();
            PrintTrace();
        }

        public void RotateClockwise()
        {
            bool[,] newIsBlock = new bool[size, size];
            foreach (int i in Enumerable.Range(0, size))
            {
                foreach (int j in Enumerable.Range(0, size))
                    newIsBlock[j, size - i - 1] = isBlock[i, j];
            }

            var temp = isBlock;
            isBlock = newIsBlock;
            if (CheckCollision() == true)
                isBlock = temp;
        }

        public void RotateCounterClockwise()
        {
            bool[,] newIsBlock = new bool[size, size];
            foreach (int i in Enumerable.Range(0, size))
            {
                foreach (int j in Enumerable.Range(0, size))
                    newIsBlock[size - j - 1, i] = isBlock[i, j];
            }
            
            var temp = isBlock;
            isBlock = newIsBlock;
            if (CheckCollision() == true)
                isBlock = temp;
        }

        public void MoveLeft()
        {
            x--;
            if (CheckCollision() == true)
                x++;   
        }

        public void MoveRight()
        {
            x++;
            if (CheckCollision() == true)
                x--;  
        }

        public void HardDrop()
        {
            while (CheckCollision() == false)
                y++;
            y--;
        }

        public void Down()
        {
            y++;
            if (CheckCollision() == true)
                y--;
        }

        public bool CheckCollision()
        {
            foreach (int i in Enumerable.Range(0, size))
            {
                foreach (int j in Enumerable.Range(0, size))
                {
                    if (isBlock[i, j] == false)
                        continue;
                    int posX = x + i, posY = y + j;

                    if ( (posX >= 0 && posX < Tetris.WIDTH && posY >= 0 && posY < Tetris.LENGTH) == false)
                        return true;
                    if (game.GetCell(posX, posY).c != '.')
                        return true;
                }
            }
            return false;
        }

        public bool CheckBlock(int checkX, int checkY)
        {
            return isBlock[x + checkX, y + checkY];
        }

        public void PrintCoord(char c, int coordX, int coordY, ConsoleColor printingColor)
        {
            foreach (int i in Enumerable.Range(0, size))
            {
                foreach (int j in Enumerable.Range(0, size))
                {
                    if (isBlock[i, j])
                        game.PrintBlock(c, coordX + i, coordY + j, printingColor);
                }
            }
        }

        public void Print(char c, ConsoleColor printingColor)
        {
            PrintCoord(c, x, y, printingColor);
        }        

        public void SaveToMatrix()
        {
            foreach (int i in Enumerable.Range(0, size))
            {
                foreach (int j in Enumerable.Range(0, size))
                {
                    if (isBlock[i, j] == false)
                        continue;
                    int posX = x + i, posY = y + j;

                    var cell = game.GetCell(posX, posY);
                    cell.c = '■';
                    cell.color = color;
                }
            }
        }
    }

    class Cell
    {
        public char c;
        public ConsoleColor color;

        public Cell(char c, ConsoleColor color)
        {
            this.c = c;
            this.color = color;
        }
    }

    class Tetris : GameWithScoreboard
    {
        public const int WIDTH = 10, LENGTH = 40;
        const char defalultHoldingChar = ' ';
        object lockObject = new object();

        long score;
        int level, linesLeft;
        Cell[,] matrix;
        Tetrimino current;
        Queue<Tetrimino> nextQueue;
        Dictionary<char, Tetrimino> models;
        char holding;
        bool isHoldUsed, isDead, isOnSoftDrop;
        System.Timers.Timer blockDownTimer, beforeLockDownTimer, lockDownTimer, currentTimer;
        string scoredInfo;
        int scoredBefore;
        Queue<Action> actionQueue;


        public Cell GetCell(int x, int y)
        {
            return matrix[x, y];
        }

        protected override string GetLogPath(bool isMonthScore)
        {
            return TimeKiller.BASIC_PATH + @"\Tetris\" + (isMonthScore ? "MonthlyScoreboard.dat" : "Scoreboard.dat");
        }

        protected override void ResetGame()
        {
            // variables Initialization
            score = 0;
            level = 0;
            linesLeft = 10;
            holding = defalultHoldingChar;
            isHoldUsed = false;
            isDead = false;
            isOnSoftDrop = false;
            scoredInfo = "";
            scoredBefore = 0;

            // matrix Intialization
            matrix = new Cell[WIDTH, LENGTH];
            foreach (int i in Enumerable.Range(0, WIDTH))
            {
                foreach (int j in Enumerable.Range(0, LENGTH))
                    matrix[i, j] = new Cell('.', ConsoleColor.White);
            }
            
            // models Initialization
            models = new Dictionary<char, Tetrimino>();
            models['I'] = new Tetrimino(this, new bool[4, 4]{
                {false, true,  false, false},
                {false, true,  false, false},
                {false, true,  false, false},
                {false, true,  false, false},
            }, 4, 'I', 3, 18, ConsoleColor.Cyan);
            models['T'] = new Tetrimino(this, new bool[3, 3]{
                {false, true,  false},
                {true, true,  false},
                {false, true,  false},
            }, 3, 'T', 3, 18, ConsoleColor.Magenta);
            models['S'] = new Tetrimino(this, new bool[3, 3]{
                {false, true,  false},
                {true,  true,  false},
                {true,  false, false},
            }, 3, 'S', 3, 18, ConsoleColor.Green);
            models['Z'] = new Tetrimino(this, new bool[3, 3]{
                {true,  false, false},
                {true,  true,  false},
                {false, true,  false},
            }, 3, 'Z', 3, 18, ConsoleColor.Red);
            models['L'] = new Tetrimino(this, new bool[3, 3]{
                {false, true,  false},
                {false, true,  false},
                {true,  true,  false},
            }, 3, 'L', 3, 18, ConsoleColor.DarkYellow);
            models['J'] = new Tetrimino(this, new bool[3, 3]{
                {true,  true,  false},
                {false, true,  false},
                {false, true,  false},
            }, 3, 'J', 3, 18, ConsoleColor.DarkBlue);
            models['O'] = new Tetrimino(this, new bool[2, 2]{
                {true, true},
                {true, true},
            }, 2, 'O', 4, 18, ConsoleColor.Yellow);

            // queue Initialization
            nextQueue = new Queue<Tetrimino>();
            current = null;
            AddQueue();
            current = (Tetrimino)(nextQueue.Peek()).Clone();
            nextQueue.Dequeue();

            actionQueue = new Queue<Action>();
            
            // Timer Initialization
            blockDownTimer = new System.Timers.Timer(); // change interval
            blockDownTimer.Elapsed += BlockDownEvent;
            beforeLockDownTimer = new System.Timers.Timer(500);
            beforeLockDownTimer.Elapsed += BeforeLockDownEvent;
            beforeLockDownTimer.AutoReset = false;
            lockDownTimer = new System.Timers.Timer(500);
            lockDownTimer.Elapsed += LockDownEvent;
            lockDownTimer.AutoReset = false;
            currentTimer = null;
        }

        protected override long Play()
        {
            // 시작 레벨 선택, 초기화
            while ((level >= 1 && level <= 15) == false)
            {
                Console.Clear();
                Console.Write("시작 레벨 (1 ~ 15) : ");
                try {
                    level = Int32.Parse(Console.ReadLine());
                } catch (FormatException) {}
            }
            blockDownTimer.Interval = GetSpeed();
            if (level == 15)
                linesLeft = 0;
            else
                linesLeft = level * 10;
            
            PrintMatrix();
            currentTimer = blockDownTimer;
            blockDownTimer.Start();

            while (isDead == false)
            {
                if (Console.KeyAvailable) // ReadKey에 의한 쓰레드 멈춤 방지
                {
                    var input = Console.ReadKey(true).Key;

                    lock (lockObject) // queue가 thread-safe 하지 않음
                    {
                        switch (input)
                        {
                        case ConsoleKey.UpArrow:
                            actionQueue.Enqueue(Hold);
                            break;
                        case ConsoleKey.LeftArrow:
                            actionQueue.Enqueue(() => current.Move(current.MoveLeft));
                            break;
                        case ConsoleKey.RightArrow:
                            actionQueue.Enqueue(() => current.Move(current.MoveRight));
                            break;
                        case ConsoleKey.DownArrow: // soft drop
                            if (currentTimer == blockDownTimer)
                                actionQueue.Enqueue(ToggleSoftDrop);
                            break;
                        case ConsoleKey.Spacebar: // hard drop 후 바로 lock down이 되어야함
                            actionQueue.Enqueue(() => current.Move(current.HardDrop));
                            actionQueue.Enqueue(LockDown);
                            break;
                        case ConsoleKey.Z:
                            actionQueue.Enqueue(() => current.Move(current.RotateClockwise));
                            break;
                        case ConsoleKey.X:
                            actionQueue.Enqueue(() => current.Move(current.RotateCounterClockwise));
                            break;
                        case ConsoleKey.Escape:
                            if (Pause() == true)
                                return 0;
                            break;
                        defalut:
                            continue;
                        }
                        if (currentTimer == beforeLockDownTimer) {
                            beforeLockDownTimer.Stop();
                            beforeLockDownTimer.Start();
                        }
                    }
                }

                while (actionQueue.Count != 0)
                {
                    var action = actionQueue.Peek();
                    actionQueue.Dequeue();
                    action();                        
                }

                if (current.isOnGround && currentTimer == blockDownTimer) {
                    // 바닥 착지했을시
                    blockDownTimer.Stop();
                    beforeLockDownTimer.Start();
                    currentTimer = beforeLockDownTimer;
                }
                else if (current.isOnGround == false && currentTimer == beforeLockDownTimer) { 
                    // isOnGround에서 회전 등으로 벗어났을 때
                    beforeLockDownTimer.Stop();
                    blockDownTimer.Start();                     
                    currentTimer = blockDownTimer;
                }
            }
            
            currentTimer.Stop();
            
            Console.WriteLine("You Dead!");
            Console.WriteLine("Score : " + score);
            Console.ReadKey(true);

            return score;
        }

        private void ToggleSoftDrop()
        {            
            blockDownTimer.Interval = isOnSoftDrop ? blockDownTimer.Interval * 10 : blockDownTimer.Interval / 10;
            isOnSoftDrop = !isOnSoftDrop;
        }

        public void Hold()
        {
            if (isHoldUsed)
                return;
            
            if (holding == defalultHoldingChar) {
                holding = current.GetName();
                isHoldUsed = true;
                AddNewBlock(false);
            }
            else {
                char temp = current.GetName();
                current = (Tetrimino)models[holding].Clone();
                holding = temp;
            }
            isHoldUsed = true;

            PrintMatrix();
        }

        private bool Pause()
        {
            currentTimer.Stop();

            Console.Clear();
            Console.WriteLine("PAUSED!");
            Console.WriteLine("Press Esc Key To Exit");
            Console.WriteLine("Press Any Key To Continue");
            var input = Console.ReadKey(true);
            if (input.Key == ConsoleKey.Escape)
                return true;

            PrintMatrix();
            currentTimer.Start();
            return false;
        }

        private void LockDown()
        {
            Thread.Sleep(500);
            lock (lockObject)
            {
                actionQueue.Clear();
                actionQueue.Enqueue(() => AddNewBlock(true));
            }
        }

        private void PrintMatrix()
        {
            Console.Clear();
            foreach (int j in Enumerable.Range(LENGTH / 2, LENGTH / 2))
            {
                Console.Write("| ");
                foreach (int i in Enumerable.Range(0, WIDTH))
                {
                    Console.ForegroundColor = matrix[i, j].color;
                    Console.Write("" + matrix[i, j].c + ' ');
                    Console.ResetColor();
                }
                    
                Console.WriteLine("|");
            }
            foreach (int i in Enumerable.Range(0, 23))
                Console.Write("-");
            Console.WriteLine("\n\n레벨 : " + level);
            Console.WriteLine("점수 : " + score);
            Console.WriteLine("남은 라인 수 : " + linesLeft);

            Console.WriteLine("\nNext        Hold");
            var model = nextQueue.Peek();
            model.PrintCoord('■', -1, 47, model.GetColor());
            if (holding != defalultHoldingChar)
                models[holding].PrintCoord('■', 5, 47, models[holding].GetColor());
            
            Console.SetCursorPosition(30, 0);
            Console.Write("← → : 좌우 이동");
            Console.SetCursorPosition(30, 1);
            Console.Write("↓ : 빠르게 낙하");
            Console.SetCursorPosition(30, 2);
            Console.Write("↑ : Hold");
            Console.SetCursorPosition(30, 3);
            Console.Write("스페이스바 : 즉시 낙하");
            Console.SetCursorPosition(30, 4);
            Console.Write("Z : 시계방향 회전");
            Console.SetCursorPosition(30, 5);
            Console.Write("X : 시계반대방향 회전");
            Console.SetCursorPosition(30, 6);
            Console.Write("Esc : 일시정지");
            
            Console.SetCursorPosition(0, 30);
            if (scoredInfo != "") {
                Console.WriteLine(scoredInfo + "!");
                Console.WriteLine("+ " + scoredBefore);
            }
        }

        public void PrintBlock(char c, int x, int y, ConsoleColor color = ConsoleColor.White)
        {
            int cursorX = Console.CursorLeft, cursorY = Console.CursorTop;

            var pos = GetCursorPosition(x, y);
            
            if (pos.Item1 < 0 || pos.Item2 < 0)
                return;
            Console.SetCursorPosition(pos.Item1, pos.Item2);

            Console.ForegroundColor = color;
            Console.Write(c);
            Console.ResetColor();
            
            Console.SetCursorPosition(cursorY, cursorX);
        }

        public int GetSpeed()
        {
            return (int)(Math.Pow(0.8 - ((level - 1) * 0.007), level - 1) * 1000);
        }

        private void AddNewBlock(bool doSave = true)
        {
            if (doSave)
                current.SaveToMatrix();

            isDead = false;
            for (int myX = 0; myX < WIDTH; ++myX)
            {
                if (matrix[myX, 19].c != '.') {
                    isDead = true;
                    break;
                }
            }
            if (isDead)
                return;
                
            // erase fulled lines
            int erasedLines = 0;
            for (int j = 0; j < LENGTH; ++j)
            {
                for (int i = 0; i < WIDTH; ++i)
                {
                    if (matrix[i, j].c != '■')
                        break;
                    else if (i == WIDTH - 1) {
                        erasedLines++;
                        for (int y = j; y >= 1; --y)
                        {
                            for (int x = 0; x < WIDTH; ++x)
                                matrix[x, y] = matrix[x, y - 1];
                        }
                        for (int x = 0; x < WIDTH; ++x)
                            matrix[x, 0] = new Cell('.', ConsoleColor.White);
                    }
                }
            }

            // scoring
            if (erasedLines != 0) {
                switch (erasedLines)
                {
                case 1:
                    scoredBefore = level * 100;
                    scoredInfo = "Single";
                    score += scoredBefore;
                    break;
                case 2:
                    scoredBefore = level * 300;
                    scoredInfo = "Double";
                    score += scoredBefore;
                    break;
                case 3:
                    scoredBefore = level * 500;
                    scoredInfo = "Triple";
                    score += scoredBefore;
                    break;
                case 4:
                    scoredBefore = level * 800;
                    scoredInfo = "Tetris";
                    score += scoredBefore;
                    break;
                }
                
                if (level != 15) {
                    linesLeft -= erasedLines;
                    if (linesLeft <= 0) {
                        level++;
                        blockDownTimer.Interval = GetSpeed();
                        if (level == 15)
                            linesLeft = 0;
                        else
                            linesLeft += 10;
                    }
                }
                
            }

            currentTimer.Stop();
            blockDownTimer.Start();
            currentTimer = blockDownTimer;

            current = (Tetrimino)(nextQueue.Peek()).Clone();
            nextQueue.Dequeue();
            if (nextQueue.Count == 1)
                AddQueue();
            isHoldUsed = false;

            PrintMatrix();
            actionQueue.Clear();
            if (isOnSoftDrop)
                ToggleSoftDrop();
            return;
        }

        private void AddQueue()
        {
            var values = models.Values;

            Random randomSeed = new Random();
            var randomizedArray = values.OrderBy(x => randomSeed.Next()).ToArray();
            foreach (var tet in randomizedArray)
                nextQueue.Enqueue(tet);
        }

        public static Tuple<int, int> GetCursorPosition(int x, int y)
        {
            return new Tuple<int, int> (x * 2 + 2, y - 20);
        }

        private void BlockDownEvent(object source, ElapsedEventArgs e)
        {
            lock (lockObject)
            {
                actionQueue.Enqueue(() => current.Move(current.Down));
            }
        }

        private void LockDownEvent(Object source, ElapsedEventArgs e)
        {
            lock (lockObject)
            {
                actionQueue.Enqueue(() => AddNewBlock(true));
            }
        }

        private void BeforeLockDownEvent(Object source, ElapsedEventArgs e)
        {
            lock (lockObject)
            {
                actionQueue.Clear();
                beforeLockDownTimer.Stop();
                LockDown();
            }
        }
    }
}

/*

lock 대상 :
actionQueue에 넣는 동작만


TO-DO List

* SRS 추가? (안할수도)
3. Back To Back, T-spin 등 점수 관련 추가
12. 리펙토링
18. Lock Down 추가 (땅 위에 착지 후 0.5초간 못건드리는 상태)
19. Extended Placement 추가 (땅 위에서 0.5초간 움직일 수 있음, 움직이면 리셋)



Solved List

1. BlockDownEvent와 Move가 겹치면 블럭 잔상이 남음
2. 사망 처리
4. Enter 눌러야 다음 블럭으로 넘어감
4-1. 바닥에 닿았을 시의 상태에서 회전시 그 상태를 벗어날 가능성 있음
5. Esc 누르면 Pause 기능
6. 미리보기 기능 (Ghost Piece)
7. 키 설명 추가/표시
8. Soft Drop 추가 (x20 spd, DownArrow key)
9. Hard Drop 키 변경 (Space Key)
10. isOnGround가 Down에서 수정되게
11. Hold시 저장(SaveMatrix)이 안됨..! + Hold칸 이미지 수정이 안됨
13. MoveDown시 0.5초 후(이동x) 바로 AddNewBlock (타이머 x)
14. level 증가, level마다 Interval 감소
15. score 저장기능
16. action queue
17. MoveDown시 이후에 가끔 조금 내려가고 바로 저장됨.
-> AddNewBlock 이후 actionQueue.Clear 실행으로 고쳐졌을수도?


가이드라인 링크
https://www.dropbox.com/s/g55gwls0h2muqzn/tetris%20guideline%20docs%202009.zip?dl=0
*/
