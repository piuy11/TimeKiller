using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace TimeKiller
{
    class Yahtzee_AI
    {


        static void Main()
        {
            while (true)
            {
                Yahtzee.Play();
            }
        }
    }

    class Dice
    {
        private Random randomSeed;
        public int value { get; }

        public Dice()
        {
            randomSeed =  = new Random();
            Roll();
        }

        public int Roll()
        {
            value = randomSeed.Next(1, 6);
            return value;
        }
    }

    class Yahtzee
    {
        private Dice[] dices;
        public bool isNonPrinting { get; }

        public Yahtzee(bool b = false)
        {
            dices = new Dice[5];
            foreach (int i in Enumerable.Range(5))
                dice[i] = new Dice();
            this.isNonPrinting = b;
        }

        public void Play()
        {
            foreach (int i in Enumerable.Range(13))
            {
                Roll();
            }
        }

        private void Roll();
        {
            foreach (int i in Enumerable.Range(5))
                dice[i].Roll();
        }
    }
}
