using System.Collections.Generic;
using UnityEngine;

public class Deste
{
    List<Kart> kartlar = new List<Kart>();

    int deckCount;
    int shuffleThreshold;

    public Deste(int deckCount, int shuffleThreshold = 100)
    {
        this.deckCount = deckCount;
        this.shuffleThreshold = shuffleThreshold;
        RebuildAndShuffle();
    }

    void RebuildAndShuffle()
    {
        kartlar.Clear();

        for (int i = 0; i < deckCount; i++)
        {
            foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in System.Enum.GetValues(typeof(CardRank)))
                {
                    kartlar.Add(new Kart(rank, suit));
                }
            }
        }

        Shuffle();
        Debug.Log($"[DECK] {deckCount} decks, {kartlar.Count} cards.");
    }

    void Shuffle()
    {
        for (int i = 0; i < kartlar.Count; i++)
        {
            int r = Random.Range(i, kartlar.Count);
            (kartlar[i], kartlar[r]) = (kartlar[r], kartlar[i]);
        }
    }

    public Kart Draw()
    {
        if (kartlar.Count <= shuffleThreshold)
            RebuildAndShuffle();

        Kart k = kartlar[0];
        kartlar.RemoveAt(0);
        return k;
    }
}
