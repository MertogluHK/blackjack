//public enum CardRank
//{
//Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace
//}

public enum CardRank
{
    Ten, Jack, Queen, King, Ace
}

public enum CardSuit
{
    Diamond, Heart, Club, Spade
}

public class Kart
{
    public CardRank Rank;
    public CardSuit Suit;

    // Blackjack değeri
    public int Deger
    {
        get
        {
            // as her zaman 11 olarak başlar
            if (Rank == CardRank.Ace)
                return 10;

            // 10, Jack, Kız, King → 10
            if (Rank == CardRank.Ten ||
                Rank == CardRank.Jack ||
                Rank == CardRank.Queen ||
                Rank == CardRank.King)
                return 10;

            // Two(0) → 2, Three(1) → 3, ...
            return (int)Rank + 2;
        }
    }

    public Kart(CardRank rank, CardSuit tur)
    {
        Rank = rank;
        Suit = tur;
    }
}

