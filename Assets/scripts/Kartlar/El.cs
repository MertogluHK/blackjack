using System.Collections.Generic;

public class El
{
    public List<Kart> kartlar = new List<Kart>();

    public void Ekle(Kart k)
    {
        kartlar.Add(k);
    }

    public int Skor()
    {
        int skor = 0;
        int asSayisi = 0;

        foreach (var k in kartlar)
        {
            int v = Deger(k);
            skor += v;
            if (k.Rank == KartRank.As) asSayisi++;
        }

        while (skor > 21 && asSayisi > 0)
        {
            skor -= 10;
            asSayisi--;
        }

        return skor;
    }

    int Deger(Kart k)
    {
        if (k.Rank == KartRank.As) return 11;
        if (k.Rank == KartRank.Vale || k.Rank == KartRank.Kiz || k.Rank == KartRank.Papaz) return 10;
        return (int)k.Rank + 2;
    }
}
