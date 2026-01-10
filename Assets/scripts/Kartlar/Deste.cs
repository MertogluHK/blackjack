using System.Collections.Generic;
using UnityEngine;

public class Deste
{
    readonly List<Kart> kartlar = new List<Kart>();

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
#if UNITY_EDITOR
        Debug.Log($"[DECK] {deckCount} decks, {kartlar.Count} cards.");
#endif
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

        // O(1) çekmek için sondan çekebiliriz:
        int last = kartlar.Count - 1;
        Kart k = kartlar[last];
        kartlar.RemoveAt(last);
        return k;
    }
}