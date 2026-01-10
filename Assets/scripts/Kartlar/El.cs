using System.Collections.Generic;

public class El
{
    public List<Kart> kartlar = new List<Kart>();

    public void Ekle(Kart k)
    {
        if (k != null) kartlar.Add(k);
    }

    public int Skor()
    {
        int skor = 0;
        int aceCount = 0;

        foreach (var k in kartlar)
        {
            if (k == null) continue;

            skor += k.Deger;
            if (k.Rank == CardRank.Ace) aceCount++;
        }

        // Aslarý 11 -> 1'e düþür
        while (skor > 21 && aceCount > 0)
        {
            skor -= 10;
            aceCount--;
        }

        return skor;
    }
}
