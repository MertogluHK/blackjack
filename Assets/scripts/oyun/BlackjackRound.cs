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

        // ✅ TEK EL
        playerHands.Add(new El());
        bets.Add(baseBet);
        doubled.Add(false);
        stood.Add(false);

        activeHandIndex = 0;
    }

    public void SetActiveHand(int i) => activeHandIndex = 0; // her zaman 0

    public Kart DealToPlayer(int handIndex = 0)
    {
        Kart k = deck.Draw();
        playerHands[0].Ekle(k);
        return k;
    }

    public Kart DealToDealer()
    {
        Kart k = deck.Draw();
        dealerHand.Ekle(k);
        return k;
    }

    public Kart Hit(int handIndex = 0) => DealToPlayer(0);

    public void Stand(int handIndex = 0) => stood[0] = true;

    public bool CanDoubleDown(int handIndex = 0)
    {
        var h = playerHands[0];
        return h.kartlar.Count == 2 && !doubled[0] && !stood[0];
    }

    public Kart DoubleDown(int handIndex = 0)
    {
        if (!CanDoubleDown(0)) return null;

        doubled[0] = true;
        bets[0] *= 2;

        return Hit(0);
    }

    public bool IsHandDone(int i = 0)
    {
        int s = playerHands[0].Skor();

        if (s >= 21) return true;
        if (stood[0]) return true;
        if (doubled[0]) return true;

        return false;
    }

    public HandResult ResolveHand(int handIndex = 0)
    {
        int p = playerHands[0].Skor();
        int d = dealerHand.Skor();

        bool pBust = p > 21;
        bool dBust = d > 21;

        bool pBJ = playerHands[0].kartlar.Count == 2 && p == 21;
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
