using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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
                Console.WriteLine("2. 숫자야구");
                Console.WriteLine("3. 로또 추첨기");
                Console.WriteLine("4. 오목");
				Console.WriteLine("5. 2048");
                Console.WriteLine("6. 블랙홀");
                Console.WriteLine("7. 야찌");
                // Console.WriteLine("8. 게시판");
                
                switch(Console.ReadKey(true).KeyChar)
                {
                    case '1':
                        Log("부자가 되어보자");
                        BeARich.Play();
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
						A2048 game5 = new A2048();
						game5.Play();
						break;
                    case '6':
                        Log("블랙홀");
                        Blackhole game6 = new Blackhole();
                        game6.Play();
                        break;
                    case '7':
                        Log("야찌");
                        Game yahtzee = new Yahtzee();
                        yahtzee.Menu();
                        break;
                    /*
                    case '8':
                        Log("게시판");
                        BulletinBoard game8 = new BulletinBoard();
                        game8.Play();
                    */
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

    abstract class Game
    {
        public virtual void Menu()
        {
            ConsoleKeyInfo input;
            do {
                Console.Clear();
                Console.WriteLine("1. 게임 시작");
                Console.WriteLine("2. 게임 설명");
                Console.WriteLine("Esc 키를 눌러 나갑니다.");
                input = Console.ReadKey(true);

                if (input.KeyChar == '1')
                    Play();
                else if (input.KeyChar == '2')
                    Description();
            } while (input.Key != ConsoleKey.Escape);
        }

        protected abstract int Play();

        protected virtual void Description()
        {
            Console.Clear();
            Console.WriteLine("게임 설명이 없습니다..");
            TimeKiller.Continue();
        }
    }

    abstract class GameWithScoreBoard : Game
    {
        Tuple<string, long>[] allTimeScores, monthlyScores;

        public override void Menu()
        {
            FirstSet();
            
            ConsoleKeyInfo input;
            do {
                Console.Clear();
                Console.WriteLine("1. 게임 시작");
                Console.WriteLine("2. 게임 설명");
                Console.WriteLine("3. 점수판");
                Console.WriteLine("Esc 키를 눌러 나갑니다.");
                input = Console.ReadKey(true);

                if (input.KeyChar == '1') {
                    int score = Play();
                    // score something
                }
                else if (input.KeyChar == '2')
                    Description();
                else if (input.KeyChar == '3')
                    ShowScoreBoard();
            } while (input.Key != ConsoleKey.Escape);
            
        }

        protected void FirstSet()
        {
            // All-Time Score
            allTimeScores = new Tuple<string, long>[10];
            if ( !File.Exists(GetLogPath(false)) ) {
                BinaryWriter bw = new BinaryWriter(File.Open(GetLogPath(false), FileMode.CreateNew));
                bw.Close();
            }
            using ( BinaryReader bw = new BinaryReader(File.Open(GetLogPath(false), FileMode.Open)) )
            {
                int i = 0;
                for (i = 0; i < 10; ++i)
                {
                    if (bw.BaseStream.Length == bw.BaseStream.Position)
                        break;
                    string name = bw.ReadString();
                    long score = bw.ReadInt64();
                    allTimeScores[i] = new Tuple<string, int>(name : name, score : score);
                }
                for (int j = i; j < 10; ++j)
                    allTimeScores[j] = (name : "", score : 0L);
            }

            // Monthly Score
            monthlyScore = new Tuple<string, long>[10];
            if ( !File.Exists(GetLogPath(true)) ) {
                BinaryWriter bw = new BinaryWriter(File.Open(GetLogPath(true), FileMode.CreateNew));
                bw.Close();
            }
            using ( BinaryReader bw = new BinaryReader(File.Open(GetLogPath(true), FileMode.Open)) )
            {
                int i = 0;
                try {
                    for (i = 0; i < 10; ++i)
                    {
                        string name = bw.ReadString();
                        long score = bw.ReadInt64();
                        monthlyScore[i] = new Tuple<string, long>(name, score);
                    }
                }
                catch (EndOfStreamException) {
                    for (int j = i; j < 10; ++j)
                        monthlyScore[j] = new Tuple<string, long>("", 0L);
                }
            }
        }

        protected abstract string GetLogPath(bool isMonthScore = false);

        protected void ShowScoreBoard()
        {
            Console.Clear();
            Console.WriteLine("점수판\n");
            int i = 1;
            foreach (var tuple in score)
            {
                Console.WriteLine(i.ToString() + ' ' + tuple.Item2 + ' ' + tuple.Item1);
                i++;
            }
            Console.ReadKey(true);
        }

        protected void WriteScoreBoard(bool isMonthScore)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(GetLogPath(isMonthScore), FileMode.CreateNew)))
            {
                foreach (var tuple in score)
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
        
        public static int Play()
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
                        return 0;
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
                catch (EndOfStreamException) {
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
            catch (FormatException) {
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

    class Blackhole
    {
        private Card[,] board;
        private List<Card> blackhole;
        private int[] cardLeft;

        public Blackhole()
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

        public void Play()
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
                    return;
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

    class Yahtzee : GameWithScoreBoard
    {
        private Dice[] dices;
        private int[] scoreboard;
        private bool[] isScored;
        private bool isAIMode;
        private const string scoreboardFrame = @"
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

        public Yahtzee()
        {
            
        }

        private void FirstSet(bool b = false)
        {
            dices = new Dice[5];
            foreach (int i in Enumerable.Range(0, dices.Length))
                dices[i] = new Dice();

            scoreboard = new int[16];
            scoreboard.Initialize();
            isScored = new bool[16];
            isScored.Initialize();
            isScored[6] = isScored[7] = isScored[15] = true;
            
            this.isAIMode = b;
        }

        protected override int Play()
        {
            FirstSet();

            foreach (int i in Enumerable.Range(0, 13))
            {
                Roll();
                int rerollLeft = 2;
                while (rerollLeft != 0)
                {
                    bool[] needReroll = GetReroll(rerollLeft);
                    if (needReroll.All(b => false))
                        break;
                    Roll(needReroll);
                    --rerollLeft;
                }
                while (true) // choose valid
                {
                    PrintBoard();
                    Console.WriteLine("주사위 값을 넣을 곳을 선택해 주세요. (A ~ M)");
                    
                    char choice = Char.ToUpper(Console.ReadKey(true).KeyChar);
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
                    var input = Char.ToUpper(Console.ReadKey(true).KeyChar);
                    if (input == 'Y') {
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

        private bool[] GetReroll(int rerollLeft)
        {
            ConsoleKeyInfo input;
            bool[] b = new bool[dices.Length];
            b.Initialize();

            while (true)
            {
                PrintBoard(rerollLeft);
                foreach (int i in Enumerable.Range(0, b.Length))
                {
                    if (b[i])
                        Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write(i + 1);
                    if (b[i])
                        Console.ResetColor();
                    Console.Write("    ");
                }
                
                Console.Write('\n');
                input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.Enter)
                    break;
                else if ( (input.KeyChar >= '1' && input.KeyChar <= '5') == false)
                    continue; 

                int index = input.KeyChar - '1';
                b[index] = !b[index];

            }

            return b;
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
}

// TODO : 카드게임들, 스도쿠제작기
