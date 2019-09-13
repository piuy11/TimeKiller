using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using General;

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


}
