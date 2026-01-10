public class BlackjackRound
{
    public Deste deck;
    public El playerHand = new El();
    public El dealerHand = new El();

    public int baseBet { get; private set; }
    public int bet { get; private set; }

    public bool doubled { get; private set; }
    public bool stood { get; private set; }

    public BlackjackRound(int deckCount, int shuffleThreshold, int baseBet)
    {
        deck = new Deste(deckCount, shuffleThreshold);
        playerHand = new El();
        dealerHand = new El();

        this.baseBet = baseBet;
        bet = baseBet;

        doubled = false;
        stood = false;
    }

    public Kart DealToPlayer()
    {
        Kart k = deck.Draw();
        playerHand.Ekle(k);
        return k;
    }

    public Kart DealToDealer()
    {
        Kart k = deck.Draw();
        dealerHand.Ekle(k);
        return k;
    }

    public Kart Hit() => DealToPlayer();

    public void Stand() => stood = true;

    public bool CanDoubleDown()
    {
        return playerHand.kartlar.Count == 2 && !doubled && !stood;
    }

    public Kart DoubleDown()
    {
        if (!CanDoubleDown()) return null;
        doubled = true;
        bet *= 2;
        return Hit();
    }

    public bool IsPlayerDone()
    {
        int s = playerHand.Skor();
        if (s >= 21) return true;
        if (stood) return true;
        if (doubled) return true;
        return false;
    }

    public bool PlayerHasBlackjackAtStart()
        => playerHand.kartlar.Count == 2 && playerHand.Skor() == 21;

    public bool DealerHasBlackjackAtStart()
        => dealerHand.kartlar.Count == 2 && dealerHand.Skor() == 21;

    public HandResult Resolve()
    {
        int p = playerHand.Skor();
        int d = dealerHand.Skor();

        bool pBust = p > 21;
        bool dBust = d > 21;

        var res = new HandResult
        {
            playerScore = p,
            dealerScore = d,
            playerBust = pBust,
            dealerBust = dBust,
            playerBlackjack = (playerHand.kartlar.Count == 2 && p == 21),
            dealerBlackjack = (dealerHand.kartlar.Count == 2 && d == 21),
            outcome = RoundOutcome.None,
            message = ""
        };

        if (pBust) { res.outcome = RoundOutcome.DealerWin; res.message = "LOSE (Bust)"; return res; }
        if (dBust) { res.outcome = RoundOutcome.PlayerWin; res.message = "WIN (Dealer bust)"; return res; }

        if (p > d) { res.outcome = RoundOutcome.PlayerWin; res.message = "WIN"; }
        else if (p < d) { res.outcome = RoundOutcome.DealerWin; res.message = "LOSE"; }
        else { res.outcome = RoundOutcome.Push; res.message = "PUSH"; }

        return res;
    }
}
