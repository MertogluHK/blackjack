using System.Collections.Generic;
using UnityEngine;

public class Deste
{
    List<Kart> kartlar = new List<Kart>();

    int desteSayisi;
    int karistirEsigi; // örn: 100

    public Deste(int desteSayisi, int karistirEsigi = 100)
    {
        this.desteSayisi = desteSayisi;
        this.karistirEsigi = karistirEsigi;

        YenidenOlusturVeKaristir();
    }

    void YenidenOlusturVeKaristir()
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

        Karistir();
        Debug.Log($"[DESTE] Yeni deste olusturuldu: {desteSayisi} deste, toplam {kartlar.Count} kart.");
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
        // Kart çekmeden önce eşiği kontrol et:
        // (<= 100 ise yeni 4 destelik shoe oluştur ve karıştır)
        if (kartlar.Count <= karistirEsigi)
        {
            Debug.Log($"[DESTE] Kart sayisi {kartlar.Count} (esik {karistirEsigi}). Yeniden karistiriliyor...");
            YenidenOlusturVeKaristir();
        }

        Kart kart = kartlar[0];
        kartlar.RemoveAt(0);
        return kart;
    }

    public int KalanKart() => kartlar.Count;
}
