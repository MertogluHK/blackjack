using System.Collections.Generic;
using UnityEngine;
using static Kart;

public class Deste
{
    List<Kart> kartlar = new List<Kart>();

    public Deste(int desteSayisi)
    {
        Olustur(desteSayisi);
        Karistir();
    }

    void Olustur(int desteSayisi)
    {
        kartlar.Clear();

        for (int i = 0; i < desteSayisi; i++)
        {
            foreach (KartTur tur in System.Enum.GetValues(typeof(KartTur)))
            {
                foreach (KartRank rank in System.Enum.GetValues(typeof(KartRank)))
                {
                    kartlar.Add(new Kart(rank, tur));
                }
            }
        }
    }

    void Karistir()
    {
        for (int i = 0; i < kartlar.Count; i++)
        {
            int r = Random.Range(i, kartlar.Count);
            (kartlar[i], kartlar[r]) = (kartlar[r], kartlar[i]);
        }
    }

    public Kart KartCek()
    {
        Kart kart = kartlar[0];
        kartlar.RemoveAt(0);
        return kart;
    }

    public int KalanKart() => kartlar.Count;
}
