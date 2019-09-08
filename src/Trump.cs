using System;
using System.Linq;

namespace Trump
{
    public enum Suit
    {
        Spade,
        Heart,
        Diamond,
        Clover
    }

    public enum Rank
    {
        A,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        J,
        Q,
        K
    }

    class Card
    {
        public int value {get;}
        public Suit suit {get;}
        public Rank rank {get;}
        public string name {get;}

        public static char[] SuitChar = {'♠', '♥', '♦', '♣'};
        public static string[] RankStr = {
            "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"
        };


        public Card(int value)
        {
            this.value = value;
            this.suit = (Suit)(value % 4);
            this.rank = (Rank)(value / 4);
            this.name = RankStr[(int)this.rank] + SuitChar[(int)this.suit];
        }
    }

    class Deck
    {
        private Card[] cards;

        public Deck()
        {
            cards = new Card[52];
            foreach (var i in Enumerable.Range(0, 52))
            {
                cards[i] = new Card(i);
            }
        }

        public void PrintDeck()
        {
            foreach (var card in cards)
            {
                Console.Write(card.name + ' ');
            }
        }
    }


}
