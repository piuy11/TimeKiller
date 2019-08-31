using System;
using System.IO;
using System.Linq;

namespace TimeKiller
{
    class TimeKiller
    {
        public const string GAME_VERSION = "test_v1.0";
        public const string BASIC_PATH = @"c:\Program Files\TimeKiller";
        
        static void BasicSet()
        {
            Directory.CreateDirectory(BASIC_PATH);
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
                
                switch(Console.ReadKey(true).KeyChar)
                {
                    case '1':
                        BeARich.Game();
                        break;
                    case '2':
                        // NumberBaseBall.Game();
                        break;
                    case '3':
                        // Lottery.Game();
                        break;
                    case '4':
                        // Renju.Game();
                        break;
                }
            }
        }
    }
    
    class BeARich
    {
        public const long StartMoney = 1000;
        public const string GAME_VERSION = "test_v1.0";
        public const string BASIC_PATH = TimeKiller.BASIC_PATH + @"\BeARich";
        public const string SCOREBOARD_PATH = BASIC_PATH + @"\scoreborad.dat";
        public const string SAVE_PATH = BASIC_PATH + @"save.dat";
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
                Console.WrtieLine("6. 구구단 맞추기 : + 100");
            } while (money > 0);
        }
        
        static void PlayContinue()
        {
            long money;
            using (BinaryReader br = new BinaryReader(File.Open(SAVE_PATH, FileMode.Open)))
            {
                money = br.ReadInt64();
            }
            return 0;
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
        
        static void Gamble()
        {
            
        }
    }
}
