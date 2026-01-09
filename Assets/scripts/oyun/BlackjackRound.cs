using System.Collections.Generic;

public class BlackjackRound
{
    public Deste deck;
    public List<El> playerHands = new List<El>();
    public El dealerHand = new El();

    public int activeHandIndex { get; private set; } = 0;

    public List<int> bets = new List<int>();
    public List<bool> doubled = new List<bool>();
    public List<bool> stood = new List<bool>();

    public BlackjackRound(int deckCount, int shuffleThreshold, int baseBet)
    {
        deck = new Deste(deckCount, shuffleThreshold);
        dealerHand = new El();

        playerHands.Clear();
        bets.Clear();
        doubled.Clear();
        stood.Clear();

        playerHands.Add(new El());
        bets.Add(baseBet);
        doubled.Add(false);
        stood.Add(false);

        activeHandIndex = 0;
    }

    public void SetActiveHand(int i) => activeHandIndex = i;

    public Kart DealToPlayer(int handIndex)
    {
        Kart k = deck.Draw();
        playerHands[handIndex].Ekle(k);
        return k;
    }

    public Kart DealToDealer()
    {
        Kart k = deck.Draw();
        dealerHand.Ekle(k);
        return k;
    }

    public Kart Hit(int handIndex) => DealToPlayer(handIndex);

    public void Stand(int handIndex) => stood[handIndex] = true;

    public bool CanDoubleDown(int handIndex)
    {
        var h = playerHands[handIndex];
        return h.kartlar.Count == 2 && !doubled[handIndex] && !stood[handIndex];
    }

    public Kart DoubleDown(int handIndex)
    {
        if (!CanDoubleDown(handIndex)) return null;

        doubled[handIndex] = true;
        bets[handIndex] *= 2;

        return Hit(handIndex);
    }

    // değer bazlı split (10/J/Q/K hepsi 10)
    public bool CanSplit(int handIndex)
    {
        var h = playerHands[handIndex];
        if (h.kartlar.Count != 2) return false;
        if (stood[handIndex]) return false;

        return h.kartlar[0].Deger == h.kartlar[1].Deger;
    }

    public bool Split(int handIndex)
    {
        if (!CanSplit(handIndex)) return false;

        var h = playerHands[handIndex];
        Kart a = h.kartlar[0];
        Kart b = h.kartlar[1];

        h.kartlar.Clear();
        h.Ekle(a);

        El newHand = new El();
        newHand.Ekle(b);

        playerHands.Insert(handIndex + 1, newHand);

        bets.Insert(handIndex + 1, bets[handIndex]);
        doubled.Insert(handIndex + 1, false);
        stood.Insert(handIndex + 1, false);

        activeHandIndex = handIndex;
        return true;
    }

    public bool IsHandDone(int i)
    {
        int s = playerHands[i].Skor();

        if (s >= 21) return true;
        if (stood[i]) return true;
        if (doubled[i]) return true;

        return false;
    }

    public bool MoveToNextPlayableHand()
    {
        for (int i = activeHandIndex + 1; i < playerHands.Count; i++)
        {
            if (!IsHandDone(i))
            {
                activeHandIndex = i;
                return true;
            }
        }
        return false;
    }

    public bool PlayerHasAnyBlackjackAtStart()
    {
        return playerHands.Count > 0 &&
               playerHands[0].kartlar.Count == 2 &&
               playerHands[0].Skor() == 21;
    }

    public bool DealerHasBlackjackAtStart()
    {
        return dealerHand.kartlar.Count == 2 &&
               dealerHand.Skor() == 21;
    }

    public HandResult ResolveHand(int handIndex)
    {
        int p = playerHands[handIndex].Skor();
        int d = dealerHand.Skor();

        bool pBust = p > 21;
        bool dBust = d > 21;

        bool pBJ = playerHands[handIndex].kartlar.Count == 2 && p == 21;
        bool dBJ = dealerHand.kartlar.Count == 2 && d == 21;

        var res = new HandResult
        {
            playerScore = p,
            dealerScore = d,
            playerBust = pBust,
            dealerBust = dBust,
            playerBlackjack = pBJ,
            dealerBlackjack = dBJ,
            outcome = RoundOutcome.None,
            message = ""
        };

        if (pBust)
        {
            res.outcome = RoundOutcome.DealerWin;
            res.message = "LOSE (Bust)";
            return res;
        }

        if (dBust)
        {
            res.outcome = RoundOutcome.PlayerWin;
            res.message = "WIN (Dealer bust)";
            return res;
        }

        if (p > d)
        {
            res.outcome = RoundOutcome.PlayerWin;
            res.message = "WIN";
        }
        else if (p < d)
        {
            res.outcome = RoundOutcome.DealerWin;
            res.message = "LOSE";
        }
        else
        {
            res.outcome = RoundOutcome.Push;
            res.message = "PUSH";
        }

        return res;
    }
}
