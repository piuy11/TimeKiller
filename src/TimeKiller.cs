using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

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

    class Tetrimino
    {
        bool[,] isBlock;
        readonly int size;
        readonly char name;
        int x, y;
        ConsoleColor color;

        public Tetrimino(bool[,] isBlock, int size, char name, int x, int y, ConsoleColor color)
        {
            this.isBlock = isBlock;
            this.size = size;
            this.name = name;
            this.x = x;
            this.y = y;
            this.color = color;
        }

        public void Rotate(bool isLeft)
        {

        }

        public bool CheckBlock(int checkX, int checkY)
        {
            return isBlock[x + checkX, y + checkY];
        }

        public void Print(char c)
        {
            int cursorX = Console.CursorLeft, cursorY = Console.CursorTop;

            // Console.WriteLine("{0}, {1}", cursorX, cursorY);

            // Erase printed blocks before
            foreach (int i in Enumerable.Range(0, size))
            {
                var pos = Tetris.GetCursorPosition(x + i, y - 1);
                if (pos.Item1 < 0 || pos.Item2 < 0)
                    continue;
                Console.SetCursorPosition(pos.Item1, pos.Item2);
                Console.Write('.');
            }

            foreach (int i in Enumerable.Range(0, size))
            {
                foreach (int j in Enumerable.Range(0, size))
                {
                    if (isBlock[i, j]) {
                        var pos = Tetris.GetCursorPosition(x + i, y + j);
                        // Console.Write("position : {0}, {1}", x + i, y + j);
                        // Console.Write("cursor : {0}, {1}", pos.Item1, pos.Item2);
                        if (pos.Item1 < 0 || pos.Item2 < 0)
                            continue;
                        Console.SetCursorPosition(pos.Item1, pos.Item2);
                        Console.ForegroundColor = color;
                        Console.Write(c);
                        Console.ResetColor();
                    }
                }
            }

            // Console.CursorLeft = cursorX;
            // Console.CursorTop  = cursorY;
            // Console.SetCursorPosition(cursorX, cursorY);
            // Console.Write("x : {0}, y : {1}", Console.CursorLeft, Console.CursorTop);
            Console.SetCursorPosition(cursorY, cursorX);
            
        }

        public void Down()
        {
            y++;
        }
    }

    class Tetris : GameWithScoreboard
    {
        public const int WIDTH = 10, LENGTH = 40;
        char[,] matrix;
        Tetrimino current;
        Queue<Tetrimino> nextQueue;
        Dictionary<char, Tetrimino> models;

        protected override string GetLogPath(bool isMonthScore)
        {
            return TimeKiller.BASIC_PATH + @"\Tetris\" + (isMonthScore ? "MonthlyScoreboard.dat" : "Scoreboard.dat");
        }

        protected override void ResetGame()
        {
            matrix = new char[WIDTH, LENGTH];
            foreach (int i in Enumerable.Range(0, WIDTH))
            {
                foreach (int j in Enumerable.Range(0, LENGTH))
                    matrix[i, j] = '.';
            }
            // x, y changed!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            models = new Dictionary<char, Tetrimino>();
            models['I'] = new Tetrimino(new bool[4, 4]{
                {false, false, false, false},
                {false, false, false, false},
                {false, false, false, false},
                {true,  true,  true,  true},
            }, 4, 'I', 3, 17, ConsoleColor.Cyan);
            models['T'] = new Tetrimino(new bool[3, 3]{
                {false, false, false},
                {false, true,  false},
                {true,  true,  true},
            }, 3, 'T', 3, 18, ConsoleColor.Magenta);
            models['S'] = new Tetrimino(new bool[3, 3]{
                {false, false, false},
                {false, true,  true},
                {true,  true,  false},
            }, 3, 'S', 3, 18, ConsoleColor.Green);
            models['Z'] = new Tetrimino(new bool[3, 3]{
                {false, false, false},
                {true,  true,  false},
                {false, true,  true},
            }, 3, 'Z', 3, 18, ConsoleColor.Red);
            models['L'] = new Tetrimino(new bool[3, 3]{
                {false, false, false},
                {false, false, true},
                {true,  true,  true},
            }, 3, 'L', 3, 18, ConsoleColor.DarkYellow);
            models['J'] = new Tetrimino(new bool[3, 3]{
                {false, false, false},
                {true,  false, false},
                {true,  true,  true},
            }, 3, 'J', 3, 18, ConsoleColor.DarkBlue);
            models['O'] = new Tetrimino(new bool[2, 2]{
                {true, true},
                {true, true},
            }, 2, 'O', 4, 19, ConsoleColor.Yellow);

            nextQueue = new Queue<Tetrimino>();
            nextQueue.Enqueue(models['I']);
            nextQueue.Enqueue(models['T']);
            nextQueue.Enqueue(models['O']);
            nextQueue.Enqueue(models['J']);
            nextQueue.Enqueue(models['L']);
            nextQueue.Enqueue(models['S']);
            nextQueue.Enqueue(models['Z']);

            
        }

        protected override long Play()
        {
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += BlockDownEvent;

            timer.Start();

            PrintMatrix();
            AddNewBlock();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var input = Console.ReadKey(true).Key;
                    if (input == ConsoleKey.Enter)
                        AddNewBlock();
                }
            }
            
            timer.Stop();
            timer.Dispose();
            
            // inputTask.Dispose();
            Console.WriteLine("End!");

            return 0;
        }

        private void PrintMatrix()
        {
            Console.Clear();
            foreach (int i in Enumerable.Range(LENGTH / 2, LENGTH / 2))
            {
                Console.Write("| ");
                foreach (int j in Enumerable.Range(0, WIDTH))
                    Console.Write("" + matrix[j, i] + ' ');
                    
                Console.WriteLine("|");
            }
            foreach (int i in Enumerable.Range(0, 23))
                Console.Write("-");
        }

        private void AddNewBlock()
        {
            current = nextQueue.Peek();
            nextQueue.Dequeue();
        }

        public static Tuple<int, int> GetCursorPosition(int x, int y)
        {
            return new Tuple<int, int> ( x * 2 + 2, y - 20);
        }

        private void BlockDownEvent(Object source, ElapsedEventArgs e)
        {
            current.Print('.');
            current.Down();
            current.Print('■');
        }        
    }
}
