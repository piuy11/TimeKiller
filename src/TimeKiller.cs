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
                
                switch(Console.ReadKey().KeyChar)
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
        
        
        private static Tuple<string, long>[] scoreBoard = new Tuple<string, long>[10];
        
        public static void Game()
        {
            BasicSet();
            Intro();
        }
        
        private static void BasicSet()
        {
            if (!File.Exists(SCOREBOARD_PATH)) {
                BinaryWriter bw = new BinaryWriter(File.Open(SCOREBOARD_PATH, FileMode.CreateNew));
                bw.Write("홍민준");
                bw.Write(10000L);
                bw.Close();
            }
                using (BinaryReader bw = new BinaryReader(File.Open(SCOREBOARD_PATH, FileMode.Open)))
                {
                    try {
                        for (int i = 0; i < 10; ++i)
                        {
                            string name = bw.ReadString();
                            long score = bw.ReadInt64();
                            scoreBoard[i] = new Tuple<string, long>(name, score);
                        }
                    }
                    catch (EndOfStreamException e) {
                        // e();
                    }
                }
        }
        
        public static long Intro()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("부자가 되어보자 " + GAME_VERSION);
                Console.WriteLine("1. 새로하기");
                Console.WriteLine("2. 이어하기");
                Console.WriteLine("3. 점수판");
                Console.WriteLine("4. 종료");
                
                Console.WriteLine(@"{0} 패치 내역
                1", GAME_VERSION);
                
                char choice = Console.ReadKey(true).KeyChar;
                switch(choice)
                {
                    case '1':
                        return StartMoney;
                    case '2':
                        return PlayContinue();
                    case '3':
                        ShowScoreBoard();
                        break;
                    case '4':
                        return -1;
                }
                    
            } while (true);
            
        }
        
        static long PlayNew()
        {
            return 0;
        }
        
        static long PlayContinue()
        {
            return 0;
        }
        
        static void ShowScoreBoard()
        {
            Console.Clear();
            Console.WriteLine("점수판 : " + scoreBoard.Length);
            foreach (var tuple in scoreBoard)
            {
                Console.Write(tuple);
            }
            Console.ReadKey(true);
        }
        
    }
}
