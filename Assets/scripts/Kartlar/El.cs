using System.Collections.Generic;

public class El
{
    public List<Kart> kartlar = new List<Kart>();

    public void Ekle(Kart k) => kartlar.Add(k);

    public int Skor()
    {
        int skor = 0;
        int aceCount = 0;

        foreach (var k in kartlar)
        {
            skor += k.Deger;
            if (k.Rank == CardRank.Ace) aceCount++;
        }

        while (skor > 21 && aceCount > 0)
        {
            skor -= 10;
            aceCount--;
        }

        return skor;
    }
}
