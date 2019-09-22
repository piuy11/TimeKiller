using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using General;

namespace TimeKiller
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

        public static Dictionary<Suit, string> SuitDic = new Dictionary<Suit, string>(){
            {Suit.Spade, "♠"}, {Suit.Heart, "♥"}, 
            {Suit.Diamond, "♦"}, {Suit.Clover, "♣"}
        };
        public static Dictionary<Rank, string> RankDic = new Dictionary<Rank, string>(){
            {Rank.A, "A"}, {Rank.Two, "2"}, {Rank.Three, "3"}, 
            {Rank.Four, "4"}, {Rank.Five, "5"}, {Rank.Six, "6"}, 
            {Rank.Seven, "7"}, {Rank.Eight, "8"}, {Rank.Nine, "9"}, 
            {Rank.Ten, "10"}, {Rank.J, "J"}, {Rank.Q, "Q"}, {Rank.K, "K"}
        };


        public Card(int value)
        {
            this.value = value;
            this.suit = (Suit)(value % 4);
            this.rank = (Rank)(value / 4);
            this.name = RankDic[this.rank] + SuitDic[this.suit];
        }

        public static bool IsNearRank(Rank a, Rank b)
        {
            int diff = Math.Abs(a - b);
            return diff == 1 || diff == 12;
        }
    }

    class Deck
    {
        private List<Card> cards, usedCards;
        private Random randomSeed;

        public Deck()
        {
            cards = new List<Card>();
            usedCards = new List<Card>();
            randomSeed = new Random();
            
            foreach (var i in Enumerable.Range(0, 52))
            {
                cards.Add(new Card(i));
            }
        }

        public Card Pick()
        {
            if (cards.Count == 0)
                General.General.Swap(ref cards, ref usedCards);

            int index = randomSeed.Next(0, cards.Count);
            usedCards.Add(cards[index]);
            cards.RemoveAt(index);
            return usedCards[usedCards.Count - 1];
        }

        public void PrintDeck()
        {
            foreach (var card in cards)
            {
                Console.Write(card.name + ' ');
            }
        }
    }
    // ⚀⚁⚂⚃⚄⚅
    class Dice : Icomparable
    {
        private Random randomSeed;
        public int value { get; private set; }

        public static Dictionary<int, char> DiceDic = new Dictionary<int, char>(){
            {1, '⚀'}, {2, '⚁'}, {3, '⚂'}, {4, '⚃'}, {5, '⚄'}, {6, '⚅'}, 
        };

        public Dice()
        {
            randomSeed = new Random();
            Roll();
        }

        public int Roll()
        {
            value = randomSeed.Next(1, 7);
            return value;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
        
            Dice other = obj as Dice;
            if (other != null) 
                return this.value.CompareTo(other.value);
            else
                throw new ArgumentException("Object is not a Dice");
        }
    }
    /*
    class Post
    {
        public int index { get; private set; }
        public bool isHidden { get; private set; }
        public string name { get; private set; }
        public string content { get; private set; }
        public string password { get; private set; }
        public List<Comment> comments { get; private set; }

        public Post(int index, bool isHidden, stirng name, string content, string password, List<Comment> comments)
        {
            this.index = index;
            this.isHidden = isHidden;
            this.name = name;
            this.content = content;
            this.password = password;
            this.comments = comments;
        }
    }

    class Comment
    {

    }
    */
}
