public enum KartRank
{
    Iki, Uc, Dort, Bes, Alti, Yedi, Sekiz, Dokuz, On, Vale, Kiz, Papaz, As
}

public enum KartTur
{
    Karo, Kupa, Maca, Sinek
}

public class Kart
{
    public KartRank Rank;
    public KartTur Tur;

    public Kart(KartRank rank, KartTur tur)
    {
        Rank = rank;
        Tur = tur;
    }
}
