using System;
using System.IO;
using System.Linq;

namespace TimeKiller
{
    class TimeKiller
    {
        public const string GAME_VERSION = "test_v1.0";
        public const string DataPath = @"c:\Program Files\TimeKiller";
        
        static void Main()
        {
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
        
        public static void Game()
        {
            Intro();
        }
        
        public static long Intro()
        {
            if (!File.Exists(TimeKiller.DataPath))
                Console.WriteLine("No Path Found!!");
                
            /*
            FileStream fs = new FileStream("D:/data.bin", FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
 
            int i = br.ReadInt32();
            float f = br.ReadSingle();
            double d = br.ReadDouble();
            string str = br.ReadString();
 
            MessageBox.Show(i + " " + f + " " + d + " " + str);
 
            br.Close();
            fs.Close();
            */
            do
            {
                Console.Clear();
                Console.WriteLine("부자가 되어보자 " + GAME_VERSION);
                Console.WriteLine("1. 새로하기");
                Console.WriteLine("2. 이어하기");
                Console.WriteLine("3. 점수판");
                Console.WriteLine("4. 종료");
                
                char choice = Console.ReadKey().KeyChar;
                switch(choice)
                {
                    case '1':
                        return StartMoney;
                    case '2':
                        return PlayContinue();
                    case '3':
                        ScoreBoard();
                        break;
                    case '4':
                        return -1;
                }
                    
            } while (true);
            
        }
        
        static long PlayContinue()
        {
            return 0;
        }
        
        static void ScoreBoard()
        {
            Console.WriteLine("");
        }
        
    }
}
